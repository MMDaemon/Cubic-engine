using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GraphicsHelper.GraphicsUtils
{
	public class VertexArrayObject : IDisposable
	{
		private struct IdData
		{
			public readonly DrawElementsType DrawElementsType;
			public readonly int Length;
			public readonly PrimitiveType PrimitiveType;

			public IdData(PrimitiveType primitiveType, int length, DrawElementsType drawElementsType)
			{
				this.PrimitiveType = primitiveType;
				this.Length = length;
				this.DrawElementsType = drawElementsType;
			}
		}

		#region members

		private const int IdBufferBinding = int.MaxValue;

		private IdData _idData;
		private int _idVao;
		private readonly Dictionary<int, uint> _boundBuffers = new Dictionary<int, uint>();

		#endregion

		#region constructor

		public VertexArrayObject()
		{
			_idVao = GL.GenVertexArray();
		}

		#endregion

		#region interface methods

		public void Dispose()
		{
			foreach (uint id in _boundBuffers.Values)
			{
				GL.DeleteBuffer(id);
			}
			_boundBuffers.Clear();
			GL.DeleteVertexArray(_idVao);
			_idVao = 0;
		}

		#endregion

		#region public methods

		public void SetId<TIndex>(TIndex[] data, PrimitiveType primitiveType) where TIndex : struct
		{
			Activate();
			uint bufferId = RequestBuffer(IdBufferBinding);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, bufferId);
			var type = typeof(TIndex);
			int elementBytes = Marshal.SizeOf(type);
			int bufferByteSize = data.Length * elementBytes;
			// set buffer data
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)bufferByteSize, data, BufferUsageHint.StaticDraw);
			//cleanup state
			Deactive();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			//save data for draw call
			DrawElementsType drawElementsType = GetDrawElementsType(type);
			_idData = new IdData(primitiveType, data.Length, drawElementsType);
		}

		public void SetAttribute<DataElement>(int bindingId, DataElement[] data, VertexAttribPointerType type, int elementSize, bool perInstance = false) where DataElement : struct
		{
			if (-1 == bindingId) return; //if matrix not used in shader or wrong name
			Activate();
			uint bufferId = RequestBuffer(bindingId);
			GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
			int elementBytes = Marshal.SizeOf(typeof(DataElement));
			int bufferByteSize = data.Length * elementBytes;
			// set buffer data
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)bufferByteSize, data, BufferUsageHint.StaticDraw);
			//set data format
			GL.VertexAttribPointer(bindingId, elementSize, type, false, elementBytes, 0);
			GL.EnableVertexAttribArray(bindingId);
			if (perInstance)
			{
				GL.VertexAttribDivisor(bindingId, 1);
			}
			//cleanup state
			Deactive();
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DisableVertexAttribArray(bindingId);
		}

		/// <summary>
		/// sets or updates a vertex attribute of type Matrix4
		/// </summary>
		/// <param name="bindingId">shader binding location</param>
		/// <param name="data">ATTENTION: here the matrices are assumed to be rowmajor. why i don't know</param>
		/// <param name="perInstance"></param>
		public void SetMatrixAttribute(int bindingId, Matrix4[] data, bool perInstance = false)
		{
			if (-1 == bindingId) return; //if matrix not used in shader or wrong name
			Activate();
			uint bufferId = RequestBuffer(bindingId);
			GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
			int elementBytes = Marshal.SizeOf(typeof(Matrix4));
			int columnBytes = Marshal.SizeOf(typeof(Vector4));
			int bufferByteSize = data.Length * elementBytes;
			// set buffer data
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)bufferByteSize, data, BufferUsageHint.StaticDraw);
			//set data format
			for (int i = 0; i < 4; i++)
			{
				GL.VertexAttribPointer(bindingId + i, 4, VertexAttribPointerType.Float, false, elementBytes, columnBytes * i);
				GL.EnableVertexAttribArray(bindingId + i);
				if (perInstance)
				{
					GL.VertexAttribDivisor(bindingId + i, 1);
				}
			}
			//cleanup state
			Deactive();
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			for (int i = 0; i < 4; i++)
			{
				GL.DisableVertexAttribArray(bindingId + i);
			}
		}

		public void Activate()
		{
			GL.BindVertexArray(_idVao);
		}

		public void Deactive()
		{
			GL.BindVertexArray(0);
		}

		public void Draw(int instanceCount = 1)
		{
			if (0 == _idData.Length) throw new VertexArrayObjectException("Empty id data set! Draw yourself using active/deactivate!");
			Activate();
			GL.DrawElementsInstanced(_idData.PrimitiveType, _idData.Length, _idData.DrawElementsType, (IntPtr)0, instanceCount);
			Deactive();
		}

		#endregion

		#region private methods

		private static DrawElementsType GetDrawElementsType(Type type)
		{
			if (type == typeof(ushort)) return DrawElementsType.UnsignedShort;
			if (type == typeof(uint)) return DrawElementsType.UnsignedInt;
			throw new Exception("Invalid index type");
		}

		private uint RequestBuffer(int bindingId)
		{
			uint bufferId;
			if (!_boundBuffers.TryGetValue(bindingId, out bufferId))
			{
				GL.GenBuffers(1, out bufferId);
				_boundBuffers[bindingId] = bufferId;
			}
			return bufferId;
		}

		#endregion
	}
}
