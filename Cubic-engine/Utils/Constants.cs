namespace CubicEngine.Utils
{
	internal static class Constants
	{
		public const int MaxAmount = 64;

		public static readonly Vector3I ChunkSize = new Vector3I(32, 32, 32);

		public static readonly Vector3I[] DirectionVectors = new Vector3I[]
		{
			new Vector3I(1, 0, 0),
			new Vector3I(-1, 0, 0),
			new Vector3I(0, 1, 0),
			new Vector3I(0, -1, 0),
			new Vector3I(0, 0, 1),
			new Vector3I(0, 0, -1)
		};

		public static readonly Vector3I[] DiagonalDirectionVectors = new Vector3I[]
		{
			new Vector3I(1, 1, 0),
			new Vector3I(1, -1, 0),
			new Vector3I(1, 0, 1),
			new Vector3I(1, 0, -1),
			new Vector3I(-1, 1, 0),
			new Vector3I(-1, -1, 0),
			new Vector3I(-1, 0, 1),
			new Vector3I(-1, 0, -1),
			new Vector3I(0, 1, 1),
			new Vector3I(0, 1, -1),
			new Vector3I(0, -1, 1),
			new Vector3I(0, -1, -1),

			new Vector3I(1, 1, 1),
			new Vector3I(1, -1, 1),
			new Vector3I(1, 1, -1),
			new Vector3I(1, -1, -1),
			new Vector3I(-1, 1, 1),
			new Vector3I(-1, -1, 1),
			new Vector3I(-1, 1, -1),
			new Vector3I(-1, -1, -1)
		};
	}
}
