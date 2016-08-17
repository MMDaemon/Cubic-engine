using CubicEngine.Utils;
using CubicEngine.Utils.Enums;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		public Voxel[,,] Voxels { get; private set; }

		public Vector3I Position { get; private set; }
		public ChunkStatus Status { get; set; } = ChunkStatus.None;

		public Voxel this[int x, int y, int z]
		{
			get
			{
				Voxel voxel;
				if (x == -1)
				{
					voxel = ((EdgeVoxel)Voxels[0, y, z]).XVoxel;
				}
				else if (x == Constants.ChunkSize.X)
				{
					voxel = ((EdgeVoxel)Voxels[Constants.ChunkSize.X - 1, y, z]).XVoxel;
				}
				else if (y == -1)
				{
					voxel = ((EdgeVoxel)Voxels[x, 0, z]).YVoxel;
				}
				else if (y == Constants.ChunkSize.Y)
				{
					voxel = ((EdgeVoxel)Voxels[x, Constants.ChunkSize.Y - 1, z]).YVoxel;
				}
				else if (z == -1)
				{
					voxel = ((EdgeVoxel)Voxels[x, y, 0]).ZVoxel;
				}
				else if (z == Constants.ChunkSize.Z)
				{
					voxel = ((EdgeVoxel)Voxels[x, y, Constants.ChunkSize.Z - 1]).ZVoxel;
				}
				else
				{
					voxel = Voxels[x, y, z];
				}
				return voxel;
			}
		}

		public Chunk(Vector3I position, Voxel[,,] voxels)
		{
			Position = position;
			Voxels = voxels;
		}
	}
}
