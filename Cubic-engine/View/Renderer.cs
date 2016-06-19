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

			_cube = CreateVertexArrayObject(new Cube(1));

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

		private VertexArrayObject CreateVertexArrayObject(Mesh mesh)
		{
			var vao = new VertexArrayObject();
			vao.SetAttribute(_shader.GetAttributeLocation("position"), mesh.Positions.ToArray(), VertexAttribPointerType.Float, 3);
			vao.SetAttribute(_shader.GetAttributeLocation("normal"), mesh.Normals.ToArray(), VertexAttribPointerType.Float, 3);
			vao.SetId(mesh.Ids.ToArray(), PrimitiveType.Triangles);
			return vao;
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
	}
}
