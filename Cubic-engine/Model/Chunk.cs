using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		readonly Voxel[,,] _voxels;

		public Chunk(int xPos, uint yPos, int zPos)
		{
			X = xPos;
			Y = yPos;
			Z = zPos;

			_voxels = new Voxel[(int)Constants.ChunkSize.X, (int)Constants.ChunkSize.Y, (int)Constants.ChunkSize.Z];

			GetVoxels();
			if (!Full)
			{
				SetInnerSurfaceVoxels();
			}
			SetOuterSurvaceVoxels();
		}

		private void GetVoxels()
		{
			Vector3 position = new Vector3((float)X * Constants.ChunkSize.X, (float)Y * Constants.ChunkSize.Y, (float)Z * Constants.ChunkSize.Z);
			int voxelCount = 0;
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_voxels[x, y, z] = new Voxel();
						Vector3 actualPos = position + new Vector3(x, y, z);
						double worldHeight = 5 + 25 * (1 + Math.Sin(actualPos.X / 50)) + 25 * (1 + Math.Sin(actualPos.Z / 100)) + 25 * (1 + Math.Sin(actualPos.X / 20)) +
											 25 * (1 + Math.Sin(actualPos.Z / 10));
						if (actualPos.Y < worldHeight)
						{
							_voxels[x, y, z].Materials.Add(MaterialType.Dirt, Constants.MaxAmount);
							voxelCount++;
						}
					}
				}
			}
			if (voxelCount == 0)
			{
				Empty = true;
			}
			else if (voxelCount == (int)Constants.ChunkSize.X * (int)Constants.ChunkSize.Y * (int)Constants.ChunkSize.Z)
			{
				Full = true;
			}
		}

		private void SetInnerSurfaceVoxels()
		{
			for (int x = 1; x < Constants.ChunkSize.X - 1; x++)
			{
				for (int y = 1; y < Constants.ChunkSize.Y - 1; y++)
				{
					for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
					{
						if (_voxels[x, y, z].Materials.Amount > Constants.MaxAmount / 2)
						{
							bool surrounded = !_voxels[x + 1, y, z].Materials.IsEmpty();
							if (surrounded)
							{
								surrounded = !_voxels[x - 1, y, z].Materials.IsEmpty();
							}
							if (surrounded)
							{
								surrounded = !_voxels[x, y + 1, z].Materials.IsEmpty();
							}
							if (surrounded)
							{
								surrounded = !_voxels[x, y - 1, z].Materials.IsEmpty();
							}
							if (surrounded)
							{
								surrounded = !_voxels[x, y, z + 1].Materials.IsEmpty();
							}
							if (surrounded)
							{
								surrounded = !_voxels[x, y, z - 1].Materials.IsEmpty();
							}
							if (!surrounded)
							{
								_voxels[x, y, z].Surface = true;
							}
						}
					}
				}
			}
		}

		private void SetOuterSurvaceVoxels()
		{
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					if (!_voxels[x, y, 0].Materials.IsEmpty())
					{
						_voxels[x, y, 0].Surface = true;
					}
					if (!_voxels[x, y, (int)Constants.ChunkSize.Z - 1].Materials.IsEmpty())
					{
						_voxels[x, y, (int)Constants.ChunkSize.Z - 1].Surface = true;
					}
				}
			}
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
				{
					if (!_voxels[x, 0, z].Materials.IsEmpty())
					{
						_voxels[x, 0, z].Surface = true;
					}
					if (!_voxels[x, (int)Constants.ChunkSize.Y - 1, z].Materials.IsEmpty())
					{
						_voxels[x, (int)Constants.ChunkSize.Y - 1, z].Surface = true;
					}
				}
			}

			for (int y = 1; y < Constants.ChunkSize.Y - 1; y++)
			{
				for (int z = 1; z < Constants.ChunkSize.Z - 1; z++)
				{
					if (!_voxels[0, y, z].Materials.IsEmpty())
					{
						_voxels[0, y, z].Surface = true;
					}
					if (!_voxels[(int)Constants.ChunkSize.X - 1, y, z].Materials.IsEmpty())
					{
						_voxels[(int)Constants.ChunkSize.X - 1, y, z].Surface = true;
					}
				}
			}
		}

		public int X { get; private set; }
		public uint Y { get; private set; }
		public int Z { get; private set; }
		public bool Empty { get; private set; } = false;
		public bool Full { get; private set; } = false;
		public bool Surrounded { get; set; } = false;
		public Voxel this[int x, int y, int z] => _voxels[x, y, z];
	}
}
