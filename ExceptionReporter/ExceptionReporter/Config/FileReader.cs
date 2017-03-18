using System.IO;

namespace ExceptionReporting.Config
{
	/// <summary>generic interface to reading a file</summary>
	public interface IFileReader
	{
		/// <summary>read all contents of a file</summary>
		string ReadAll(string fileName);
	}

	internal class FileReader : IFileReader
	{
		public string ReadAll(string fileName)
		{
			return File.ReadAllText(fileName);
		}
	}
}
