#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.IO;
using Boo.Lang;

namespace Boo.IO
{
	[EnumeratorItemType(typeof(string))]
	public class TextReaderEnumerator : System.Collections.IEnumerator, System.Collections.IEnumerable
	{
		TextReader _reader;
		
		string _currentLine;
		
		public TextReaderEnumerator(TextReader reader)
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
			StreamReader sreader = _reader as StreamReader;
			if (null != sreader)
			{
				sreader.BaseStream.Position = 0;
				sreader.DiscardBufferedData();
			}
			else
			{
				throw new NotSupportedException();
			}
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
