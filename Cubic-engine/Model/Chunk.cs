using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		public Voxel[,,] Voxels { get; private set; }

		public Vector3i Position { get; private set; }
		public ChunkStatus Status { get; set; } = ChunkStatus.None;

		public Voxel this[int x, int y, int z] => Voxels[x, y, z];

		public Chunk(Vector3i position, Voxel[,,] voxels)
		{
			Position = position;
			Voxels = voxels;
		}
	}
}
