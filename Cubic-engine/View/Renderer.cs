using System.Collections.Generic;
using System.Text;
using CubicEngine.Model;
using CubicEngine.Resources;
using CubicEngine.Utils;
using GraphicsHelper.GraphicsUtils;
using GraphicsHelper.ShaderUtils;
using Model;
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

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

		}

		public void AddChunk(Chunk chunk)
		{
			VertexArrayObject chunkVertexArrayObject = CreateVertexArrayObject(new Cube(1));
			_particleCounts.Add(CreateCubeInstances(chunk, chunkVertexArrayObject, _shader));
			_chunkVertexArrayObjects.Add(chunkVertexArrayObject);
		}

		public void ResizeWindow(int width, int height)
		{
			Camera.Aspect = (float)width / (float)height;
			GL.Viewport(0, 0, width, height);
		}

		public void Render()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_shader.Begin();

			_materialTexture.BindToUniform(_shader.GetUniformLocation("materialTexture"), TextureUnit.Texture0);

			GL.Uniform1(_shader.GetUniformLocation("materialCount"), MaterialManager.Instance.MaterialCount);
			GL.Uniform3(_shader.GetUniformLocation("lightDirection"), Vector3.Normalize(new Vector3(2, 3, 1)));
			GL.Uniform4(_shader.GetUniformLocation("lightColor"), new Color4(1, 1, 1, 1f));

			Matrix4 cam = Camera.CalcMatrix();
			GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref cam);

			for (int i = 0; i < _chunkVertexArrayObjects.Count; i++)
			{
				_chunkVertexArrayObjects[i].Draw(_particleCounts[i]);
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

		private int CreateCubeInstances(Chunk chunk, VertexArrayObject vao, Shader shader)
		{
			int particleCount = 0;
			List<Vector3> instancePositions = new List<Vector3>();
			List<int> instanceMaterials = new List<int>();
			List<Vector3> instanceMaterialDirections = new List<Vector3>();

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

							instanceMaterials.Add(GetMaterialsFromVoxel(chunk, x, y, z, 1)[0]);

							Vector3 materialDirection = new Vector3
							{
								X = chunk[x - 1, y, z].Materials.Amount - chunk[x + 1, y, z].Materials.Amount,
								Y = chunk[x, y - 1, z].Materials.Amount - chunk[x, y + 1, z].Materials.Amount,
								Z = chunk[x, y, z - 1].Materials.Amount - chunk[x, y, z + 1].Materials.Amount
							};
							instanceMaterialDirections.Add(materialDirection.Normalized());
						}
					}
				}
			}

			vao.SetAttribute(shader.GetAttributeLocation("instancePosition"), instancePositions.ToArray(), VertexAttribPointerType.Float, 3, true);
			vao.SetAttribute(shader.GetAttributeLocation("instanceMaterial"), instanceMaterials.ToArray(), VertexAttribPointerType.Float, 4, true);
			vao.SetAttribute(shader.GetAttributeLocation("instanceMaterialDirection"), instanceMaterialDirections.ToArray(), VertexAttribPointerType.Float, 3, true);

			return particleCount;
		}

		private int[] GetMaterialsFromVoxel(Chunk chunk, int x, int y, int z, int sitesCount)
		{
			int[] color = new int[sitesCount];
			int currentMax = 0;
			foreach (Material material in chunk[x, y, z].Materials)
			{
				if (material.Amount > currentMax)
				{
					currentMax = material.Amount;
					color[0] = material.TypeId;
				}
			}
			return color;
		}
	}
}
