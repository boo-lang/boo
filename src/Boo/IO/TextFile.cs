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
	}
}
