using System;
using System.IO;

namespace Boo.IO
{
	public class StreamReaderEnumerator : System.Collections.IEnumerator, System.Collections.IEnumerable
	{
		StreamReader _reader;
		
		string _currentLine;
		
		public StreamReaderEnumerator(StreamReader reader)
		{
			if (null == reader)
			{
				throw new ArgumentNullException("reader");
			}
			_reader = reader;
		}
		
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this;
		}
		
		public void Reset()
		{
			_reader.BaseStream.Position = 0;
			_reader.DiscardBufferedData();
		}
		
		public bool MoveNext()
		{
			_currentLine = _reader.ReadLine();
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
