using OpenTK.Graphics.OpenGL;
using System;

namespace GraphicsHelper.GraphicsUtils
{
	/// <summary>
	/// Gl Texture class that allows loading from a file.
	/// </summary>
	public class Texture : IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Texture"/> class.
		/// </summary>
		public Texture()
		{
			//generate one texture and put its ID number into the "m_uTextureID" variable
			GL.GenTextures(1, out m_uTextureID);
			Width = 0;
			Height = 0;
		}

		public void Clamp()
		{
			BeginUse();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
			EndUse();
		}
		public void Repeat()
		{
			BeginUse();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			EndUse();
		}

		public void Dispose()
		{
			GL.DeleteTexture(m_uTextureID);
		}

		/// <summary>
		/// Filters the texture with a point filter.
		/// </summary>
		public void FilterBilinear()
		{
			BeginUse();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 0);
			EndUse();
		}

		public void FilterNearest()
		{
			BeginUse();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 0);
			EndUse();
		}

		/// <summary>
		/// Filters the texture with a tent filter.
		/// </summary>
		public void FilterTrilinear()
		{
			BeginUse();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
			EndUse();
		}

		public void BeginUse()
		{
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D, m_uTextureID);
		}

		public void EndUse()
		{
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.Disable(EnableCap.Texture2D);
		}

		public void BindToUniform(int uniformLocation, TextureUnit textureUnit)
		{
			GL.ActiveTexture(textureUnit);
			BeginUse();
			GL.Uniform1(uniformLocation, textureUnit - TextureUnit.Texture0);
		}

		public void LoadPixels(IntPtr pixels, int width, int height, PixelInternalFormat internalFormat, PixelFormat inputPixelFormat, PixelType type)
		{
			BeginUse();
			GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, inputPixelFormat, type, pixels);
			Width = width;
			Height = height;
			EndUse();
		}

		public static Texture Create(int width, int height, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba8
			, PixelFormat inputPixelFormat = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte)
		{
			var texture = new Texture();
			//create empty texture of given size
			texture.LoadPixels(IntPtr.Zero, width, height, internalFormat, inputPixelFormat, type);
			//set default parameters for filtering and clamping
			texture.FilterBilinear();
			texture.Repeat();
			return texture;
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

		public uint ID { get { return this.m_uTextureID; } }

		private readonly uint m_uTextureID = 0;
	}
}
