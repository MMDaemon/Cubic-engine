using System;
using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using OpenTK.Graphics.ES30;

namespace CubicEngine.Model
{
	class ChunkManager
	{
		private Voxel[][,] _outerChunkFaces;
		private Voxel[,,] _currentVoxels;

		public ChunkManager()
		{
			_currentVoxels = _currentVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
		}

		private static Voxel[,,] CreateVoxels(Vector3i chunkPosition)
		{
			Vector3i position = new Vector3i(chunkPosition.X * Constants.ChunkSize.X, chunkPosition.Y * Constants.ChunkSize.Y, chunkPosition.Z * Constants.ChunkSize.Z);

			Voxel[,,] voxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						if (x < Constants.ChunkSize.X && y < Constants.ChunkSize.Y && z < Constants.ChunkSize.Z)
						{
							Vector3i voxelPosition = position + new Vector3i(x, y, z);
							voxels[x, y, z] = CreateVoxel(voxelPosition);
						}
					}
				}
			}
			return voxels;
		}

		private static Voxel CreateVoxel(Vector3i voxelPosition)
		{
			Voxel voxel = new Voxel();

			double worldHeight = 5 + 25 * (1 + Math.Sin((float)voxelPosition.X / 50)) + 25 * (1 + Math.Sin((float)voxelPosition.Z / 100)) + 25 * (1 + Math.Sin((float)voxelPosition.X / 20)) +
											 25 * (1 + Math.Sin((float)voxelPosition.Z / 10));
			if (voxelPosition.Y < worldHeight)
			{
				voxel.Materials.Add(MaterialType.Dirt, Constants.MaxAmount);
			}

			return voxel;
		}

		public Chunk GetChunk(int x, int y, int z)
		{
			return GetChunk(new Vector3i(x, y, z));
		}

		public Chunk GetChunk(Vector3i position)
		{
			return LoadChunk(position) ?? CreateChunk(position);
		}

		private Chunk LoadChunk(Vector3i chunkPosition)
		{
			//TODO implement Chunk loading
			return null;
		}

		private Chunk CreateChunk(Vector3i chunkPosition)
		{
			ChunkStatus status = CreateAndCheckVoxels(chunkPosition);

			if (status != ChunkStatus.Empty)
			{
				ChunkStatus statusTmp = CreateOuterFaces(chunkPosition);
				if (statusTmp == ChunkStatus.Surrounded)
				{
					status = statusTmp;
				}
				CheckOuterSurvaceVoxels();
			}

			Chunk chunk = new Chunk(chunkPosition, _currentVoxels) { Status = status };

			ResetMembers();

			return chunk;
		}

		private ChunkStatus CreateOuterFaces(Vector3i chunkPosition)
		{
			_outerChunkFaces = new Voxel[6][,];

			ChunkStatus status = ChunkStatus.Surrounded;

			#region top

			Vector3i currentPosition = chunkPosition + new Vector3i(0, 1, 0);

			_outerChunkFaces[(int)Direction.Top] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Z];

			Chunk chunk = LoadChunk(currentPosition);
			Voxel[,,] voxels = chunk != null ? chunk.Voxels : CreateVoxels(currentPosition);

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					_outerChunkFaces[(int)Direction.Top][x, z] = voxels[x, 0, z];
					if (_outerChunkFaces[(int)Direction.Top][x, z].Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region bottom

			currentPosition = chunkPosition + new Vector3i(0, -1, 0);

			_outerChunkFaces[(int)Direction.Bottom] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Z];

			chunk = LoadChunk(currentPosition);
			voxels = chunk != null ? chunk.Voxels : CreateVoxels(currentPosition);

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					_outerChunkFaces[(int)Direction.Bottom][x, z] = voxels[x, Constants.ChunkSize.Y - 1, z];
					if (_outerChunkFaces[(int)Direction.Bottom][x, z].Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region left

			currentPosition = chunkPosition + new Vector3i(-1, 0, 0);

			_outerChunkFaces[(int)Direction.Left] = new Voxel[Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			chunk = LoadChunk(currentPosition);
			voxels = chunk != null ? chunk.Voxels : CreateVoxels(currentPosition);

			for (int y = 0; y < Constants.ChunkSize.Y; y++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					_outerChunkFaces[(int)Direction.Left][y, z] = voxels[Constants.ChunkSize.X - 1, y, z];
					if (_outerChunkFaces[(int)Direction.Left][y, z].Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region right

			currentPosition = chunkPosition + new Vector3i(1, 0, 0);

			_outerChunkFaces[(int)Direction.Right] = new Voxel[Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			chunk = LoadChunk(currentPosition);
			voxels = chunk != null ? chunk.Voxels : CreateVoxels(currentPosition);

			for (int y = 0; y < Constants.ChunkSize.Y; y++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					_outerChunkFaces[(int)Direction.Right][y, z] = voxels[0, y, z];
					if (_outerChunkFaces[(int)Direction.Right][y, z].Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region front

			currentPosition = chunkPosition + new Vector3i(0, 0, 1);

			_outerChunkFaces[(int)Direction.Front] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y];

			chunk = LoadChunk(currentPosition);
			voxels = chunk != null ? chunk.Voxels : CreateVoxels(currentPosition);

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					_outerChunkFaces[(int)Direction.Front][x, y] = voxels[x, y, 0];
					if (_outerChunkFaces[(int)Direction.Front][x, y].Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region back

			currentPosition = chunkPosition + new Vector3i(0, 0, -1);

			_outerChunkFaces[(int)Direction.Back] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y];

			chunk = LoadChunk(currentPosition);
			voxels = chunk != null ? chunk.Voxels : CreateVoxels(currentPosition);

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					_outerChunkFaces[(int)Direction.Back][x, y] = voxels[x, y, Constants.ChunkSize.Z - 1];
					if (_outerChunkFaces[(int)Direction.Back][x, y].Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			return status;
		}

		private ChunkStatus CreateAndCheckVoxels(Vector3i chunkPosition)
		{
			Vector3i position = new Vector3i(chunkPosition.X * Constants.ChunkSize.X, chunkPosition.Y * Constants.ChunkSize.Y, chunkPosition.Z * Constants.ChunkSize.Z);

			ChunkStatus status = ChunkStatus.Full;

			int fillAmount = 0;
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						if (x < Constants.ChunkSize.X && y < Constants.ChunkSize.Y && z < Constants.ChunkSize.Z)
						{
							Vector3i voxelPosition = position + new Vector3i(x, y, z);
							_currentVoxels[x, y, z] = CreateVoxel(voxelPosition);

							if (_currentVoxels[x, y, z].Materials.IsEmpty())
							{
								status = ChunkStatus.None;
							}
							else
							{
								fillAmount++;
							}
						}
						if (x > 1 && y > 1 && z > 1 && !_currentVoxels[x - 1, y - 1, z - 1].Materials.IsEmpty())
						{
							CheckSurfaceVoxel(x - 1, y - 1, z - 1);
						}
					}
				}
			}
			if (fillAmount == 0)
			{
				status = ChunkStatus.Empty;
			}
			return status;
		}

		private void CheckSurfaceVoxel(int x, int y, int z)
		{
			_currentVoxels[x, y, z].Surface = !(GetVoxel(x - 1, y, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x + 1, y, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y - 1, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y + 1, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y, z - 1).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y, z + 1).Materials.Amount == Constants.MaxAmount);
		}

		private void CheckOuterSurvaceVoxels()
		{
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					if (!_currentVoxels[x, y, 0].Materials.IsEmpty())
					{
						CheckSurfaceVoxel(x, y, 0);
					}
					if (!_currentVoxels[x, y, Constants.ChunkSize.Z - 1].Materials.IsEmpty())
					{
						CheckSurfaceVoxel(x, y, Constants.ChunkSize.Z - 1);
					}
				}
			}
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
				{
					if (!_currentVoxels[x, 0, z].Materials.IsEmpty())
					{
						CheckSurfaceVoxel(x, 0, z);
					}
					if (!_currentVoxels[x, Constants.ChunkSize.Y - 1, z].Materials.IsEmpty())
					{
						CheckSurfaceVoxel(x, Constants.ChunkSize.Y - 1, z);
					}
				}
			}

			for (int y = 1; y < Constants.ChunkSize.Y - 1; y++)
			{
				for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
				{
					if (!_currentVoxels[0, y, z].Materials.IsEmpty())
					{
						CheckSurfaceVoxel(0, y, z);
					}
					if (!_currentVoxels[Constants.ChunkSize.X - 1, y, z].Materials.IsEmpty())
					{
						CheckSurfaceVoxel(Constants.ChunkSize.X - 1, y, z);
					}
				}
			}
		}

		private Voxel GetVoxel(int x, int y, int z)
		{
			if (x == -1)
			{
				return _outerChunkFaces[(int)Direction.Left][y, z];
			}
			if (x == Constants.ChunkSize.X)
			{
				return _outerChunkFaces[(int)Direction.Right][y, z];
			}
			if (y == -1)
			{
				return _outerChunkFaces[(int)Direction.Bottom][x, z];
			}
			if (y == Constants.ChunkSize.Y)
			{
				return _outerChunkFaces[(int)Direction.Top][x, z];
			}
			if (z == -1)
			{
				return _outerChunkFaces[(int)Direction.Back][x, y];
			}
			if (z == Constants.ChunkSize.Z)
			{
				return _outerChunkFaces[(int)Direction.Front][x, y];
			}
			return _currentVoxels[x, y, z];
		}

		private void ResetMembers()
		{
			_outerChunkFaces = null;
			_currentVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
		}
	}
}
