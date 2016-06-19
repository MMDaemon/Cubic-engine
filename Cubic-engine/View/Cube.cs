using GraphicsHelper.GraphicsUtils;
using OpenTK;

namespace CubicEngine.View
{
	internal class Cube : Mesh
	{
		public Cube(float size)
		{
			float s2 = size * 0.5f;

			//corners
			Positions.Add(new Vector3(-s2, s2, s2)); //0
			Positions.Add(new Vector3(s2, s2, s2)); //1
			Positions.Add(new Vector3(s2, s2, -s2)); //2
			Positions.Add(new Vector3(-s2, s2, -s2)); //3
			Positions.Add(new Vector3(-s2, -s2, s2)); //4
			Positions.Add(new Vector3(s2, -s2, s2)); //5
			Positions.Add(new Vector3(s2, -s2, -s2)); //6
			Positions.Add(new Vector3(-s2, -s2, -s2)); //7

			//normals
			Normals.Add(-Vector3.UnitX);
			Normals.Add(Vector3.UnitZ);
			Normals.Add(Vector3.Normalize(Vector3.One));
			Normals.Add(Vector3.UnitY);
			Normals.Add(Vector3.Normalize(Vector3.One));
			Normals.Add(-Vector3.UnitY);
			Normals.Add(Vector3.UnitX);
			Normals.Add(-Vector3.UnitZ);

			//Top Face
			Ids.Add(0);
			Ids.Add(1);
			Ids.Add(3);
			Ids.Add(1);
			Ids.Add(2);
			Ids.Add(3);
			//Bottom Face
			Ids.Add(7);
			Ids.Add(6);
			Ids.Add(5);
			Ids.Add(4);
			Ids.Add(7);
			Ids.Add(5);
			//Front Face
			Ids.Add(0);
			Ids.Add(4);
			Ids.Add(1);
			Ids.Add(4);
			Ids.Add(5);
			Ids.Add(1);
			//Back Face
			Ids.Add(3);
			Ids.Add(2);
			Ids.Add(7);
			Ids.Add(2);
			Ids.Add(6);
			Ids.Add(7);
			//Left face
			Ids.Add(3);
			Ids.Add(7);
			Ids.Add(0);
			Ids.Add(7);
			Ids.Add(4);
			Ids.Add(0);
			//Right face
			Ids.Add(1);
			Ids.Add(5);
			Ids.Add(6);
			Ids.Add(2);
			Ids.Add(1);
			Ids.Add(6);
		}
	}
}
