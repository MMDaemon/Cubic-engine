namespace CubicEngine.Model
{
	internal class EngineModel
	{
		public Chunk GetChunk(int x, uint y, int z)
		{
			return new Chunk(x, y, z);
		}
	}
}
