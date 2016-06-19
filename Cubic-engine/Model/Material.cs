using CubicEngine.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubicEngine.Model
{
	internal class Material
	{
		public Material(MaterialType type, int amount)
		{
			Type = type;
			Amount = amount;
		}

		public MaterialType Type { get; }

		public int Amount { get; }
	}
}