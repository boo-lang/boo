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
			return new LineEnumerator(this);
		}
		
		class LineEnumerator : System.Collections.IEnumerator, System.Collections.IEnumerable
		{
			TextFile _f;
			
			string _currentLine;
			
			public LineEnumerator(TextFile f)
			{
				_f = f;
			}
			
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}
			
			public void Reset()
			{
				_f.BaseStream.Position = 0;
				_f.DiscardBufferedData();
			}
			
			public bool MoveNext()
			{
				_currentLine = _f.ReadLine();
				return _currentLine != null;
			}
			
			public object Current
			{
				get
				{
					return _currentLine;
				}
			}
		}

	}
}
