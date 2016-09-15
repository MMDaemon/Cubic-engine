using System.Collections.Generic;
using System.Text;
using CubicEngine.Model;
using CubicEngine.Resources;
using CubicEngine.Utils;
using GraphicsHelper.GraphicsUtils;
using GraphicsHelper.ShaderUtils;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace CubicEngine.View
{
	internal class Renderer
	{
		private readonly Shader _shader;
		private readonly Texture _materialTexture;
		private readonly List<VertexArrayObject> _chunkVertexArrayObjects;
		private readonly List<int> _particleCounts;
		private readonly List<BufferObject> _materialBuffers;
		private readonly List<BufferObject> _extentBuffers;


		public CameraFirstPerson Camera { get; } = new CameraFirstPerson();

		public Renderer()
		{

			Camera.FarClip = 500;
			Camera.Position = new Vector3(0, 200, 0);

			var sVertex = Encoding.UTF8.GetString(Shaders.vertex);
			var sFragment = Encoding.UTF8.GetString(Shaders.fragment);
			_shader = ShaderLoader.FromStrings(sVertex, sFragment);

			_materialTexture = TextureLoader.FromBitmap(MaterialManager.Instance.GetMaterialsAsBitmap());
			_materialTexture.FilterNearest();

			_chunkVertexArrayObjects = new List<VertexArrayObject>();
			_particleCounts = new List<int>();
			_materialBuffers = new List<BufferObject>();
			_extentBuffers = new List<BufferObject>();

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

		}

		public void AddChunk(Chunk chunk)
		{
			VertexArrayObject chunkVertexArrayObject = CreateVertexArrayObject(new Cube(1));
			CreateCubeInstances(chunk, chunkVertexArrayObject, _shader);
		}

		public void ResizeWindow(int width, int height)
		{
			Camera.Aspect = (float)width / height;
			GL.Viewport(0, 0, width, height);
		}

		public void Render()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_shader.Begin();

			_materialTexture.BindToUniform(_shader.GetUniformLocation("materialTexture"), TextureUnit.Texture0);

			GL.Uniform1(_shader.GetUniformLocation("totalMaterialCount"), MaterialManager.Instance.MaterialCount);
			GL.Uniform3(_shader.GetUniformLocation("lightDirection"), Vector3.Normalize(new Vector3(2, 3, 1)));
			GL.Uniform4(_shader.GetUniformLocation("lightColor"), new Color4(1, 1, 1, 1f));

			Matrix4 cam = Camera.CalcMatrix();
			GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref cam);

			for (int i = 0; i < _chunkVertexArrayObjects.Count; i++)
			{
				_materialBuffers[i].ActivateBind(3);
				_extentBuffers[i].ActivateBind(4);
				_chunkVertexArrayObjects[i].Draw(_particleCounts[i]);
				_materialBuffers[i].Deactive();
				_extentBuffers[i].Deactive();
			}

			_materialTexture.EndUse();

			_shader.End();
		}

		private VertexArrayObject CreateVertexArrayObject(Mesh mesh)
		{
			var vao = new VertexArrayObject();
			vao.SetAttribute(_shader.GetAttributeLocation("position"), mesh.Positions.ToArray(), VertexAttribPointerType.Float, 3);
			vao.SetAttribute(_shader.GetAttributeLocation("normal"), mesh.Normals.ToArray(), VertexAttribPointerType.Float, 3);
			vao.SetAttribute(_shader.GetAttributeLocation("uv"), mesh.Uvs.ToArray(), VertexAttribPointerType.Float, 2);
			vao.SetId(mesh.Ids.ToArray(), PrimitiveType.Triangles);
			return vao;
		}

		private void CreateCubeInstances(Chunk chunk, VertexArrayObject vao, Shader shader)
		{
			int particleCount = 0;
			List<Vector3> instancePositions = new List<Vector3>();
			List<Vector3> instanceMaterialDirections = new List<Vector3>();
			List<int> instanceMaterialOffsets = new List<int>();
			List<int> instanceMaterialCounts = new List<int>();

			List<int> materials = new List<int>();
			List<float> extents = new List<float>();

			int offset = 0;
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						if (chunk[x, y, z].Surface)
						{
							particleCount++;

							Vector3 actualPos = new Vector3(chunk.Position.X * Constants.ChunkSize.X, chunk.Position.Y * Constants.ChunkSize.Y, chunk.Position.Z * Constants.ChunkSize.Z) + new Vector3(x, y, z);
							instancePositions.Add(actualPos);

							Vector3 materialDirection = new Vector3
							{
								X = chunk[x - 1, y, z].Materials.Amount - chunk[x + 1, y, z].Materials.Amount,
								Y = chunk[x, y - 1, z].Materials.Amount - chunk[x, y + 1, z].Materials.Amount,
								Z = chunk[x, y, z - 1].Materials.Amount - chunk[x, y, z + 1].Materials.Amount
							};
							if (materialDirection.Equals(new Vector3(0, 0, 0)))
							{
								materialDirection = new Vector3(0, 1, 0);
							}
							instanceMaterialDirections.Add(materialDirection.Normalized());

							float[] currentExtents;
							materials.AddRange(GetMaterialsFromVoxel(out currentExtents, chunk, x, y, z));
							extents.AddRange(currentExtents);
							instanceMaterialOffsets.Add(offset);
							instanceMaterialCounts.Add(currentExtents.Length);
							offset += currentExtents.Length;
						}
					}
				}
			}

			vao.SetAttribute(shader.GetAttributeLocation("instancePosition"), instancePositions.ToArray(), VertexAttribPointerType.Float, 3, true);
			vao.SetAttribute(shader.GetAttributeLocation("instanceMaterialDirection"), instanceMaterialDirections.ToArray(), VertexAttribPointerType.Float, 3, true);
			vao.SetAttribute(shader.GetAttributeLocation("instanceMaterialOffset"), instanceMaterialOffsets.ToArray(), VertexAttribPointerType.Float, 1, true);
			vao.SetAttribute(shader.GetAttributeLocation("instanceMaterialCount"), instanceMaterialCounts.ToArray(), VertexAttribPointerType.Float, 1, true);

			_chunkVertexArrayObjects.Add(vao);
			_particleCounts.Add(particleCount);

			BufferObject buffer = new BufferObject(BufferTarget.ShaderStorageBuffer);
			buffer.Set(materials.ToArray(), BufferUsageHint.StaticCopy);
			_materialBuffers.Add(buffer);
			buffer = new BufferObject(BufferTarget.ShaderStorageBuffer);
			buffer.Set(extents.ToArray(), BufferUsageHint.StaticCopy);
			_extentBuffers.Add(buffer);
		}

		private int[] GetMaterialsFromVoxel(out float[] extents, Chunk chunk, int x, int y, int z)
		{
			List<int> materialList = new List<int>();
			List<float> extentList = new List<float>();

			Material currentMaterial = new Material(0, 0);
			foreach (Material material in chunk[x, y, z].Materials)
			{
				if (material.Amount > currentMaterial.Amount)
				{
					currentMaterial = material;
				}
			}
			materialList.Add(currentMaterial.TypeId);
			extentList.Add(1);

			extents = extentList.ToArray();
			return materialList.ToArray();
		}
	}
}
