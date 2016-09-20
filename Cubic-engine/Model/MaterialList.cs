using System.Collections;
using System.Collections.Generic;
using CubicEngine.Utils;

namespace CubicEngine.Model
{
	internal class MaterialList : IEnumerable
	{
		private readonly Dictionary<int, int> _materials;
		private MaterialManager _materialManager;

		/// <summary>
		/// Constructor of the MaterialList.
		/// </summary>
		public MaterialList()
		{
			_materials = new Dictionary<int, int>();
			_materialManager = MaterialManager.Instance;
		}

		public int Amount
		{
			get
			{
				int currentAmount = 0;
				foreach (int amount in _materials.Values)
				{
					currentAmount += amount;
				}
				return currentAmount;
			}
		}

		public bool IsEmpty()
		{
			return _materials.Count == 0;
		}

		public bool Add(string materialName, int amount)
		{
			return Add(_materialManager.GetMaterialId(materialName), amount);
		}

		/// <summary>
		/// Adds the amount of the material of specified type to the MaterialList if possible.
		/// </summary>
		/// <param name="materialId">type of the material to add.</param>
		/// <param name="amount">amount of the material to add. Can not be 0.</param>
		/// <returns>If the action was sucessfull.</returns>
		public bool Add(int materialId, int amount)
		{
			bool possible = amount != 0 && Amount + amount <= Constants.MaxAmount && _materialManager.ContainsMaterial(materialId);
			if (possible)
			{
				if (_materials.ContainsKey(materialId))
				{
					_materials[materialId] += amount;
				}
				else
				{
					_materials.Add(materialId, amount);
				}
			}
			return possible;
		}

		public bool Remove(string materialName, int amount)
		{
			return Remove(_materialManager.GetMaterialId(materialName), amount);
		}

		/// <summary>
		/// Removes the amount of the material of specified type from the MaterialList if possible.
		/// </summary>
		/// <param name="materialId">type of the material to remove.</param>
		/// <param name="amount">amount of the material to remove.</param>
		/// <returns>If the action was sucessfull.</returns>
		public bool Remove(int materialId, int amount)
		{
			bool possible = _materials.ContainsKey(materialId) && _materials[materialId] >= amount;
			if (possible)
			{
				_materials[materialId] -= amount;
				if (_materials[materialId] == 0)
				{
					_materials.Remove(materialId);
				}
			}
			return possible;
		}

		public bool Contains(string materialName)
		{
			return _materials.ContainsKey(_materialManager.GetMaterialId(materialName));
		}

		public bool Contains(int typeId)
		{
			return _materials.ContainsKey(typeId);
		}

		public IEnumerator GetEnumerator()
		{
			List<Material> materials = new List<Material>();
			foreach (KeyValuePair<int, int> material in _materials)
			{
				materials.Add(new Material(material.Key, material.Value));
			}
			return materials.GetEnumerator();
		}

		public int this[string typeName] => this[_materialManager.GetMaterialId(typeName)];

		public int this[int typeId]
		{
			get
			{
				int amount = 0;

				if (_materials.ContainsKey(typeId))
				{
					amount = _materials[typeId];
				}

				return amount;
			}
		}

		public Dictionary<int, float> GetMaterialDistribution()
		{
			Dictionary<int, float> materialDistribution = new Dictionary<int, float>();

			foreach (KeyValuePair<int, int> material in _materials)
			{
				materialDistribution.Add(material.Key, (float)material.Value / Amount);
			}

			return materialDistribution;
		}

		public override string ToString()
		{
			string text = "(";
			foreach(KeyValuePair<int,int> material in _materials)
			{
				text+= string.Format("{0}: {1}; ",_materialManager.GetMaterialName(material.Key),material.Value);
			}
			return text+")";
		}
	}
}
