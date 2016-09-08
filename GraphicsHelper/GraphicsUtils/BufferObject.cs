using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace GraphicsHelper.GraphicsUtils
{
	public class BufferObject : IDisposable
	{
		private int _bufferId;

		public BufferObject(BufferTarget bufferTarget)
		{
			BufferTarget = bufferTarget;
			GL.GenBuffers​(1, out _bufferId);
		}

		public void Dispose()
		{
			if (-1 == _bufferId) return;
			GL.DeleteBuffer(_bufferId);
			_bufferId = -1;
		}

		public BufferTarget BufferTarget { get; private set; }

		public void Activate()
		{
			GL.BindBuffer​(BufferTarget, _bufferId);
		}

		public void ActivateBind(int index)
		{
			Activate();
			BufferRangeTarget target = (BufferRangeTarget)BufferTarget;
			GL.BindBufferBase​(target, index, _bufferId);
		}

		public void Deactive()
		{
			GL.BindBuffer​(BufferTarget, 0);
		}

		public void Set<TDataElement>(TDataElement[] data, BufferUsageHint usageHint) where TDataElement : struct
		{
			Activate();
			int elementBytes = Marshal.SizeOf(typeof(TDataElement));
			int bufferByteSize = data.Length * elementBytes;
			// set buffer data
			GL.BufferData(BufferTarget, (IntPtr)bufferByteSize, data, usageHint);
			//cleanup state
			Deactive();
		}
	}
}
