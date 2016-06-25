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
		}

		private void GetVoxels()
		{
			Vector3 position = new Vector3((float)X * Constants.ChunkSize.X, (float)Y * Constants.ChunkSize.Y, (float)Z * Constants.ChunkSize.Z);
			Vector3 center = new Vector3(0, 110, 0);
			const float radius = 100;

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_voxels[x, y, z] = new Voxel();
						if ((position + new Vector3(x, y, z) - center).Length < radius)
						{
							_voxels[x, y, z].Materials.Add(MaterialType.Dirt, Constants.MaxAmount);
						}
					}
				}
			}
		}

		public int X { get; private set; }
		public uint Y { get; private set; }
		public int Z { get; private set; }
		public Voxel this[int x, int y, int z] => _voxels[x, y, z];
	}
}
