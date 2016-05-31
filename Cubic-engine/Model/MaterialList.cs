using System.Collections;
using System.Collections.Generic;
using System;
using CubicEngine.Utils.Enums;
using CubicEngine.Utils;

namespace CubicEngine.Model
{
	class MaterialList : IEnumerable
	{
		private Dictionary<MaterialType, int> _materials;

		public IEnumerator GetEnumerator()
		{
			return _materials.GetEnumerator();
		}

		public void Add(MaterialType type, int amount)
		{
			if (CurrentAmount + amount <= Constants.MAX_AMOUNT)
			{
				if (_materials.ContainsKey(type))
				{
					_materials[type] += amount;
				}
				else
				{
					_materials.Add(type, amount);
				}
			}
			else
			{
				throw new Exception("Not enough space");
			}
		}

		public void Remove(MaterialType type, int amount)
		{
			if (_materials.ContainsKey(type) && _materials[type] >= amount)
			{
				_materials[type] -= amount;
				if (_materials[type] == 0)
				{
					_materials.Remove(type);
				}
			}
		}

		public float this[MaterialType type]
		{
			get
			{
				return _materials[type];
			}
		}

		public int CurrentAmount
		{
			get
			{
				int currentAmount = 0;
				foreach (KeyValuePair<MaterialType, int> material in _materials)
				{
					currentAmount += material.Value;
				}
				return currentAmount;
			}
		}
	}
}
