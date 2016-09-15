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

			// Add Materials
			voxels[0, 1, 0].Materials.Add(MaterialManager.Instance.GetMaterialId("Stone"), 63);
			voxels[0, 1, 0].Materials.Add(MaterialManager.Instance.GetMaterialId("Dirt"), 1);
			voxels[0, 1, 0].Surface = true;
			voxels[1, 1, 1].Materials.Add(MaterialManager.Instance.GetMaterialId("Grass"), 50);
			voxels[1, 1, 1].Materials.Add(MaterialManager.Instance.GetMaterialId("Dirt"), 14);
			voxels[1, 1, 1].Surface = true;

			for (int x = 0; x < Constants.ChunkSize.X; x++)
			{
				for (int z = 0; z < Constants.ChunkSize.Z; z++)
				{
					voxels[x, 0, z].Materials.Add(MaterialManager.Instance.GetMaterialId("Stone"), 64);
					voxels[x, 0, z].Surface = true;
				}
			}

			return voxels;
		}
	}
}
