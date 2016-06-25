using GraphicsHelper.GraphicsUtils;
using OpenTK;

namespace CubicEngine.View
{
	internal class Cube : Mesh
	{
		public Cube(float size)
		{
			float s2 = size * 0.5f;

			#region top face
			Positions.Add(new Vector3(-s2, s2, s2)); //0
			Positions.Add(new Vector3(s2, s2, s2)); //1
			Positions.Add(new Vector3(s2, s2, -s2)); //2
			Positions.Add(new Vector3(-s2, s2, -s2)); //3

			Normals.Add(Vector3.UnitY);
			Normals.Add(Vector3.UnitY);
			Normals.Add(Vector3.UnitY);
			Normals.Add(Vector3.UnitY);

			Ids.Add(0);
			Ids.Add(1);
			Ids.Add(3);
			Ids.Add(1);
			Ids.Add(2);
			Ids.Add(3);
			#endregion

			#region bottom face
			Positions.Add(new Vector3(-s2, -s2, s2)); //4
			Positions.Add(new Vector3(s2, -s2, s2)); //5
			Positions.Add(new Vector3(s2, -s2, -s2)); //6
			Positions.Add(new Vector3(-s2, -s2, -s2)); //7

			Normals.Add(-Vector3.UnitY);
			Normals.Add(-Vector3.UnitY);
			Normals.Add(-Vector3.UnitY);
			Normals.Add(-Vector3.UnitY);

			Ids.Add(7);
			Ids.Add(6);
			Ids.Add(5);
			Ids.Add(4);
			Ids.Add(7);
			Ids.Add(5);
			#endregion

			#region front face
			Positions.Add(new Vector3(-s2, s2, s2)); //8
			Positions.Add(new Vector3(s2, s2, s2)); //9
			Positions.Add(new Vector3(-s2, -s2, s2)); //10
			Positions.Add(new Vector3(s2, -s2, s2)); //11

			Normals.Add(Vector3.UnitZ);
			Normals.Add(Vector3.UnitZ);
			Normals.Add(Vector3.UnitZ);
			Normals.Add(Vector3.UnitZ);

			Ids.Add(8);
			Ids.Add(10);
			Ids.Add(9);
			Ids.Add(10);
			Ids.Add(11);
			Ids.Add(9);
			#endregion

			#region back face
			Positions.Add(new Vector3(s2, s2, -s2)); //12
			Positions.Add(new Vector3(-s2, s2, -s2)); //13
			Positions.Add(new Vector3(s2, -s2, -s2)); //14
			Positions.Add(new Vector3(-s2, -s2, -s2)); //15

			Normals.Add(-Vector3.UnitZ);
			Normals.Add(-Vector3.UnitZ);
			Normals.Add(-Vector3.UnitZ);
			Normals.Add(-Vector3.UnitZ);

			Ids.Add(13);
			Ids.Add(12);
			Ids.Add(15);
			Ids.Add(12);
			Ids.Add(14);
			Ids.Add(15);
			#endregion

			#region left face
			Positions.Add(new Vector3(-s2, s2, s2)); //16
			Positions.Add(new Vector3(-s2, s2, -s2)); //17
			Positions.Add(new Vector3(-s2, -s2, s2)); //18
			Positions.Add(new Vector3(-s2, -s2, -s2)); //19

			Normals.Add(-Vector3.UnitX);
			Normals.Add(-Vector3.UnitX);
			Normals.Add(-Vector3.UnitX);
			Normals.Add(-Vector3.UnitX);

			Ids.Add(17);
			Ids.Add(19);
			Ids.Add(16);
			Ids.Add(19);
			Ids.Add(18);
			Ids.Add(16);
			#endregion

			#region right face
			Positions.Add(new Vector3(s2, s2, s2)); //20
			Positions.Add(new Vector3(s2, s2, -s2)); //21
			Positions.Add(new Vector3(s2, -s2, s2)); //22
			Positions.Add(new Vector3(s2, -s2, -s2)); //23

			Normals.Add(Vector3.UnitX);
			Normals.Add(Vector3.UnitX);
			Normals.Add(Vector3.UnitX);
			Normals.Add(Vector3.UnitX);

			Ids.Add(20);
			Ids.Add(22);
			Ids.Add(23);
			Ids.Add(21);
			Ids.Add(20);
			Ids.Add(23);
			#endregion
		}
	}
}
