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

namespace BooCompilerResources.Tests

import NUnit.Framework
import System.IO
import System.Reflection
import System.Resources
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Resources
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class TestResource(ICompilerResource):
	Name:
		get:
			return "strings1.resources"
	
	Description:
		get:
			return ""
			
	def WriteResources(writer as IResourceWriter):
		writer.AddResource("message", "Hello, world!")

[TestFixture]
class CompilerResourcesFixture:
	
	[Test]
	def CustomResource():
		asm = CompileResource("CustomResource.dll", TestResource())		
		resources = ResourceManager("strings1", asm)
		Assert.AreEqual("Hello, world!", resources.GetString("message"))
		
	[Test]
	def EmbeddedFileResource():
		fname = MapPath("fileresource.resources")
		using writer=ResourceWriter(fname):
			writer.AddResource("message", "Hello from file!")
			writer.AddResource("list", [1, 2, 3])
			
		asm = CompileResource("FileResource.dll", FileResource(fname))
		resources = ResourceManager("fileresource", asm)
		Assert.AreEqual("Hello from file!", resources.GetString("message"))
		Assert.AreEqual([1, 2, 3], resources.GetObject("list"))
		
	def CompileResource(outputAssembly as string, resource as ICompilerResource):
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.OutputType = CompilerOutputType.Library
		parameters.OutputAssembly = MapPath(outputAssembly)
		parameters.Resources.Add(resource)
		parameters.Pipeline = CompileToFile()
		context = compiler.Run()
		Assert.AreEqual(0, len(context.Errors), context.Errors.ToString())
		
		asm = Assembly.LoadFrom(parameters.OutputAssembly)
		Assert.IsNotNull(asm, "Assembly must be loadable after Run.")
		return asm
		
	def MapPath(fname as string):
		return Path.Combine(
				Path.GetDirectoryName(typeof(BooCompiler).Assembly.Location),
				fname)
		
	
