#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Tests

import NUnit.Framework
import Boo.IO
import System.IO
import System.Text

[TestFixture]
class TextFileFixture:
	
	[Test]
	def TestReadFile():
		
		fname = WriteTempFile("cabeção", Encoding.Default)		
		Assert.AreEqual("cabeção", TextFile.ReadFile(fname))
		
		fname = WriteTempFile("cabeção", Encoding.UTF8)
		Assert.AreEqual("cabeção", TextFile.ReadFile(fname))		
	
	[Test]
	def TestWriteFile():
		TextFile.WriteFile(fname=Path.GetTempFileName(), "zuçaqüistão")
		Assert.AreEqual("zuçaqüistão", ReadUTF8File(fname))
		
	def ReadUTF8File(fname as string):
		using reader=File.OpenText(fname):
			return reader.ReadToEnd()
			
	def WriteTempFile(content as string, encoding as Encoding) as string:
		using writer=StreamWriter(File.OpenWrite(fname=Path.GetTempFileName()), encoding):
			writer.Write(content)
			return fname
