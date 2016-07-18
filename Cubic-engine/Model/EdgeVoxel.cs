using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubicEngine.Model
{
	class EdgeVoxel : Voxel
	{
		public Voxel XVoxel { get; set; }
		public Voxel YVoxel { get; set; }
		public Voxel ZVoxel { get; set; }
	}
}
