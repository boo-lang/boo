
namespace BooCompilerResources.Tests

import NUnit.Framework
import System.IO
import System.Reflection
import System.Resources
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Resources
import Boo.Lang.Compiler.Pipelines

class TestResource(ICompilerResource):
	def WriteResource(service as IResourceService):		
		writer = service.DefineResource("strings1.resources", "")
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
		
	def ReadToEnd(stream as FileStream):
		using reader=StreamReader(stream):
			return reader.ReadToEnd()
		
	def CompileResource(outputAssembly as string, resource as ICompilerResource):
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.OutputType = CompilerOutputType.Library
		parameters.OutputAssembly = MapPath(outputAssembly)
		parameters.Resources.Add(resource)
		parameters.Pipeline = CompileToFile()
		context = compiler.Run()
		Assert.AreEqual(0, len(context.Errors), context.Errors.ToString(true))
		
		asm = Assembly.LoadFrom(parameters.OutputAssembly)
		Assert.IsNotNull(asm, "Assembly must be loadable after Run.")
		return asm
		
	def MapPath(fname as string):
		return Path.Combine(
				Path.GetDirectoryName(typeof(BooCompiler).Assembly.Location),
				fname)
		
	
