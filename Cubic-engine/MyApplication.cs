using System;
using System.Security.Cryptography.X509Certificates;
using CubicEngine.Model;
using CubicEngine.View;
using OpenTK;
using OpenTK.Graphics.ES20;
using OpenTK.Input;

namespace CubicEngine
{
	internal class MyApplication
	{
		private readonly GameWindow _gameWindow;
		private readonly EngineModel _model;
		private readonly Renderer _renderer;

		private int x = -20;
		private uint y = 0;
		private int z = -20;

		private MyApplication()
		{
			_gameWindow = new GameWindow();
			_model = new EngineModel();
			_renderer = new Renderer();

			_gameWindow.MouseMove += GameWindow_MouseMove;
			_gameWindow.Resize += GameWindow_Resize;
			_gameWindow.RenderFrame += GameWindow_RenderFrame;
			_gameWindow.RenderFrame += (sender, e) => { _gameWindow.SwapBuffers(); };
			_gameWindow.UpdateFrame += _gameWindow_UpdateFrame;
			_gameWindow.KeyDown += _gameWindow_KeyDown;
		}

		private void _gameWindow_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			Matrix4 rotationY = Matrix4.CreateRotationY(_renderer.Camera.Heading);
			switch (e.Key)
			{
				case Key.W:
				case Key.Up:
					_renderer.Camera.Position += new Vector3(rotationY.M11, rotationY.M21, rotationY.M31);
					break;
				case Key.S:
				case Key.Down:
					_renderer.Camera.Position -= new Vector3(rotationY.M11, rotationY.M21, rotationY.M31);
					break;
				case Key.A:
				case Key.Left:
					Matrix4 rotationLeft = Matrix4.CreateRotationY(_renderer.Camera.Heading - (float)Math.PI/2);
					_renderer.Camera.Position += new Vector3(rotationLeft.M11, rotationLeft.M21, rotationLeft.M31);
					break;
				case Key.D:
				case Key.Right:
					Matrix4 rotationRight = Matrix4.CreateRotationY(_renderer.Camera.Heading + (float)Math.PI / 2);
					_renderer.Camera.Position += new Vector3(rotationRight.M11, rotationRight.M21, rotationRight.M31);
					break;
				case Key.Space:
					_renderer.Camera.Position += Vector3.UnitY;
					break;
				case Key.ShiftLeft:
					_renderer.Camera.Position -= Vector3.UnitY;
					break;
				default:
					break;

			}
		}

		private void _gameWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			AddChunks(4);
		}

		private void AddChunks(int amount)
		{
			int count = 0;
			while (x < 20)
			{
				while (y < 8)
				{
					while (z < 20)
					{
						_renderer.AddChunk(new Chunk(x, y, z));
						count++;
						z++;
						if (count >= amount)
						{
							return;
						}
					}
					y++;
					z = -20;
				}
				x++;
				y = 0;

			}
		}

		public static void Main(string[] args)
		{
			var app = new MyApplication();
			app._gameWindow.Run(60, 60);
		}

		private void GameWindow_Resize(object sender, System.EventArgs e)
		{
			_renderer.ResizeWindow(_gameWindow.Width, _gameWindow.Height);
		}

		private void GameWindow_RenderFrame(object sender, FrameEventArgs e)
		{
			_renderer.Render();
		}

		private void GameWindow_MouseMove(object sender, MouseMoveEventArgs e)
		{
			if (ButtonState.Pressed == e.Mouse.LeftButton)
			{
				_renderer.Camera.Heading += 10 * e.XDelta / (float)_gameWindow.Width;
				_renderer.Camera.Tilt += 10 * e.YDelta / (float)_gameWindow.Height;
			}
		}
	}
}
