using CubicEngine.Model;
using CubicEngine.View;
using OpenTK;

namespace CubicEngine
{
	class MyApplication
	{
		private GameWindow gameWindow;
		private Renderer renderer;
		private EngineModel model;

		private MyApplication()
		{
			gameWindow = new GameWindow();
			renderer = new Renderer();
			model = new EngineModel();

			gameWindow.Resize += GameWindow_Resize;
			gameWindow.RenderFrame += GameWindow_RenderFrame;
			gameWindow.RenderFrame += (sender, e) => { gameWindow.SwapBuffers(); };
		}

		static void Main(string[] args)
		{
			var app = new MyApplication();
			app.gameWindow.Run(60, 60);
		}

		private void GameWindow_Resize(object sender, System.EventArgs e)
		{
			renderer.ResizeWindow(gameWindow.Width, gameWindow.Height);
		}

		private void GameWindow_RenderFrame(object sender, FrameEventArgs e)
		{
			renderer.Render(model.World);
		}
	}
}
