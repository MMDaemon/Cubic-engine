using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CubicEngine.Utils
{
	struct Vector3i
	{
		public int X;
		public int Y;
		public int Z;

		public Vector3i(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static Vector3i operator +(Vector3i vec1, Vector3i vec2)
		{
			return new Vector3i(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		}

		public static Vector3i operator -(Vector3i vec1, Vector3i vec2)
		{
			return new Vector3i(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		}

		public static Vector3i operator -(Vector3i vec)
		{
			return new Vector3i(-vec.X, -vec.Y, -vec.Z);
		}

		public static explicit operator Vector3(Vector3i vec)
		{
			return new Vector3(vec.X, vec.Y, vec.Z);
		}
	}
}
