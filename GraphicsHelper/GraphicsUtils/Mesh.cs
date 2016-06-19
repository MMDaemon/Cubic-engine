using System.Collections.Generic;
using OpenTK;

namespace GraphicsHelper.GraphicsUtils
{
	public class Mesh
	{
		public List<Vector3> Positions { get; } = new List<Vector3>();
		public List<Vector3> Normals { get; } = new List<Vector3>();
		public List<Vector2> Uvs { get; } = new List<Vector2>();
		public List<uint> Ids { get; } = new List<uint>();

		public Mesh SwitchHandedness()
		{
			var mesh = new Mesh();
			foreach (var pos in Positions)
			{
				var newPos = pos;
				newPos.Z = -newPos.Z;
				mesh.Positions.Add(newPos);
			}
			foreach (var n in Normals)
			{
				var newN = n;
				newN.Z = -newN.Z;
				mesh.Normals.Add(newN);
			}
			mesh.Uvs.AddRange(Uvs);
			mesh.Ids.AddRange(Ids);
			return mesh;
		}
		public Mesh SwitchTriangleMeshWinding()
		{
			var mesh = new Mesh();
			mesh.Positions.AddRange(Positions);
			mesh.Normals.AddRange(Normals);
			mesh.Uvs.AddRange(Uvs);
			for (int i = 0; i < Ids.Count; i += 3)
			{
				mesh.Ids.Add(Ids[i]);
				mesh.Ids.Add(Ids[i + 2]);
				mesh.Ids.Add(Ids[i + 1]);
			}
			return mesh;
		}
	};
}
