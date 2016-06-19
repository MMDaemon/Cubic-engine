using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		readonly Voxel[,,] _voxels;
		readonly Random _random = new Random();

		public Chunk()
		{
			_voxels = new Voxel[(int)Constants.ChunkSize.X, (int)Constants.ChunkSize.Y, (int)Constants.ChunkSize.Z];
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_voxels[x, y, z] = new Voxel();
						_voxels[x, y, z].Materials.Add(MaterialType.Dirt, _random.Next(Constants.MaxAmount+1));
					}
				}
			}
		}

		public Voxel this[int x, int y, int z] => _voxels[x, y, z];
	}
}
