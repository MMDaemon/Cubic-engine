using System.Drawing;

namespace CubicEngine.Model
{
	internal class MaterialType
	{
		public MaterialType(string name, Color color)
		{
			Name = name;
			Color = color;
		}

		public string Name { get; private set; }
		public Color Color { get; private set; }
	}
}
