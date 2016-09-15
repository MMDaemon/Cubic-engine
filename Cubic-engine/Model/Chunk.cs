using CubicEngine.Utils;
using CubicEngine.Utils.Enums;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		private readonly World _world;
		public Voxel[,,] Voxels { get; private set; }

		public Vector3I Position { get; private set; }
		public ChunkStatus Status { get; set; } = ChunkStatus.None;

		public Voxel this[Vector3I position]
		{
			get
			{
				Voxel voxel;
				if (position < new Vector3I(0, 0, 0) || position >= Constants.ChunkSize)
				{
					Vector3I absolutePosition = Position * Constants.ChunkSize + position;
					voxel = _world.GetVoxel(absolutePosition);
				}
				else
				{
					voxel = Voxels[position.X, position.Y, position.Z];
				}
				return voxel;
			}
		}

		public Voxel this[int x, int y, int z] => this[new Vector3I(x, y, z)];

		public Chunk(Vector3I position, Voxel[,,] voxels, World world)
		{
			_world = world;
			Position = position;
			Voxels = voxels;
		}
	}
}
