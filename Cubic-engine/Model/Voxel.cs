using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubicEngine.Model
{
	class Voxel
	{
		private MaterialList _materials;

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
