using CubicEngine.Utils;
using CubicEngine.Utils.Enums;
using OpenTK;
using System;

namespace CubicEngine.Model
{
	internal class Chunk
	{
		readonly Voxel[,,] _voxels;

		public Vector3i Position { get; private set; }
		public ChunkStatus Status { get; set; } = ChunkStatus.None;

		public Voxel this[int x, int y, int z] => _voxels[x, y, z];

		public Chunk(Vector3i position, Voxel[,,] voxels)
		{
			Position = position;
			_voxels = voxels;
		}
	}
}
