namespace CubicEngine.Model
{
	internal class EngineModel
	{
		public EngineModel()
		{
			World = new Chunk();
		}

		public Chunk World { get; private set; }
	}
}
