using CubicEngine.Model;
using CubicEngine.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CubicEngine.View
{
	class Renderer
	{
		public Renderer()
		{
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Less);
		}

		public void ResizeWindow(int width, int height)
		{
			Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / height, 0.1f, 500);
			//Set perspective
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.LoadMatrix(ref perspective);
			GL.Viewport(0, 0, width, height);
		}

		public void Render(World world)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Clear(ClearBufferMask.DepthBufferBit);

			Matrix4 lookAt = Matrix4.LookAt(new Vector3(200, 200, 150), Vector3.Zero, Vector3.UnitY);
			//Set Camera
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.LoadMatrix(ref lookAt);

			//Draw the scene
			//DrawCube(1, new Vector3(0, 0, 0), new Vector4(1, 1, 1, 0));
			DrawWorld(world);
		}

		private void DrawWorld(World world)
		{
			for(int x =0; x < World.size; x++)
			{
				for (int y = 0; y < World.size; y++)
				{
					for (int z = 0; z < World.size; z++)
					{
						if (world[x, y, z].Materials.Amount >= Constants.MAX_AMOUNT)
						{
							DrawCube(1, new Vector3(x, y, z), new Vector4(1, 1, 1, 0));
						}
					}
				}
			}
		}

		private void DrawCube(float size, Vector3 position, Vector4 color)
		{
			float halfEdge = size / 2;
			//Top edges
			Vector3 edge0 = new Vector3(position.X - halfEdge, position.Y + halfEdge, position.Z + halfEdge);
			Vector3 edge1 = new Vector3(position.X + halfEdge, position.Y + halfEdge, position.Z + halfEdge);
			Vector3 edge2 = new Vector3(position.X + halfEdge, position.Y + halfEdge, position.Z - halfEdge);
			Vector3 edge3 = new Vector3(position.X - halfEdge, position.Y + halfEdge, position.Z - halfEdge);
			//Bottom edges
			Vector3 edge4 = new Vector3(position.X - halfEdge, position.Y - halfEdge, position.Z + halfEdge);
			Vector3 edge5 = new Vector3(position.X + halfEdge, position.Y - halfEdge, position.Z + halfEdge);
			Vector3 edge6 = new Vector3(position.X + halfEdge, position.Y - halfEdge, position.Z - halfEdge);
			Vector3 edge7 = new Vector3(position.X - halfEdge, position.Y - halfEdge, position.Z - halfEdge);

			GL.PushMatrix();

			GL.Color4(color);
			GL.Begin(PrimitiveType.Quads);

			//Top face
			GL.Vertex3(edge0);
			GL.Vertex3(edge1);
			GL.Vertex3(edge2);
			GL.Vertex3(edge3);

			//Bottom face
			GL.Vertex3(edge7);
			GL.Vertex3(edge6);
			GL.Vertex3(edge5);
			GL.Vertex3(edge4);

			//Front face
			GL.Vertex3(edge0);
			GL.Vertex3(edge4);
			GL.Vertex3(edge5);
			GL.Vertex3(edge1);

			//Back face
			GL.Vertex3(edge2);
			GL.Vertex3(edge6);
			GL.Vertex3(edge7);
			GL.Vertex3(edge3);

			//Right face
			GL.Vertex3(edge1);
			GL.Vertex3(edge5);
			GL.Vertex3(edge6);
			GL.Vertex3(edge2);

			//Left face
			GL.Vertex3(edge3);
			GL.Vertex3(edge7);
			GL.Vertex3(edge4);
			GL.Vertex3(edge0);

			GL.End();

			GL.PopMatrix();
		}
	}
}
