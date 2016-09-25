using CubicEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CubicEngine.Model
{
	internal class ChunkEventArgs : EventArgs
	{
		public Chunk Chunk { get; private set; }

		public ChunkEventArgs(Chunk chunk)
		{
			Chunk = chunk;
		}
	}

	internal class World
	{
		readonly Dictionary<Vector3I, Chunk> _loadedChunks = new Dictionary<Vector3I, Chunk>();
		readonly Dictionary<Vector3I, Chunk> _renderingChunks = new Dictionary<Vector3I, Chunk>();

		public event EventHandler<ChunkEventArgs> ChunkRenderReady;

		public Voxel GetVoxel(Vector3I absolutePosition)
		{
			Vector3I pos = absolutePosition;
			if (absolutePosition.X < 0)
			{
				pos.X -= (Constants.ChunkSize.X - 1);
			}
			if (absolutePosition.Y < 0)
			{
				pos.Y -= (Constants.ChunkSize.Y - 1);
			}
			if (absolutePosition.Z < 0)
			{
				pos.Z -= (Constants.ChunkSize.Z - 1);
			}
			Vector3I chunkPosition = pos / Constants.ChunkSize;
			Vector3I internalPosition = absolutePosition - chunkPosition * Constants.ChunkSize;

			return _loadedChunks[chunkPosition][internalPosition];
		}

		public void Update()
		{
			AddChunks();
			CheckForRenderReadyChunks();
		}

		public void ResendChunks()
		{
			foreach (Chunk chunk in _renderingChunks.Values)
			{
				OnChunkRenderReady(chunk);
			}
		}

		protected virtual void OnChunkRenderReady(Chunk chunk)
		{
			ChunkEventArgs e = new ChunkEventArgs(chunk);
			EventHandler<ChunkEventArgs> handler = ChunkRenderReady;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void AddChunks()
		{
			if (_loadedChunks.Count == 0)
			{
				_loadedChunks.Add(new Vector3I(0, 0, 0), new Chunk(new Vector3I(0, 0, 0), CreateManualVoxels(), this));
				_loadedChunks.Add(new Vector3I(1, 0, 0), new Chunk(new Vector3I(1, 0, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(0, 1, 0), new Chunk(new Vector3I(0, 1, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(0, 0, 1), new Chunk(new Vector3I(0, 0, 1), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(-1, 0, 0), new Chunk(new Vector3I(-1, 0, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(0, -1, 0), new Chunk(new Vector3I(0, -1, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(0, 0, -1), new Chunk(new Vector3I(0, 0, -1), CreateEmptyVoxels(), this));

				_loadedChunks.Add(new Vector3I(1, 1, 0), new Chunk(new Vector3I(1, 1, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(1, 0, 1), new Chunk(new Vector3I(1, 0, 1), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(1, -1, 0), new Chunk(new Vector3I(1, -1, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(1, 0, -1), new Chunk(new Vector3I(1, 0, -1), CreateEmptyVoxels(), this));

				_loadedChunks.Add(new Vector3I(-1, 1, 0), new Chunk(new Vector3I(-1, 1, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(-1, 0, 1), new Chunk(new Vector3I(-1, 0, 1), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(-1, -1, 0), new Chunk(new Vector3I(-1, -1, 0), CreateEmptyVoxels(), this));
				_loadedChunks.Add(new Vector3I(-1, 0, -1), new Chunk(new Vector3I(-1, 0, -1), CreateEmptyVoxels(), this));
			}
		}

		private void CheckForRenderReadyChunks()
		{
			List<Chunk> chunks = new List<Chunk>();
			foreach (Chunk chunk in _loadedChunks.Values)
			{
				if (CheckRenderReady(chunk))
				{
					chunks.Add(chunk);
				}
			}
			foreach (Chunk chunk in chunks)
			{
				SetRendering(chunk);
			}
		}

		private bool CheckRenderReady(Chunk chunk)
		{
			Vector3I[] positions = new Vector3I[]
			{
				chunk.Position+new Vector3I(1,0,0),
				chunk.Position+new Vector3I(0,1,0),
				chunk.Position+new Vector3I(0,0,1),
				chunk.Position+new Vector3I(-1,0,0),
				chunk.Position+new Vector3I(0,-1,0),
				chunk.Position+new Vector3I(0,0,-1),
				chunk.Position+new Vector3I(1,1,0),
				chunk.Position+new Vector3I(1,0,1),
				chunk.Position+new Vector3I(1,-1,0),
				chunk.Position+new Vector3I(1,0,-1),
				chunk.Position+new Vector3I(-1,1,0),
				chunk.Position+new Vector3I(-1,0,1),
				chunk.Position+new Vector3I(-1,-1,0),
				chunk.Position+new Vector3I(-1,0,-1)
			};

			bool renderReady = true;
			foreach (Vector3I position in positions)
			{
				if (!_loadedChunks.Keys.Contains(position) && !_renderingChunks.Keys.Contains(position))
				{
					renderReady = false;
				}
			}

			if (renderReady)
			{
				OnChunkRenderReady(chunk);
			}
			return renderReady;
		}

		private void SetRendering(Chunk chunk)
		{
			_loadedChunks.Remove(chunk.Position);
			_renderingChunks.Add(chunk.Position, chunk);
		}

		private Voxel[,,] CreateEmptyVoxels()
		{
			Voxel[,,] voxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						voxels[x, y, z] = new Voxel();
					}
				}
			}
			return voxels;
		}

		private Voxel[,,] CreateManualVoxels()
		{
			Voxel[,,] createVoxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						createVoxels[x, y, z] = new Voxel();
					}
				}
			}

			int[,] stoneHeights = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,1,2,2,2,3,3,3,6,8,10,12,14,16,18,20,22,23,24,25,24},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,2,2,3,3,4,6,8,10,11,14,16,17,18,18,20,21,24,23,22},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,2,3,3,4,5,6,8,9,12,14,17,17,18,19,20,21,22,22,21},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,2,3,4,5,7,8,9,11,14,16,17,17,18,19,19,19,20,20,20},
				{0,0,0,0,0,0,0,0,0,0,0,1,2,3,3,4,6,7,8,10,12,15,16,17,17,18,18,19,18,18,18,18},
				{0,0,0,0,0,0,0,0,0,0,1,1,2,3,3,4,6,7,8,8,10,12,14,16,17,17,18,18,17,17,18,18},
				{0,0,0,0,0,0,0,0,0,0,1,1,2,3,3,4,6,7,7,8,9,11,14,15,16,17,17,17,17,17,17,17},
				{0,0,0,0,0,0,0,0,0,0,1,1,2,3,3,4,6,6,7,7,8,10,12,14,16,17,17,16,17,17,17,17},
				{0,0,0,0,0,0,0,0,0,0,1,1,2,2,3,4,5,6,7,8,9,10,11,13,15,16,16,16,16,17,17,17},
				{0,0,0,0,0,0,0,0,0,0,1,1,1,2,3,4,5,6,7,8,9,9,10,12,13,14,14,15,16,17,16,16},
				{0,0,0,0,0,0,0,0,0,0,1,1,1,2,3,4,5,6,6,7,7,8,8,10,12,12,13,14,13,15,15,14},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,2,2,3,4,5,6,7,7,7,7,9,10,10,11,12,12,13,13,12},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,2,3,3,4,6,7,7,7,7,8,9,9,8,10,10,12,11,11,10},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,2,3,3,4,5,6,7,7,7,7,8,8,7,8,9,9,10,9,8},
				{0,0,0,0,0,0,0,0,0,0,1,1,1,2,2,3,3,3,4,4,6,6,7,7,7,7,7,7,8,9,8,7},
				{0,0,0,0,0,0,0,0,0,0,1,1,1,1,2,2,2,3,3,4,5,5,4,4,5,6,6,6,6,8,8,7},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,1,2,2,2,2,2,2,3,3,3,4,4,5,5,4,4,6,7,6},
				{0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,2,2,2,2,2,2,2,2,3,3,3,3,3,3,4,5,4},
				{0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,1,1,1,1,1,2,2,2,1},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
			};

			for (int x = 0; x < 32; x++)
			{
				for (int z = 0; z < 32; z++)
				{
					for (int y = 0; y <= stoneHeights[z, x]; y++)
					{
						createVoxels[x, y, z].Materials.Add("Stone", 64);
					}
				}
			}

			int[,] dirtHeights = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},

				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,5,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,4,5,5,5,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,3,4,4,4,4,5,5,5,5,5,5,0,0,0,0,0,0},
				{0,0,0,1,1,1,2,2,2,2,2,1,0,0,2,4,4,4,4,4,4,4,4,4,4,5,5,5,5,0,0,0},
				{0,1,1,2,2,2,3,3,3,3,3,2,1,1,3,4,4,4,4,4,4,4,4,4,4,4,4,4,4,5,5,5},
				{0,1,2,3,3,3,4,4,4,4,4,3,2,2,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{1,2,3,4,4,4,5,5,5,5,4,4,3,3,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{2,3,4,5,5,5,5,5,5,5,5,4,4,4,4,4,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4},
				{3,4,5,5,5,5,5,5,5,5,5,5,4,4,5,5,5,5,5,5,5,5,5,5,5,5,4,4,4,4,4,4},
				{4,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,4,4},
				{5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,6,6,5,5,5,5,5,5,5,5,4},
				{5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,5,5,5,5,5,5,4},
				{5,5,5,5,5,5,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5,5,5,5},
				{5,5,5,5,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5,5,5},
				{5,5,5,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5,5,5},
				{6,6,6,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5,5},
				{6,6,6,6,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5},
				{6,6,6,6,6,6,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5},
				{6,6,6,6,6,6,6,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5},
			};

			for (int x = 0; x < 32; x++)
			{
				for (int z = 0; z < 32; z++)
				{
					for (int y = 0; y <= dirtHeights[z, x]; y++)
					{
						createVoxels[x, y, z].Materials.Add("Dirt", 64);
					}
				}
			}

			int[,] sandHeights = new int[,]
			{
				{5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{5,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
			};

			for (int x = 0; x < 32; x++)
			{
				for (int z = 0; z < 32; z++)
				{
					for (int y = 0; y <= sandHeights[z, x]; y++)
					{
						createVoxels[x, y, z].Materials.Add("Sand", 64);
					}
				}
			}

			Voxel[,,] voxels = new Voxel[Constants.ChunkSize.X, Constants.ChunkSize.Y, Constants.ChunkSize.Z];
			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int y = 0; y < Constants.ChunkSize.Y; y++)
				{
					for (int z = 0; z < Constants.ChunkSize.Z; z++)
					{
						voxels[x, y, z] = new Voxel();

						if (createVoxels[x, y, z].Materials.Amount > 0)
						{

							foreach (Material material in createVoxels[x, y, z].Materials)
							{
								voxels[x, y, z].Materials.Add(material.TypeId, material.Amount / 2);
							}

							Dictionary<int, int> materials = new Dictionary<int, int>();

							for (int i = -1; i <= 1; i++)
							{
								for (int j = -1; j <= 1; j++)
								{
									for (int k = -1; k <= 1; k++)
									{
										if ((i != 0 || j != 0 || k != 0) && (i == 0 || j == 0 || k == 0))
										{
											try
											{
												foreach (Material material in createVoxels[x + i, y + j, z + k].Materials)
												{
													if (!materials.ContainsKey(material.TypeId))
													{
														materials.Add(material.TypeId, material.Amount);
													}
													else
													{
														materials[material.TypeId] += material.Amount;
													}
												}
											}
											catch (IndexOutOfRangeException)
											{
												foreach (Material material in createVoxels[x, y, z].Materials)
												{
													if (!materials.ContainsKey(material.TypeId))
													{
														materials.Add(material.TypeId, material.Amount);
													}
													else
													{
														materials[material.TypeId] += material.Amount;
													}
												}
											}
										}
									}
								}
							}
							bool surface = false;

							foreach (Vector3I direction in Constants.DirectionVectors)
							{
								try
								{
									if (createVoxels[x + direction.X, y + direction.Y, z + direction.Z].Materials.Amount == 0)
									{
										surface = true;
									}
								}
								catch (IndexOutOfRangeException) { }
							}

							voxels[x, y, z].Surface = surface;

							foreach (KeyValuePair<int, int> material in materials)
							{
								voxels[x, y, z].Materials.Add(material.Key, material.Value / 18 / 2);
							}

							if (voxels[x, y, z].Materials.Contains("Dirt") && surface)
							{
								voxels[x, y, z].Materials.Remove("Dirt", 1);
								voxels[x, y, z].Materials.Add("Grass", 1);
							}
						}
					}
				}
			}

			return voxels;
		}
	}
}
