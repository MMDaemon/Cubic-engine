using CubicEngine.Utils.Enums;
using OpenTK.Input;
using System.Collections.Generic;

namespace CubicEngine.Controller
{
	class KeyboardListener
	{
		Dictionary<Key, bool> KeyStates = new Dictionary<Key, bool>();

		public KeyboardListener()
		{
			KeyStates.Add(Key.W, false);
			KeyStates.Add(Key.A, false);
			KeyStates.Add(Key.S, false);
			KeyStates.Add(Key.D, false);
			KeyStates.Add(Key.Up, false);
			KeyStates.Add(Key.Left, false);
			KeyStates.Add(Key.Down, false);
			KeyStates.Add(Key.Right, false);
			KeyStates.Add(Key.Space, false);
			KeyStates.Add(Key.ShiftLeft, false);
		}

		public void KeyDown(Key key)
		{
			if (KeyStates.ContainsKey(key))
			{
				KeyStates[key] = true;
			}
		}

		public void KeyUp(Key key)
		{
			if (KeyStates.ContainsKey(key))
			{
				KeyStates[key] = false;
			}
		}

		public List<KeyAction> GetKeyActions()
		{
			List<KeyAction> movements = new List<KeyAction>();
			if (KeyStates[Key.W] || KeyStates[Key.Up])
			{
				movements.Add(KeyAction.MoveForwards);
			}
			if (KeyStates[Key.A] || KeyStates[Key.Left])
			{
				movements.Add(KeyAction.MoveLeft);
			}
			if (KeyStates[Key.S] || KeyStates[Key.Down])
			{
				movements.Add(KeyAction.MoveBackwards);
			}
			if (KeyStates[Key.D] || KeyStates[Key.Right])
			{
				movements.Add(KeyAction.MoveRight);
			}
			if (KeyStates[Key.Space])
			{
				movements.Add(KeyAction.MoveUp);
			}
			if (KeyStates[Key.ShiftLeft])
			{
				movements.Add(KeyAction.MoveDown);
			}

			return movements;
		}

	}
}
