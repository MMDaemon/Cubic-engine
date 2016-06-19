using System;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Text.RegularExpressions;

namespace GraphicsHelper.ShaderUtils
{
	public static class ShaderLoader
	{
		/// <summary>
		/// Compiles and links vertex and fragment shaders from strings.
		/// </summary>
		/// <param name="sVertexShd">The s vertex SHD_.</param>
		/// <param name="sFragmentShd">The s fragment SHD_.</param>
		/// <returns>a new instance</returns>
		public static Shader FromStrings(string sVertexShd, string sFragmentShd)
		{
			Shader shd = new Shader();
			try
			{
				shd.Compile(sVertexShd, ShaderType.VertexShader);
				shd.Compile(sFragmentShd, ShaderType.FragmentShader);
				shd.Link();
			}
			catch (Exception e)
			{
				shd.Dispose();
				throw e;
			}
			return shd;
		}

		/// <summary>
		/// Compiles and links vertex and fragment shaders from files.
		/// </summary>
		/// <param name="sVertexShdFile">The s vertex SHD file_.</param>
		/// <param name="sFragmentShdFile">The s fragment SHD file_.</param>
		/// <returns>a new instance</returns>
		public static Shader FromFiles(string sVertexShdFile, string sFragmentShdFile)
		{

			string sVertexShd = ShaderStringFromFileWithIncludes(sVertexShdFile);
			string sFragmentShd = ShaderStringFromFileWithIncludes(sFragmentShdFile);
			return FromStrings(sVertexShd, sFragmentShd);
		}

		/// <summary>
		/// Reads the contents of a file into a string
		/// </summary>
		/// <param name="shaderFile">path to the shader file</param>
		/// <returns>string with contents of shaderFile</returns>
		public static string ShaderStringFromFileWithIncludes(string shaderFile)
		{
			string sShader = null;
			if (!File.Exists(shaderFile))
			{
				throw new FileNotFoundException("Could not find shader file '" + shaderFile + "'");
			}
			sShader = File.ReadAllText(shaderFile);

			//handle includes
			string sCurrentPath = Path.GetDirectoryName(shaderFile) + Path.DirectorySeparatorChar; // get path to current shader
			string sName = Path.GetFileName(shaderFile);
			//split into lines
			var lines = sShader.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var pattern = @"\s*#include\s+" + '"' + "(.+)" + '"';
			int lineNr = 1;
			foreach (var line in lines)
			{
				// Search for include pattern (e.g. #include raycast.glsl) (nested not supported)
				foreach (Match match in Regex.Matches(line, pattern, RegexOptions.Singleline))
				{
					string sFullMatch = match.Value;
					string sIncludeFileName = match.Groups[1].ToString(); // get the filename to include
					string sIncludePath = sCurrentPath + sIncludeFileName; // build path to file

					if (!File.Exists(sIncludePath))
					{
						throw new FileNotFoundException("Could not find include-file '" + sIncludeFileName + "' for shader '" + shaderFile + "'.");
					}
					string sIncludeShd = File.ReadAllText(sIncludePath); // read include as string
					using (var shader = new Shader())
					{
						try
						{
							shader.Compile(sIncludeShd, ShaderType.FragmentShader); //test compile include shader
						}
						catch (ShaderException e)
						{
							throw new ShaderException(e.Type,
								"include compile '" + sIncludePath + "'",
								e.Log, sIncludeShd);
						}
					}
					sIncludeShd += Environment.NewLine + "#line " + lineNr.ToString() + Environment.NewLine;
					sShader = sShader.Replace(sFullMatch, sIncludeShd); // replace #include with actual include
				}
				++lineNr;
			}
			return sShader;
		}
	}
}
