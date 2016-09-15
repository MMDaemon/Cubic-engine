using System;
using OpenTK.Graphics.OpenGL;

namespace GraphicsHelper.ShaderUtils
{
	/// <summary>
	/// Shader class
	/// </summary>
	public class Shader : IDisposable
	{
		public bool IsLinked { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Shader"/> class.
		/// </summary>
		public Shader()
		{
			_mProgramId = GL.CreateProgram();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (0 != _mProgramId)
			{
				GL.DeleteProgram(_mProgramId);
			}
		}

		public void Compile(string sShader, ShaderType type)
		{
			IsLinked = false;
			int shaderObject = GL.CreateShader(type);
			if (0 == shaderObject) throw new ShaderException(type.ToString(), "Could not create shader object", string.Empty, sShader);
			// Compile vertex shader
			GL.ShaderSource(shaderObject, sShader);
			GL.CompileShader(shaderObject);
			int statusCode;
			GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out statusCode);
			if (1 != statusCode)
			{
				throw new ShaderException(type.ToString(), "Error compiling shader", GL.GetShaderInfoLog(shaderObject), sShader);
			}
			GL.AttachShader(_mProgramId, shaderObject);
			//shaderIDs.Add(shaderObject);
		}

		/// <summary>
		/// Begins this shader use.
		/// </summary>
		public void Begin()
		{
			GL.UseProgram(_mProgramId);
		}

		/// <summary>
		/// Ends this shader use.
		/// </summary>
		public void End()
		{
			GL.UseProgram(0);
		}

		public int GetAttributeLocation(string name)
		{
			return GL.GetAttribLocation(_mProgramId, name);
		}

		public int GetUniformLocation(string name)
		{
			return GL.GetUniformLocation(_mProgramId, name);
		}

		public void Link()
		{
			try
			{
				GL.LinkProgram(_mProgramId);
			}
			catch (Exception)
			{
				throw new ShaderException("Link", "Unknown error!", string.Empty, string.Empty);
			}
			int statusCode;
			GL.GetProgram(_mProgramId, GetProgramParameterName.LinkStatus, out statusCode);
			if (1 != statusCode)
			{
				throw new ShaderException("Link", "Error linking shader", GL.GetProgramInfoLog(_mProgramId), string.Empty);
			}
			IsLinked = true;
		}

		private readonly int _mProgramId;
	}
}
