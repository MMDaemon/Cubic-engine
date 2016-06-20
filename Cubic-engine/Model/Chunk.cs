using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		readonly Voxel[,,] _voxels;

		public Chunk()
		{
			Vector3 center = Constants.ChunkSize/2;
			float radius = center.X;

			_voxels = new Voxel[(int)Constants.ChunkSize.X, (int)Constants.ChunkSize.Y, (int)Constants.ChunkSize.Z];
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						_voxels[x, y, z] = new Voxel();
						if ((new Vector3(x, y, z) - center).Length < radius)
						{
							_voxels[x, y, z].Materials.Add(MaterialType.Dirt, Constants.MaxAmount);
						}
					}
				}
			}
		}

		public Voxel this[int x, int y, int z] => _voxels[x, y, z];
	}
}
