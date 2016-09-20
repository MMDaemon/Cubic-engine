using OpenTK;

namespace CubicEngine.Utils
{
	internal struct Vector3I
	{
		public int X;
		public int Y;
		public int Z;

		public Vector3I(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static Vector3I operator +(Vector3I vec1, Vector3I vec2)
		{
			return new Vector3I(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		}

		public static Vector3I operator -(Vector3I vec1, Vector3I vec2)
		{
			return new Vector3I(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		}

		public static Vector3I operator -(Vector3I vec)
		{
			return new Vector3I(-vec.X, -vec.Y, -vec.Z);
		}

		public static bool operator <(Vector3I vec1, Vector3I vec2)
		{
			return (vec1.X < vec2.X || vec1.Y < vec2.Y || vec1.Z < vec2.Z);
		}

		public static bool operator <=(Vector3I vec1, Vector3I vec2)
		{
			return (vec1.X <= vec2.X || vec1.Y <= vec2.Y || vec1.Z <= vec2.Z);
		}

		public static bool operator >(Vector3I vec1, Vector3I vec2)
		{
			return (vec1.X > vec2.X || vec1.Y > vec2.Y || vec1.Z > vec2.Z);
		}

		public static bool operator >=(Vector3I vec1, Vector3I vec2)
		{
			return (vec1.X >= vec2.X || vec1.Y >= vec2.Y || vec1.Z >= vec2.Z);
		}

		public static Vector3I operator *(Vector3I vec1, Vector3I vec2)
		{
			return new Vector3I(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
		}

		public static Vector3I operator /(Vector3I vec1, Vector3I vec2)
		{
			return new Vector3I(vec1.X / vec2.X, vec1.Y / vec2.Y, vec1.Z / vec2.Z);
		}

		public static Vector3I operator %(Vector3I vec1, Vector3I vec2)
		{
			return new Vector3I(vec1.X % vec2.X, vec1.Y % vec2.Y, vec1.Z % vec2.Z);
		}

		public static explicit operator Vector3(Vector3I vec)
		{
			return new Vector3(vec.X, vec.Y, vec.Z);
		}

		public override string ToString()
		{
			return string.Format("({0},{1},{2})",X,Y,Z);
		}
	}
}
