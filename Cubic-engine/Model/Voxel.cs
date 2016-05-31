namespace CubicEngine.Model
{
	class Voxel
	{
		private MaterialList _materials;

		/// <summary>
		/// Constructor of the Voxel.
		/// </summary>
		public Voxel()
		{
			_materials = new MaterialList();
		}

		public bool Surface { get; set; }
		

		public MaterialList Materials
		{
			get { return _materials; }
		}
	}
}
