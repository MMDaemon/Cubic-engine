using CubicEngine.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;

namespace CubicEngine.Model
{
	internal struct Material
	{
		public Material(MaterialType type, int amount)
		{
			Type = type;
			Amount = amount;
		}

		public MaterialType Type { get; }

		public int Amount { get; }

		public Color4 Color
		{
			get
			{
				Color4 color;
				switch (Type)
				{
					case MaterialType.Stone:
						color = new Color4(0.4f, 0.4f, 0.4f, 1);
						break;
					case MaterialType.Dirt:
						color = new Color4(0.3f, 0.2f, 0.05f, 1);
						break;
					default:
						color = new Color4(0, 0, 0, 1);
						break;
				}
				return color;
			}
		}
	}
}