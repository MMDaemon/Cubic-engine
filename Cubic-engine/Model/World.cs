using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	internal class World
	{
		public static readonly Vector3 Size = new Vector3(150, 150, 150);
		readonly Voxel[,,] _voxels;
		readonly Random _random = new Random();

		public World()
		{
			_voxels = new Voxel[(int)Size.X, (int)Size.Y, (int)Size.Z];
			for (int x = 0; x < Size.X; x++)
			{
				for (int y = 0; y < Size.Y; y++)
				{
					for (int z = 0; z < Size.Z; z++)
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
