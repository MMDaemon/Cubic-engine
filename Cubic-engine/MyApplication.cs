using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubic_engine
{
	class MyApplication
	{
		private GameWindow gameWindow = new GameWindow();

		static void Main(string[] args)
		{
			var app = new MyApplication();
			app.Run();
		}

		private MyApplication()
		{
			gameWindow.RenderFrame += game_RenderFrame;
		}

		private void Run()
		{
			gameWindow.Run(60);
		}

		private void game_RenderFrame(object sender, FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			gameWindow.SwapBuffers();
		}
	}
}
