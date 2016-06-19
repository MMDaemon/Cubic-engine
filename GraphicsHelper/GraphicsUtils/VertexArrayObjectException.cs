using System;

namespace GraphicsHelper.GraphicsUtils
{
	[Serializable]
	public class VertexArrayObjectException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VertexArrayObjectException"/> class.
		/// </summary>
		/// <param name="msg">The error msg.</param>
		public VertexArrayObjectException(string msg) : base(msg) { }
	}
}
