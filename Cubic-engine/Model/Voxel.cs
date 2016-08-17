namespace CubicEngine.Model
{
	internal class Voxel
	{
		/// <summary>
		/// Constructor of the Voxel.
		/// </summary>
		public Voxel()
		{
			Materials = new MaterialList();
			Surface = false;
		}

		public bool Surface { get; set; }

		public MaterialList Materials { get; }
	}
}