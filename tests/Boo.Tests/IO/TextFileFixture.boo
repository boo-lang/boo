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

namespace Boo.Lang.Tests

import NUnit.Framework
import Boo.IO
import System.IO
import System.Text

[TestFixture]
class TextFileFixture:
	
	[Test]
	def TestReadFile():
		
		fname = WriteTempFile("cabeÃ§Ã£o", Encoding.Default)		
		Assert.AreEqual("cabeÃ§Ã£o", TextFile.ReadFile(fname))
		
		fname = WriteTempFile("cabeÃ§Ã£o", Encoding.UTF8)
		Assert.AreEqual("cabeÃ§Ã£o", TextFile.ReadFile(fname))		
	
	[Test]
	def TestWriteFile():
		TextFile.WriteFile(fname=Path.GetTempFileName(), "zuÃ§aqÃ¼istÃ£o")
		Assert.AreEqual("zuÃ§aqÃ¼istÃ£o", ReadUTF8File(fname))
		
	def ReadUTF8File(fname as string):
		using reader=File.OpenText(fname):
			return reader.ReadToEnd()
			
	def WriteTempFile(content as string, encoding as Encoding) as string:
		using writer=StreamWriter(File.OpenWrite(fname=Path.GetTempFileName()), encoding):
			writer.Write(content)
			return fname
