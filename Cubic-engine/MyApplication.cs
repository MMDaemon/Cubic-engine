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

		private int x = -8;
		private uint y = 0;
		private int z = -8;

		private MyApplication()
		{
			_gameWindow = new GameWindow();
			_model = new EngineModel();
			_renderer = new Renderer();

			_gameWindow.MouseMove += GameWindow_MouseMove;
			_gameWindow.MouseWheel += GameWindow_MouseWheel;
			_gameWindow.Resize += GameWindow_Resize;
			_gameWindow.RenderFrame += GameWindow_RenderFrame;
			_gameWindow.RenderFrame += (sender, e) => { _gameWindow.SwapBuffers(); };
			_gameWindow.UpdateFrame += _gameWindow_UpdateFrame;
		}

		private void _gameWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			AddChunks(4);
		}

		private void AddChunks(int amount)
		{
			int count = 0;
			while (x < 8)
			{
				while (y < 8)
				{
					while (z < 8)
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
					z = -8;
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

		private void GameWindow_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			_renderer.Camera.Distance -= 10 * e.DeltaPrecise;
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
