namespace booc.Tests

import System.IO
import System.Reflection.Metadata
import System.Reflection.Metadata.Ecma335
import System.Reflection.PortableExecutable
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

[TestFixture]
class EmittedImageTest:
	"""
	The PE header of what the compiler writes to disk.

	Windows refuses to load an image whose header omits ExecutableImage; the
	loader on Linux and macOS does not look. A library missing the flag
	therefore fails only on Windows, and only once a later compilation
	references it, so nothing else in the suite catches it.
	"""

	_outputDirectory as string

	[SetUp]
	def SetUp():
		_outputDirectory = Path.Combine(Path.GetTempPath(), "boo-tests", "emitted-image")
		Directory.CreateDirectory(_outputDirectory)

	[TearDown]
	def TearDown():
		if Directory.Exists(_outputDirectory):
			Directory.Delete(_outputDirectory, true)

	[Test]
	def LibraryIsMarkedExecutableImage():
		characteristics = CharacteristicsOf(CompilerOutputType.Library, "lib.dll", "class Foo:\n\tpass\n")

		Assert.IsTrue(characteristics.HasFlag(Characteristics.Dll), "a library must be marked as a DLL")
		Assert.IsTrue(characteristics.HasFlag(Characteristics.ExecutableImage), "Windows will not load an image without ExecutableImage")

	[Test]
	def ExecutableIsMarkedExecutableImage():
		characteristics = CharacteristicsOf(CompilerOutputType.ConsoleApplication, "app.exe", "print 'hello'\n")

		Assert.IsTrue(characteristics.HasFlag(Characteristics.ExecutableImage))
		Assert.IsFalse(characteristics.HasFlag(Characteristics.Dll), "an executable is not a DLL")

	[Test]
	def CoreLibraryIsReferencedByItsReferenceName():
		references = AssemblyReferencesOf("corelib.dll", "class Foo:\n\tdef Bar(s as string) as string:\n\t\treturn s\n")

		# Roslyn will not resolve a type through the implementation assembly, so
		# naming it leaves a Boo assembly unusable from C# at compile time.
		CollectionAssert.DoesNotContain(references, "System.Private.CoreLib")
		CollectionAssert.Contains(references, "System.Runtime")

	[Test]
	def MarshallingDescriptorsSurviveEmission():
		source = (
			"import System.Runtime.InteropServices\n" +
			"\n" +
			"class Native:\n" +
			"\t[DllImport('libc')]\n" +
			"\tstatic def puts([MarshalAs(UnmanagedType.LPStr)] s as string) as int:\n" +
			"\t\tpass\n")

		# Lose the FieldMarshal table and the call marshals its arguments the
		# default way rather than the declared way, which goes wrong at the
		# call and nowhere earlier.
		Assert.AreNotEqual(0, RowCountOf("marshal.dll", source, TableIndex.FieldMarshal))

	def RowCountOf(fileName as string, source as string, table as TableIndex) as int:
		using stream = File.OpenRead(Compile(CompilerOutputType.Library, fileName, source)), \
				reader = PEReader(stream):
			return reader.GetMetadataReader().GetTableRowCount(table)

	def AssemblyReferencesOf(fileName as string, source as string) as (string):
		using stream = File.OpenRead(Compile(CompilerOutputType.Library, fileName, source)), \
				reader = PEReader(stream):
			metadata = reader.GetMetadataReader()
			names = []
			for handle in metadata.AssemblyReferences:
				names.Add(metadata.GetString(metadata.GetAssemblyReference(handle).Name))
			return array(string, names)

	def CharacteristicsOf(outputType as CompilerOutputType, fileName as string, source as string) as Characteristics:
		using stream = File.OpenRead(Compile(outputType, fileName, source)), \
				reader = PEReader(stream):
			return reader.PEHeaders.CoffHeader.Characteristics

	def Compile(outputType as CompilerOutputType, fileName as string, source as string) as string:
		output = Path.Combine(_outputDirectory, fileName)

		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput(fileName, source))
		compiler.Parameters.Pipeline = CompileToFile()
		compiler.Parameters.OutputType = outputType
		compiler.Parameters.OutputAssembly = output

		result = compiler.Run()
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString())
		return output
