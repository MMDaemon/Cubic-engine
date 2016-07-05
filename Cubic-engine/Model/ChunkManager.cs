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
			ChunkStatus status = CreateOuterFaces(chunkPosition);

			ChunkStatus statusTmp = CreateVoxels(chunkPosition);

			if (statusTmp == ChunkStatus.Empty || status == ChunkStatus.None)
			{
				status = statusTmp;
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

			_outerChunkFaces[(int) Direction.Top] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Z];

			Chunk chunk = LoadChunk(currentPosition);
			if (chunk != null)
			{
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Top][x, z] = chunk[x, 0, z];
						if (_outerChunkFaces[(int) Direction.Top][x, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}
			else
			{
				CreateVoxels(currentPosition, false);
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Top][x, z] = _currentVoxels[x, 0, z];
						if (_outerChunkFaces[(int)Direction.Top][x, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}

			#endregion

			#region bottom

			currentPosition = chunkPosition + new Vector3i(0, -1, 0);

			_outerChunkFaces[(int)Direction.Bottom] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Z];

			chunk = LoadChunk(currentPosition);
			if (chunk != null)
			{
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Bottom][x, z] = chunk[x, Constants.ChunkSize.Y - 1, z];
						if (_outerChunkFaces[(int)Direction.Bottom][x, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}
			else
			{
				CreateVoxels(currentPosition, false);
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Bottom][x, z] = _currentVoxels[x, Constants.ChunkSize.Y - 1, z];
						if (_outerChunkFaces[(int)Direction.Bottom][x, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}

			#endregion

			#region left

			currentPosition = chunkPosition + new Vector3i(-1, 0, 0);

			_outerChunkFaces[(int)Direction.Left] = new Voxel[Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			chunk = LoadChunk(currentPosition);
			if (chunk != null)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Left][y, z] = chunk[Constants.ChunkSize.X - 1, y, z];
						if (_outerChunkFaces[(int)Direction.Left][y, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}
			else
			{
				CreateVoxels(currentPosition, false);
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Left][y, z] = _currentVoxels[Constants.ChunkSize.X - 1, y, z];
						if (_outerChunkFaces[(int)Direction.Left][y, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}

			#endregion

			#region right

			currentPosition = chunkPosition + new Vector3i(1, 0, 0);

			_outerChunkFaces[(int)Direction.Right] = new Voxel[Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			chunk = LoadChunk(currentPosition);
			if (chunk != null)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Right][y, z] = chunk[0, y, z];
						if (_outerChunkFaces[(int)Direction.Right][y, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}
			else
			{
				CreateVoxels(currentPosition, false);
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_outerChunkFaces[(int)Direction.Right][y, z] = _currentVoxels[0, y, z];
						if (_outerChunkFaces[(int)Direction.Right][y, z].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}

			#endregion

			#region front

			currentPosition = chunkPosition + new Vector3i(0, 0, 1);

			_outerChunkFaces[(int)Direction.Front] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y];

			chunk = LoadChunk(currentPosition);
			if (chunk != null)
			{
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int y = 0; y < Constants.ChunkSize.Y; y++)
					{
						_outerChunkFaces[(int)Direction.Front][x, y] = chunk[y, y, 0];
						if (_outerChunkFaces[(int)Direction.Front][x, y].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}
			else
			{
				CreateVoxels(currentPosition, false);
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int y = 0; y < Constants.ChunkSize.Y; y++)
					{
						_outerChunkFaces[(int)Direction.Front][x, y] = _currentVoxels[x, y, 0];
						if (_outerChunkFaces[(int)Direction.Front][x, y].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}

			#endregion

			#region back

			currentPosition = chunkPosition + new Vector3i(0, 0, -1);

			_outerChunkFaces[(int)Direction.Back] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y];

			chunk = LoadChunk(currentPosition);
			if (chunk != null)
			{
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int y = 0; y < Constants.ChunkSize.Y; y++)
					{
						_outerChunkFaces[(int)Direction.Back][x, y] = chunk[x, y, Constants.ChunkSize.Z - 1];
						if (_outerChunkFaces[(int)Direction.Back][x, y].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}
			else
			{
				CreateVoxels(currentPosition, false);
				for (int x = 0; x < Constants.ChunkSize.X; x++)
				{
					for (int y = 0; y < Constants.ChunkSize.Y; y++)
					{
						_outerChunkFaces[(int)Direction.Back][x, y] = _currentVoxels[x, y, Constants.ChunkSize.Z - 1];
						if (_outerChunkFaces[(int)Direction.Back][x, y].Materials.IsEmpty())
						{
							status = ChunkStatus.None;
						}
					}
				}
			}

			#endregion

			return status;
		}

		private ChunkStatus CreateVoxels(Vector3i chunkPosition, bool surfaceVoxelCheck = true)
		{
			Vector3i position = new Vector3i(chunkPosition.X * Constants.ChunkSize.X, chunkPosition.Y * Constants.ChunkSize.Y, chunkPosition.Z * Constants.ChunkSize.Z);

			ChunkStatus status = ChunkStatus.Full;

			int fillAmount = 0;
			for (int x = 0; x <= Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y <= Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z <= Constants.ChunkSize.Z; z++)
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
						if (surfaceVoxelCheck && x > 0 && y > 0 && z > 0 && !_currentVoxels[x - 1, y - 1, z - 1].Materials.IsEmpty())
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

		private Voxel CreateVoxel(Vector3i voxelPosition)
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

		private void CheckSurfaceVoxel(int x, int y, int z)
		{
			_currentVoxels[x, y, z].Surface = !(GetVoxel(x - 1, y, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x + 1, y, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y - 1, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y + 1, z).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y, z - 1).Materials.Amount == Constants.MaxAmount &&
												GetVoxel(x, y, z + 1).Materials.Amount == Constants.MaxAmount);
		}

		private void CheckSurfaceVoxel(Vector3i voxelPositionInChunk)
		{
			CheckSurfaceVoxel(voxelPositionInChunk.X, voxelPositionInChunk.Y, voxelPositionInChunk.Z);
		}

		private void SetOuterSurvaceVoxels()
		{
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					if (!_currentVoxels[x, y, 0].Materials.IsEmpty())
					{
						_currentVoxels[x, y, 0].Surface = true;
					}
					if (!_currentVoxels[x, y, (int)Constants.ChunkSize.Z - 1].Materials.IsEmpty())
					{
						_currentVoxels[x, y, (int)Constants.ChunkSize.Z - 1].Surface = true;
					}
				}
			}
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
				{
					if (!_currentVoxels[x, 0, z].Materials.IsEmpty())
					{
						_currentVoxels[x, 0, z].Surface = true;
					}
					if (!_currentVoxels[x, (int)Constants.ChunkSize.Y - 1, z].Materials.IsEmpty())
					{
						_currentVoxels[x, (int)Constants.ChunkSize.Y - 1, z].Surface = true;
					}
				}
			}

			for (int y = 1; y < Constants.ChunkSize.Y - 1; y++)
			{
				for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
				{
					if (!_currentVoxels[0, y, z].Materials.IsEmpty())
					{
						_currentVoxels[0, y, z].Surface = true;
					}
					if (!_currentVoxels[(int)Constants.ChunkSize.X - 1, y, z].Materials.IsEmpty())
					{
						_currentVoxels[(int)Constants.ChunkSize.X - 1, y, z].Surface = true;
					}
				}
			}
		}

		private Voxel GetVoxel(int x, int y, int z)
		{
			return GetVoxel(new Vector3i(x, y, z));
		}

		private Voxel GetVoxel(Vector3i voxelPositionInChunk)
		{
			if (voxelPositionInChunk.X == -1)
			{
				return _outerChunkFaces[(int) Direction.Left][voxelPositionInChunk.Y, voxelPositionInChunk.Z];
			}
			if (voxelPositionInChunk.X == Constants.ChunkSize.X)
			{
				return _outerChunkFaces[(int)Direction.Right][voxelPositionInChunk.Y, voxelPositionInChunk.Z];
			}
			if (voxelPositionInChunk.Y == -1)
			{
				return _outerChunkFaces[(int)Direction.Bottom][voxelPositionInChunk.X, voxelPositionInChunk.Z];
			}
			if (voxelPositionInChunk.Y == Constants.ChunkSize.Y)
			{
				return _outerChunkFaces[(int)Direction.Top][voxelPositionInChunk.X, voxelPositionInChunk.Z];
			}
			if (voxelPositionInChunk.Z == -1)
			{
				return _outerChunkFaces[(int)Direction.Back][voxelPositionInChunk.X, voxelPositionInChunk.Y];
			}
			if (voxelPositionInChunk.Z == Constants.ChunkSize.Z)
			{
				return _outerChunkFaces[(int)Direction.Front][voxelPositionInChunk.X, voxelPositionInChunk.Y];
			}
			return _currentVoxels[voxelPositionInChunk.X, voxelPositionInChunk.Y, voxelPositionInChunk.Z];
		}

		private void CreateOuterChunkFaces(Vector3i chunkPosition)
		{
			//TODO Create outerChunkFaces
		}

		private void ResetMembers()
		{
			_outerChunkFaces = null;
			_currentVoxels = _currentVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
		}
	}
}
