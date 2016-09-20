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
using System;

namespace CubicEngine.View
{
	internal class Renderer
	{
		public event EventHandler ReRender;

		private int _renderMethod;
		private int _renderAlgorythm;
		private readonly int[] _algorythmCounts = new int[] { 4 };

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
			Camera.Position = new Vector3(6, 10, 30);
			Camera.Heading -= (float)(0.32 * Math.PI);
			Camera.Tilt += (float)(0.1 * Math.PI);

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

			GL.ClearColor(new Color4(0.9f, 0.9f, 0.99f, 1));

		}

		public void SwitchRenderMethod()
		{
			_renderMethod = (_renderMethod + 1) % _algorythmCounts.Length;
			_renderAlgorythm = 0;
			ClearBuffers();
			OnReRender();

		}

		public void SwitchRenderAlgorythm()
		{
			_renderAlgorythm = (_renderAlgorythm + 1) % _algorythmCounts[_renderMethod];
			ClearBuffers();
			OnReRender();
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

		protected virtual void OnReRender()
		{
			EventArgs e = new EventArgs();
			ReRender?.Invoke(this, e);
		}

		private void ClearBuffers()
		{
			_chunkVertexArrayObjects.Clear();
			_particleCounts.Clear();
			_materialBuffers.Clear();
			_extentBuffers.Clear();
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
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					for (int y = 0; y < Constants.ChunkSize.Y; y++)
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
			int[] materials;

			switch (_renderMethod)
			{
				case 0:

					switch (_renderAlgorythm)
					{
						case 0:

							materials = GetOneWithMost(out extents, chunk, x, y, z);

							break;

						case 1:

							materials = GetOneWithLeast(out extents, chunk, x, y, z);

							break;

						case 2:

							materials = GetOneBySurrounding(out extents, chunk, x, y, z);

							break;

						case 3:

							materials = GetOneBySixSurrounding(out extents, chunk, x, y, z);

							break;

						default:

							materials = GetOneWithMost(out extents, chunk, x, y, z);

							break;
					}

					break;
				default:

					materials = GetOneWithMost(out extents, chunk, x, y, z);

					break;
			}

			return materials;
		}

		private int[] GetOneWithMost(out float[] extents, Chunk chunk, int x, int y, int z)
		{
			List<int> materialList = new List<int>();
			List<float> extentList = new List<float>();

			Material currentMaterial = new Material(0, -1);
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

		private int[] GetOneWithLeast(out float[] extents, Chunk chunk, int x, int y, int z)
		{
			List<int> materialList = new List<int>();
			List<float> extentList = new List<float>();

			Material leastMaterial = new Material(0, Constants.MaxAmount + 1);
			foreach (Material material in chunk[x, y, z].Materials)
			{
				if (material.Amount < leastMaterial.Amount)
				{
					leastMaterial = material;
				}
			}

			materialList.Add(leastMaterial.TypeId);
			extentList.Add(1);

			extents = extentList.ToArray();
			return materialList.ToArray();
		}

		private int[] GetOneBySurrounding(out float[] extents, Chunk chunk, int x, int y, int z)
		{
			Vector3I voxelPosition = new Vector3I(x, y, z);

			List<int> materialList = new List<int>();
			List<float> extentList = new List<float>();

			List<Vector3I> visibleNeighborPositions = new List<Vector3I> { voxelPosition };
			List<Vector3I> restNeighborPositions = new List<Vector3I>();

			#region fill lists

			foreach (Vector3I direction in Constants.DirectionVectors)
			{
				Vector3I neighborVector = voxelPosition + direction;
				if (chunk[neighborVector].Materials.Amount > 0)
				{
					restNeighborPositions.Add(neighborVector);
				}
			}

			foreach (Vector3I direction in Constants.DiagonalDirectionVectors)
			{
				Vector3I neighborVector = voxelPosition + direction;
				if (chunk[neighborVector].Materials.Amount > 0)
				{
					restNeighborPositions.Add(neighborVector);
				}
			}

			foreach (Vector3I direction in Constants.DirectionVectors)
			{
				if (chunk[voxelPosition + direction].Materials.Amount == 0)
				{
					Vector3I[] neighborToFacePositions = GetNeighborsToFace(chunk, direction, voxelPosition);

					foreach (Vector3I position in neighborToFacePositions)
					{
						if (restNeighborPositions.Remove(position))
						{
							visibleNeighborPositions.Add(position);
						}
					}
				}
			}

			#endregion

			Dictionary<int, float> visibleAverageDistribution = GetAverageDistribution(chunk, visibleNeighborPositions);
			Dictionary<int, float> restAverageDistribution = GetAverageDistribution(chunk, restNeighborPositions);

			int currentMaterial = 0;
			float currentValue = float.MinValue;
			foreach (Material material in chunk[voxelPosition].Materials)
			{
				float value = visibleAverageDistribution[material.TypeId] / (restAverageDistribution.ContainsKey(material.TypeId) ? restAverageDistribution[material.TypeId] : 0.00001f);

				if (value > currentValue)
				{
					currentValue = value;
					currentMaterial = material.TypeId;
				}
			}

			materialList.Add(currentMaterial);
			extentList.Add(1);

			extents = extentList.ToArray();
			return materialList.ToArray();
		}

		private int[] GetOneBySixSurrounding(out float[] extents, Chunk chunk, int x, int y, int z)
		{
			Vector3I voxelPosition = new Vector3I(x, y, z);

			List<int> materialList = new List<int>();
			List<float> extentList = new List<float>();

			Dictionary<int, float> averageValues = new Dictionary<int, float>();

			foreach (Vector3I currentDirection in Constants.DirectionVectors)
			{
				if (chunk[voxelPosition + currentDirection].Materials.Amount == 0)
				{
					List<Vector3I> visibleNeighborPositions = new List<Vector3I> { voxelPosition };
					List<Vector3I> restNeighborPositions = new List<Vector3I>();

					#region fill lists

					foreach (Vector3I direction in Constants.DirectionVectors)
					{
						Vector3I neighborVector = voxelPosition + direction;
						if (chunk[neighborVector].Materials.Amount > 0)
						{
							restNeighborPositions.Add(neighborVector);
						}
					}

					foreach (Vector3I direction in Constants.DiagonalDirectionVectors)
					{
						Vector3I neighborVector = voxelPosition + direction;
						if (chunk[neighborVector].Materials.Amount > 0)
						{
							restNeighborPositions.Add(neighborVector);
						}
					}

					Vector3I[] neighborToFacePositions = GetNeighborsToFace(chunk, currentDirection, voxelPosition);

					foreach (Vector3I position in neighborToFacePositions)
					{
						if (restNeighborPositions.Remove(position))
						{
							visibleNeighborPositions.Add(position);
						}
					}

					#endregion

					Dictionary<int, float> visibleAverageDistribution = GetAverageDistribution(chunk, visibleNeighborPositions);
					Dictionary<int, float> restAverageDistribution = GetAverageDistribution(chunk, restNeighborPositions);

					foreach (Material material in chunk[voxelPosition].Materials)
					{
						float value = visibleAverageDistribution[material.TypeId] / (restAverageDistribution.ContainsKey(material.TypeId) ? restAverageDistribution[material.TypeId] : 0.00001f);

						if (averageValues.ContainsKey(material.TypeId))
						{
							averageValues[material.TypeId] += value;
						}
						else
						{
							averageValues.Add(material.TypeId, value);
						}
					}
				}
			}

			int currentMaterial = 0;
			float currentValue = 0;
			foreach (KeyValuePair<int, float> material in averageValues)
			{
				if (currentValue < material.Value)
				{
					currentValue = material.Value;
					currentMaterial = material.Key;
				}
			}

			extentList.Add(1);
			materialList.Add(currentMaterial);

			extents = extentList.ToArray();
			return materialList.ToArray();
		}

		private int[] GetSixBySurrounding(out float[] extents, Chunk chunk, int x, int y, int z)
		{
			Vector3I voxelPosition = new Vector3I(x, y, z);

			List<int> materialList = new List<int>();
			List<float> extentList = new List<float>();

			foreach (Vector3I currentDirection in Constants.DirectionVectors)
			{
				if (chunk[voxelPosition + currentDirection].Materials.Amount == 0)
				{
					List<Vector3I> visibleNeighborPositions = new List<Vector3I> { voxelPosition };
					List<Vector3I> restNeighborPositions = new List<Vector3I>();

					#region fill lists

					foreach (Vector3I direction in Constants.DirectionVectors)
					{
						Vector3I neighborVector = voxelPosition + direction;
						if (chunk[neighborVector].Materials.Amount > 0)
						{
							restNeighborPositions.Add(neighborVector);
						}
					}

					foreach (Vector3I direction in Constants.DiagonalDirectionVectors)
					{
						Vector3I neighborVector = voxelPosition + direction;
						if (chunk[neighborVector].Materials.Amount > 0)
						{
							restNeighborPositions.Add(neighborVector);
						}
					}


					Vector3I[] neighborToFacePositions = GetNeighborsToFace(chunk, currentDirection, voxelPosition);

					foreach (Vector3I position in neighborToFacePositions)
					{
						if (restNeighborPositions.Remove(position))
						{
							visibleNeighborPositions.Add(position);
						}
					}


					#endregion

					Dictionary<int, float> visibleAverageDistribution = GetAverageDistribution(chunk, visibleNeighborPositions);
					Dictionary<int, float> restAverageDistribution = GetAverageDistribution(chunk, restNeighborPositions);

					int currentMaterial = 0;
					float currentValue = float.MinValue;
					foreach (Material material in chunk[voxelPosition].Materials)
					{
						float value = visibleAverageDistribution[material.TypeId] / (restAverageDistribution.ContainsKey(material.TypeId) ? restAverageDistribution[material.TypeId] : 0.00001f);

						if (value > currentValue)
						{
							currentValue = value;
							currentMaterial = material.TypeId;
						}
					}

					materialList.Add(currentMaterial);
					extentList.Add(1);
				}
			}

			extents = extentList.ToArray();
			return materialList.ToArray();
		}

		private Dictionary<int, float> GetAverageDistribution(Chunk chunk, List<Vector3I> positions)
		{
			Dictionary<int, float> averageDistribution = new Dictionary<int, float>();

			Dictionary<int, float> materialDistribution = new Dictionary<int, float>();
			foreach (Vector3I position in positions)
			{
				Dictionary<int, float> distribution = chunk[position].Materials.GetMaterialDistribution();

				foreach (KeyValuePair<int, float> material in distribution)
				{
					if (materialDistribution.ContainsKey(material.Key))
					{
						materialDistribution[material.Key] += material.Value;
					}
					else
					{
						materialDistribution.Add(material.Key, material.Value);
					}
				}
			}
			foreach (KeyValuePair<int, float> material in materialDistribution)
			{
				averageDistribution.Add(material.Key, material.Value / positions.Count);
			}

			return averageDistribution;
		}

		private Vector3I[] GetNeighborsToFace(Chunk chunk, Vector3I direction, Vector3I voxelPosition)
		{
			List<Vector3I> neighbors = new List<Vector3I>();

			Vector3I currentVector;

			List<Vector3I> orthogonals = new List<Vector3I>();

			if (direction.X != 0)
			{
				orthogonals.Add(new Vector3I(0, 1, 0));
				orthogonals.Add(new Vector3I(0, -1, 0));
				orthogonals.Add(new Vector3I(0, 0, 1));
				orthogonals.Add(new Vector3I(0, 0, -1));
				orthogonals.Add(new Vector3I(0, 1, 1));
				orthogonals.Add(new Vector3I(0, 1, -1));
				orthogonals.Add(new Vector3I(0, -1, 1));
				orthogonals.Add(new Vector3I(0, -1, -1));
			}

			if (direction.Y != 0)
			{
				orthogonals.Add(new Vector3I(1, 0, 0));
				orthogonals.Add(new Vector3I(-1, 0, 0));
				orthogonals.Add(new Vector3I(0, 0, 1));
				orthogonals.Add(new Vector3I(0, 0, -1));
				orthogonals.Add(new Vector3I(1, 0, 1));
				orthogonals.Add(new Vector3I(1, 0, -1));
				orthogonals.Add(new Vector3I(-1, 0, 1));
				orthogonals.Add(new Vector3I(-1, 0, -1));
			}

			if (direction.Z != 0)
			{
				orthogonals.Add(new Vector3I(1, 0, 0));
				orthogonals.Add(new Vector3I(-1, 0, 0));
				orthogonals.Add(new Vector3I(0, 1, 0));
				orthogonals.Add(new Vector3I(0, -1, 0));
				orthogonals.Add(new Vector3I(1, 1, 0));
				orthogonals.Add(new Vector3I(1, -1, 0));
				orthogonals.Add(new Vector3I(-1, 1, 0));
				orthogonals.Add(new Vector3I(-1, -1, 0));
			}

			for (int i = 0; i < orthogonals.Count; i++)
			{
				currentVector = orthogonals[i];
				if (chunk[voxelPosition + direction + currentVector].Materials.Amount > 0)
				{
					neighbors.Add(voxelPosition + direction + currentVector);
				}
				else if (chunk[voxelPosition + currentVector].Materials.Amount > 0)
				{
					neighbors.Add(voxelPosition + currentVector);
				}
			}

			return neighbors.ToArray();
		}
	}
}
