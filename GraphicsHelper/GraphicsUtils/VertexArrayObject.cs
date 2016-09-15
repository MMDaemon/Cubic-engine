using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

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
				PrimitiveType = primitiveType;
				Length = length;
				DrawElementsType = drawElementsType;
			}
		}

		#region members

		private const int IdBufferBinding = int.MaxValue;

		private IdData _idData;
		private int _idVao;
		private readonly Dictionary<int, BufferObject> _boundBuffers = new Dictionary<int, BufferObject>();

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
			foreach (var buffer in _boundBuffers.Values)
			{
				buffer.Dispose();
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
			var buffer = RequestBuffer(IdBufferBinding, BufferTarget.ElementArrayBuffer);
			// set buffer data
			buffer.Set(data, BufferUsageHint.StaticDraw);
			//activate for state
			buffer.Activate();
			//cleanup state
			Deactive();
			buffer.Deactive();
			//save data for draw call
			DrawElementsType drawElementsType = GetDrawElementsType(typeof(TIndex));
			_idData = new IdData(primitiveType, data.Length, drawElementsType);
		}

		public void SetAttribute<TDataElement>(int bindingId, TDataElement[] data, VertexAttribPointerType type, int elementSize, bool perInstance = false) where TDataElement : struct
		{
			if (-1 == bindingId) return; //if attribute not used in shader or wrong name
			Activate();
			var buffer = RequestBuffer(bindingId, BufferTarget.ArrayBuffer);
			buffer.Set(data, BufferUsageHint.StaticDraw);
			//activate for state
			buffer.Activate();
			//set data format
			int elementBytes = Marshal.SizeOf(typeof(TDataElement));
			GL.VertexAttribPointer(bindingId, elementSize, type, false, elementBytes, 0);
			GL.EnableVertexAttribArray(bindingId);
			if (perInstance)
			{
				GL.VertexAttribDivisor(bindingId, 1);
			}
			//cleanup state
			Deactive();
			buffer.Deactive();
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
			var buffer = RequestBuffer(bindingId, BufferTarget.ArrayBuffer);
			// set buffer data
			buffer.Set(data, BufferUsageHint.StaticDraw);
			//activate for state
			buffer.Activate();
			//set data format
			int columnBytes = Marshal.SizeOf(typeof(Vector4));
			int elementBytes = Marshal.SizeOf(typeof(Matrix4));
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

		public void DrawArrays(PrimitiveType type, int count, int start = 0)
		{
			Activate();
			GL.DrawArrays(type, start, count);
			Deactive();
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

		private BufferObject RequestBuffer(int bindingId, BufferTarget bufferTarget)
		{
			BufferObject buffer;
			if (!_boundBuffers.TryGetValue(bindingId, out buffer))
			{
				buffer = new BufferObject(bufferTarget);
				_boundBuffers[bindingId] = buffer;
			}
			return buffer;
		}

		#endregion
	}
}
