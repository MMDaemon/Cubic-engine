using System.Collections;
using System.Collections.Generic;
using CubicEngine.Utils.Enums;
using CubicEngine.Utils;

namespace CubicEngine.Model
{
	class MaterialList : IEnumerable
	{
		private Dictionary<MaterialType, int> _materials;

		/// <summary>
		/// Constructor of the MaterialList.
		/// </summary>
		public MaterialList()
		{
			_materials = new Dictionary<MaterialType, int>();
		}

		/// <summary>
		/// Adds the amount of the material of specified type to the MaterialList if possible.
		/// </summary>
		/// <param name="type">type of the material to add.</param>
		/// <param name="amount">amount of the material to add. Can not be 0.</param>
		/// <returns>If the action was sucessfull.</returns>
		public bool Add(MaterialType type, int amount)
		{
			bool possible = amount != 0 && Amount + amount <= Constants.MAX_AMOUNT;
			if (possible)
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
			return possible;
		}

		/// <summary>
		/// Removes the amount of the material of specified type from the MaterialList if possible.
		/// </summary>
		/// <param name="type">type of the material to remove.</param>
		/// <param name="amount">amount of the material to remove.</param>
		/// <returns>If the action was sucessfull.</returns>
		public bool Remove(MaterialType type, int amount)
		{
			bool possible = _materials.ContainsKey(type) && _materials[type] >= amount;
			if (possible)
			{
				_materials[type] -= amount;
				if (_materials[type] == 0)
				{
					_materials.Remove(type);
				}
			}
			return possible;
		}

		public IEnumerator GetEnumerator()
		{
			return _materials.GetEnumerator();
		}

		public int this[MaterialType type]
		{
			get
			{
				int amount = 0;

				if (_materials.ContainsKey(type))
				{
					amount = _materials[type];
				}

				return amount;
			}
		}

		public int Amount
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
