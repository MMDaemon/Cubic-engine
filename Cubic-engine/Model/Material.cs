using CubicEngine.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubicEngine.Model
{
	class Material
	{
		private MaterialType _type;

		private int _amount;


		public Material(MaterialType type, int amount)
		{
			_type = type;
			_amount = amount;
		}

		public MaterialType Type
		{
			get { return _type; }
		}

		public int Amount
		{
			get { return _amount; }
		}
	}
}
