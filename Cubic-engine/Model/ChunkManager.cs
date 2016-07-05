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
			ChunkStatus status = ChunkStatus.None;
			int fillAmount = 0;

			fillAmount = CreateVoxels(chunkPosition);

			if (fillAmount == 0)
			{
				status = ChunkStatus.Empty;
			}
			else if (fillAmount == (int)Constants.ChunkSize.X * (int)Constants.ChunkSize.Y * (int)Constants.ChunkSize.Z * Constants.MaxAmount)
			{
				status = ChunkStatus.Full;
			}

			Chunk chunk = new Chunk(chunkPosition, _currentVoxels) { Status = status };

			ClearMembers();

			return chunk;
		}

		private int CreateVoxels(Vector3i chunkPosition)
		{
			InitializeOuterChunkFaces();

			InitializeCurrentVoxels();

			Vector3i position = new Vector3i(chunkPosition.X * Constants.ChunkSize.X, chunkPosition.Y * Constants.ChunkSize.Y, chunkPosition.Z * Constants.ChunkSize.Z);

			int fillAmount = 0;
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						Vector3i voxelPosition = position + new Vector3i(x, y, z);
						_currentVoxels[x, y, z] = CreateVoxel(voxelPosition);

						fillAmount += _currentVoxels[x, y, z].Materials.Amount;

						if (x > 1 && y > 1 && z > 1 && !_currentVoxels[x - 1, y - 1, z - 1].Materials.IsEmpty())
						{
							CheckSurfaceVoxel(x - 1, y - 1, z - 1);
						}
					}
				}
			}
			SetOuterSurvaceVoxels();

			return fillAmount;
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
			return _currentVoxels[voxelPositionInChunk.X, voxelPositionInChunk.Y, voxelPositionInChunk.Z];
		}

		private void CreateOuterChunkFaces(Vector3i chunkPosition)
		{
			//TODO Create outerChunkFaces
		}

		private void InitializeOuterChunkFaces()
		{
			_outerChunkFaces = new Voxel[6][,];

			_outerChunkFaces[(int)Direction.Top] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Z];
			_outerChunkFaces[(int)Direction.Bottom] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Z];

			_outerChunkFaces[(int)Direction.Left] = new Voxel[Constants.ChunkSize.Y, Constants.ChunkSize.Z];
			_outerChunkFaces[(int)Direction.Right] = new Voxel[Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			_outerChunkFaces[(int)Direction.Front] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y];
			_outerChunkFaces[(int)Direction.Back] = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y];
		}

		private void InitializeCurrentVoxels()
		{
			_currentVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
		}

		private void ClearMembers()
		{
			_outerChunkFaces = null;
			_currentVoxels = null;
		}
	}
}
