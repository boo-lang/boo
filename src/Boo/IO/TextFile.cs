using System;
using System.IO;

namespace Boo.IO
{
	public class TextFile : StreamReader, System.Collections.IEnumerable
	{
		public TextFile(string fname) : base(fname)
		{			
		}
		
		public System.Collections.IEnumerator GetEnumerator()
		{
			return new StreamReaderEnumerator(this);
		}
		
		public static string ReadFile(string fname)
		{
			using (StreamReader reader=File.OpenText(fname))
			{
				return reader.ReadToEnd(); 
			}
		}
		
		public static void WriteFile(string fname, string contents)
		{
			using (StreamWriter writer=new StreamWriter(fname, false, System.Text.Encoding.UTF8))
			{
				writer.Write(contents);
			}
		}
	}
}
