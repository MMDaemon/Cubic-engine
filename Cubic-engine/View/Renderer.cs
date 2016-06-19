using System;
using System.Collections.Generic;
using System.Text;
using CubicEngine.Model;
using CubicEngine.Resources;
using CubicEngine.Utils;
using GraphicsHelper.GraphicsUtils;
using GraphicsHelper.ShaderUtils;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace CubicEngine.View
{
	internal class Renderer
	{
		private readonly Shader _shader;
		private readonly VertexArrayObject _cube;
		private int _particleCount = 0;

		public CameraOrbit Camera { get; } = new CameraOrbit();

		public Renderer(World world)
		{

			Camera.FarClip = 500;
			Camera.Distance = 30;

			var sVertex = Encoding.UTF8.GetString(Shaders.vertex);
			var sFragment = Encoding.UTF8.GetString(Shaders.fragment);
			_shader = ShaderLoader.FromStrings(sVertex, sFragment);

			_cube = CreateVertexArrayObject();

			//CreateCube(_cube, _shader);
			CreateCubeInstances(world, _cube, _shader);

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

		}

		public void ResizeWindow(int width, int height)
		{

			GL.Viewport(0, 0, width, height);
		}

		public void Render()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_shader.Begin();

			GL.Uniform3(_shader.GetUniformLocation("lightDirection"), Vector3.Normalize(new Vector3(2, 3, 1)));
			GL.Uniform4(_shader.GetUniformLocation("lightColor"), new Color4(1, 1, 1, 1f));
			GL.Uniform4(_shader.GetUniformLocation("materialColor"), new Color4(0.9f, 0.9f, 0.1f, 1f));

			Matrix4 cam = Camera.CalcMatrix();
			GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref cam);

			_cube.Draw(_particleCount);
			
			_shader.End();
		}

		private void DrawWorld(World world)
		{
			for(int x =0; x < World.Size.X; x++)
			{
				for (int y = 0; y < World.Size.Y; y++)
				{
					for (int z = 0; z < World.Size.Z; z++)
					{
						if (world[x, y, z].Materials.Amount >= Constants.MaxAmount)
						{
							DrawCube(1, new Vector3(x, y, z), new Vector4(1, 1, 1, 0));
						}
					}
				}
			}
		}

		private VertexArrayObject CreateVertexArrayObject()
		{
			Mesh mesh = new Cube(1);
			var vao = new VertexArrayObject();
			vao.SetAttribute(_shader.GetAttributeLocation("position"), mesh.Positions.ToArray(), VertexAttribPointerType.Float, 3);
			vao.SetAttribute(_shader.GetAttributeLocation("normal"), mesh.Normals.ToArray(), VertexAttribPointerType.Float, 3);
			vao.SetId(mesh.Ids.ToArray(), PrimitiveType.Triangles);
			return vao;
		}

		private void CreateCube(VertexArrayObject vao, Shader shader)
		{
			_particleCount++;
			Vector3[] instancePositions = {new Vector3(0,0,0)};
			vao.SetAttribute(shader.GetAttributeLocation("instancePosition"), instancePositions , VertexAttribPointerType.Float, 3, true);
		}

		private void CreateCubeInstances(World world, VertexArrayObject vao, Shader shader)
		{
			List<Vector3> instancePositions = new List<Vector3>();
			for (int x = 0; x < World.Size.X; x++)
			{
				for (int y = 0; y < World.Size.Y; y++)
				{
					for (int z = 0; z < World.Size.Z; z++)
					{
						if (world[x, y, z].Materials.Amount >= Constants.MaxAmount)
						{
							_particleCount++;
							instancePositions.Add(new Vector3(x,y,z));
						}
					}
				}
			}
			vao.SetAttribute(shader.GetAttributeLocation("instancePosition"), instancePositions.ToArray(), VertexAttribPointerType.Float, 3, true);
		}

		private static void DrawCube(float size, Vector3 position, Vector4 color)
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
