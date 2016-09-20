using System;
using CubicEngine.Model;
using CubicEngine.View;
using OpenTK;
using OpenTK.Input;
using CubicEngine.Controller;
using CubicEngine.Utils.Enums;
using System.Collections.Generic;

namespace CubicEngine
{
	internal class MyApplication
	{
		private readonly GameWindow _gameWindow;
		KeyboardListener _keyboardListener;
		private readonly World _world;
		private readonly Renderer _renderer;

		private MyApplication()
		{
			_gameWindow = new GameWindow();
			_keyboardListener = new KeyboardListener();
			_world = new World();
			_renderer = new Renderer();

			_gameWindow.MouseMove += GameWindow_MouseMove;
			_gameWindow.Resize += GameWindow_Resize;
			_gameWindow.RenderFrame += GameWindow_RenderFrame;
			_gameWindow.RenderFrame += (sender, e) => { _gameWindow.SwapBuffers(); };
			_gameWindow.UpdateFrame += GameWindow_UpdateFrame;
			_gameWindow.KeyDown += GameWindow_KeyDown;
			_gameWindow.KeyUp += GameWindow_KeyUp;

			_world.ChunkRenderReady += World_ChunkRenderReady;
			_renderer.ReRender += _renderer_ReRender;
		}

		private void MoveCamera(List<KeyAction> actions)
		{
			Matrix4 rotationY = Matrix4.CreateRotationY(_renderer.Camera.Heading);

			if (actions.Contains(KeyAction.MoveForwards))
			{
				_renderer.Camera.Position += new Vector3(rotationY.M11, rotationY.M21, rotationY.M31);
			}

			if (actions.Contains(KeyAction.MoveBackwards))
			{
				_renderer.Camera.Position -= new Vector3(rotationY.M11, rotationY.M21, rotationY.M31);
			}

			if (actions.Contains(KeyAction.MoveLeft))
			{
				Matrix4 rotationLeft = Matrix4.CreateRotationY(_renderer.Camera.Heading - (float)Math.PI / 2);
				_renderer.Camera.Position += new Vector3(rotationLeft.M11, rotationLeft.M21, rotationLeft.M31);
			}

			if (actions.Contains(KeyAction.MoveRight))
			{
				Matrix4 rotationRight = Matrix4.CreateRotationY(_renderer.Camera.Heading + (float)Math.PI / 2);
				_renderer.Camera.Position += new Vector3(rotationRight.M11, rotationRight.M21, rotationRight.M31);
			}

			if (actions.Contains(KeyAction.MoveUp))
			{
				_renderer.Camera.Position += Vector3.UnitY;
			}

			if (actions.Contains(KeyAction.MoveDown))
			{
				_renderer.Camera.Position -= Vector3.UnitY;
			}

			if (actions.Contains(KeyAction.SwitchMethod))
			{
				_renderer.SwitchRenderMethod();
			}

			if (actions.Contains(KeyAction.SwitchAlgorythm))
			{
				_renderer.SwitchRenderAlgorythm();
			}

		}

		private void World_ChunkRenderReady(object sender, ChunkEventArgs e)
		{
			_renderer.AddChunk(e.Chunk);
		}

		private void _renderer_ReRender(object sender, EventArgs e)
		{
			_world.ResendChunks();
		}

		private void GameWindow_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			_keyboardListener.KeyDown(e.Key);
		}

		private void GameWindow_KeyUp(object sender, KeyboardKeyEventArgs e)
		{
			_keyboardListener.KeyUp(e.Key);
		}

		private void GameWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			MoveCamera(_keyboardListener.GetKeyActions());
			_world.Update();
		}

		public static void Main(string[] args)
		{
			var app = new MyApplication();
			app._gameWindow.Run(60, 60);
		}

		private void GameWindow_Resize(object sender, EventArgs e)
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
				if (_renderer.Camera.Tilt > (float)(Math.PI / 2 - 0.01))
				{
					_renderer.Camera.Tilt = (float)(Math.PI / 2 - 0.01);
				}
				if (_renderer.Camera.Tilt < (float)-(Math.PI / 2 - 0.01))
				{
					_renderer.Camera.Tilt = (float)-(Math.PI / 2 - 0.01);
				}
			}
		}
	}
}
