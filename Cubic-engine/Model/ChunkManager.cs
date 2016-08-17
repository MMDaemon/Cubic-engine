using System;
using CubicEngine.Utils;
using CubicEngine.Utils.Enums;

namespace CubicEngine.Model
{
	class ChunkManager
	{
		private Voxel[,,] _currentVoxels;

		public ChunkManager()
		{
			_currentVoxels = _currentVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
		}

		private static Voxel[,,] CreateVoxels(Vector3I chunkPosition)
		{
			Vector3I position = new Vector3I(chunkPosition.X * Constants.ChunkSize.X, chunkPosition.Y * Constants.ChunkSize.Y, chunkPosition.Z * Constants.ChunkSize.Z);

			Voxel[,,] voxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						if (x < Constants.ChunkSize.X && y < Constants.ChunkSize.Y && z < Constants.ChunkSize.Z)
						{
							Vector3I voxelPosition = position + new Vector3I(x, y, z);
							voxels[x, y, z] = CreateVoxel(voxelPosition);
						}
					}
				}
			}
			return voxels;
		}

		private static Voxel CreateVoxel(Vector3I voxelPosition, bool edge = false)
		{
			var voxel = edge ? new EdgeVoxel() : new Voxel();

			double worldHeight = 5 + 15 * (1 + Math.Cos((float)voxelPosition.X / 40)) + 35 * (1 + Math.Cos((float)voxelPosition.Z / 90)) + 15 * (1 + Math.Cos((float)voxelPosition.X / 30)) +
											 35 * (1 + Math.Cos((float)voxelPosition.Z / 100));
			if (voxelPosition.Y < worldHeight)
			{
				int matHeight = (int)(Constants.MaxAmount * (voxelPosition.Y - (45 + 5 * (1 + Math.Cos((float)voxelPosition.X / 5)) + 15 * (1 + Math.Sin((float)voxelPosition.Z / 50)) + 5 * (1 + Math.Cos((float)voxelPosition.X / 8)) +
											 15 * (1 + Math.Sin((float)voxelPosition.Z / 70)))));

				int stoneAmount = Constants.MaxAmount < matHeight ? Constants.MaxAmount : (matHeight < 0 ? 0 : matHeight);
				voxel.Materials.Add("Stone", stoneAmount);
				voxel.Materials.Add("Dirt", Constants.MaxAmount - stoneAmount);
			}

			return voxel;
		}

		public Chunk GetChunk(int x, int y, int z)
		{
			return GetChunk(new Vector3I(x, y, z));
		}

		public Chunk GetChunk(Vector3I position)
		{
			return LoadChunk(position) ?? CreateChunk(position);
		}

		private Chunk LoadChunk(Vector3I chunkPosition)
		{
			//TODO implement Chunk loading
			return null;
		}

		private Chunk CreateChunk(Vector3I chunkPosition)
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

		private ChunkStatus CreateOuterFaces(Vector3I chunkPosition)
		{
			ChunkStatus status = ChunkStatus.Surrounded;

			#region left & right

			Vector3I currentPosition1 = chunkPosition + new Vector3I(-1, 0, 0);
			Chunk chunk1 = LoadChunk(currentPosition1);
			Voxel[,,] voxels1 = chunk1 != null ? chunk1.Voxels : CreateVoxels(currentPosition1);

			Vector3I currentPosition2 = chunkPosition + new Vector3I(1, 0, 0);
			Chunk chunk2 = LoadChunk(currentPosition2);
			Voxel[,,] voxels2 = chunk2 != null ? chunk2.Voxels : CreateVoxels(currentPosition2);

			for (int y = 0; y < Constants.ChunkSize.Y; y++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					((EdgeVoxel)_currentVoxels[0, y, z]).XVoxel = voxels1[Constants.ChunkSize.X - 1, y, z];
					((EdgeVoxel)_currentVoxels[Constants.ChunkSize.X - 1, y, z]).XVoxel = voxels2[0, y, z];

					if (((EdgeVoxel)_currentVoxels[0, y, z]).XVoxel.Materials.IsEmpty() || ((EdgeVoxel)_currentVoxels[Constants.ChunkSize.X - 1, y, z]).XVoxel.Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region top & bottom

			currentPosition1 = chunkPosition + new Vector3I(0, 1, 0);
			chunk1 = LoadChunk(currentPosition1);
			voxels1 = chunk1 != null ? chunk1.Voxels : CreateVoxels(currentPosition1);

			currentPosition2 = chunkPosition + new Vector3I(0, -1, 0);
			chunk2 = LoadChunk(currentPosition2);
			voxels2 = chunk2 != null ? chunk2.Voxels : CreateVoxels(currentPosition2);

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					((EdgeVoxel)_currentVoxels[x, Constants.ChunkSize.Y - 1, z]).YVoxel = voxels1[x, 0, z];
					((EdgeVoxel)_currentVoxels[x, 0, z]).YVoxel = voxels2[x, Constants.ChunkSize.Y - 1, z];

					if (((EdgeVoxel)_currentVoxels[x, Constants.ChunkSize.Y - 1, z]).YVoxel.Materials.IsEmpty() || ((EdgeVoxel)_currentVoxels[x, 0, z]).YVoxel.Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			#region front & back

			currentPosition1 = chunkPosition + new Vector3I(0, 0, 1);
			chunk1 = LoadChunk(currentPosition1);
			voxels1 = chunk1 != null ? chunk1.Voxels : CreateVoxels(currentPosition1);

			currentPosition2 = chunkPosition + new Vector3I(0, 0, -1);
			chunk2 = LoadChunk(currentPosition2);
			voxels2 = chunk2 != null ? chunk2.Voxels : CreateVoxels(currentPosition2);

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Z; y++)
				{
					((EdgeVoxel)_currentVoxels[x, y, Constants.ChunkSize.Z - 1]).ZVoxel = voxels1[x, y, 0];
					((EdgeVoxel)_currentVoxels[x, y, 0]).ZVoxel = voxels2[x, y, Constants.ChunkSize.Z - 1];

					if (((EdgeVoxel)_currentVoxels[x, y, Constants.ChunkSize.Z - 1]).ZVoxel.Materials.IsEmpty() || ((EdgeVoxel)_currentVoxels[x, y, 0]).ZVoxel.Materials.IsEmpty())
					{
						status = ChunkStatus.None;
					}
				}
			}

			#endregion

			return status;
		}

		private ChunkStatus CreateAndCheckVoxels(Vector3I chunkPosition)
		{
			Vector3I position = new Vector3I(chunkPosition.X * Constants.ChunkSize.X, chunkPosition.Y * Constants.ChunkSize.Y, chunkPosition.Z * Constants.ChunkSize.Z);

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
							Vector3I voxelPosition = position + new Vector3I(x, y, z);
							bool edge = (x == 0 || y == 0 || z == 0 || x == Constants.ChunkSize.X - 1 || y == Constants.ChunkSize.Y - 1 || z == Constants.ChunkSize.Z - 1);
							_currentVoxels[x, y, z] = CreateVoxel(voxelPosition, edge);

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
			Voxel voxel;
			if (x == -1)
			{
				voxel = ((EdgeVoxel)_currentVoxels[0, y, z]).XVoxel;
			}
			else if (x == Constants.ChunkSize.X)
			{
				voxel = ((EdgeVoxel)_currentVoxels[Constants.ChunkSize.X - 1, y, z]).XVoxel;
			}
			else if (y == -1)
			{
				voxel = ((EdgeVoxel)_currentVoxels[x, 0, z]).YVoxel;
			}
			else if (y == Constants.ChunkSize.Y)
			{
				voxel = ((EdgeVoxel)_currentVoxels[x, Constants.ChunkSize.Y - 1, z]).YVoxel;
			}
			else if (z == -1)
			{
				voxel = ((EdgeVoxel)_currentVoxels[x, y, 0]).ZVoxel;
			}
			else if (z == Constants.ChunkSize.Z)
			{
				voxel = ((EdgeVoxel)_currentVoxels[x, y, Constants.ChunkSize.Z - 1]).ZVoxel;
			}
			else
			{
				voxel = _currentVoxels[x, y, z];
			}

			return voxel;
		}

		private void ResetMembers()
		{
			_currentVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
		}
	}
}
