using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using System;

namespace CubicEngine.Model
{
	class World
	{
		public const int size = 150;
		Voxel[,,] voxels = new Voxel[size, size, size];
		Random random = new Random();

		public World()
		{
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					for (int z = 0; z < size; z++)
					{
						voxels[x, y, z] = new Voxel();
						voxels[x, y, z].Materials.Add(MaterialType.DIRT, random.Next(Constants.MAX_AMOUNT+1));
					}
				}
			}
		}

		public Voxel this[int x, int y, int z]
		{
			get
			{
				return voxels[x, y, z];
			}
		}
	}
}
