using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	class World
	{
		public static readonly Vector3 SIZE = new Vector3(150, 150, 150);
		Voxel[,,] voxels;
		Random random = new Random();

		public World()
		{
			voxels = new Voxel[(int)SIZE.X, (int)SIZE.Y, (int)SIZE.Z];
			for (int x = 0; x < SIZE.X; x++)
			{
				for (int y = 0; y < SIZE.Y; y++)
				{
					for (int z = 0; z < SIZE.Z; z++)
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
