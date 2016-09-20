using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CubicEngine.Model
{
	internal class MaterialManager
	{
		private readonly Dictionary<int, MaterialType> _materialTypes;
		private static MaterialManager _instance;

		private MaterialManager()
		{
			_materialTypes = new Dictionary<int, MaterialType>();
			InitializeMaterials();
		}

		public bool ContainsMaterial(int materialId)
		{
			return _materialTypes.ContainsKey(materialId);
		}

		/// <summary>
		/// Gets the id of the material with the given materialName.
		/// </summary>
		/// <param name="materialName">Name of the material.</param>
		/// <returns>Id of the material. -1 if material does not exist.</returns>
		public int GetMaterialId(string materialName)
		{
			int[] materialIds =
				(from materialType in _materialTypes
				 where materialType.Value.Name == materialName
				 select materialType.Key).ToArray();

			int materialId = -1;
			if (materialIds.Length == 1)
			{
				materialId = materialIds[0];
			}

			return materialId;
		}

		public string GetMaterialName(int materialId)
		{
			return _materialTypes[materialId].Name;
		}

		public Bitmap GetMaterialsAsBitmap()
		{
			Bitmap bitmap = new Bitmap(_materialTypes.Count, 1);
			foreach (KeyValuePair<int, MaterialType> entry in _materialTypes)
			{
				bitmap.SetPixel(entry.Key, 0, entry.Value.Color);
			}
			return bitmap;
		}

		private bool Add(string materialName, Color color)
		{
			bool sucess = false;
			if (GetMaterialId(materialName) == -1)
			{
				int materialId = 0;
				while (_materialTypes.ContainsKey(materialId))
				{
					materialId++;
				}
				_materialTypes.Add(materialId, new MaterialType(materialName, color));
				sucess = true;
			}
			return sucess;
		}

		private void InitializeMaterials()
		{
			Add("Stone", Color.FromArgb(255, 130, 130, 130));
			Add("Dirt", Color.FromArgb(255, 110, 90, 15));
			Add("Sand", Color.FromArgb(255, 250, 250, 190));
			Add("Grass", Color.FromArgb(255, 20, 250, 20));
		}

		public int MaterialCount => _materialTypes.Count;

		public static MaterialManager Instance => _instance ?? (_instance = new MaterialManager());
	}
}
