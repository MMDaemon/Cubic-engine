using Model;

namespace CubicEngine.Model
{
	internal struct Material
	{
		public Material(string typeName, int amount)
		{
			TypeId = MaterialManager.Instance.GetMaterialId(typeName);
			Amount = amount;
		}

		public Material(int typeId, int amount)
		{
			TypeId = typeId;
			Amount = amount;
		}

		public int TypeId { get; }

		public int Amount { get; }
	}
}