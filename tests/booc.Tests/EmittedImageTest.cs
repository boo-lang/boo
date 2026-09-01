using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using NUnit.Framework;

namespace booc.Tests;

/// <summary>
/// The PE header of what the compiler writes to disk.
/// </summary>
/// <remarks>
/// Windows refuses to load an image whose header omits ExecutableImage; the
/// loader on Linux and macOS does not look. A library missing the flag
/// therefore fails only on Windows, and only once a later compilation
/// references it, so nothing else in the suite catches it.
/// </remarks>
[TestFixture]
public class EmittedImageTest
{
	private string _outputDirectory;

	[SetUp]
	public void SetUp()
	{
		_outputDirectory = Path.Combine(Path.GetTempPath(), "boo-tests", "emitted-image");
		Directory.CreateDirectory(_outputDirectory);
	}

	[TearDown]
	public void TearDown()
	{
		if (Directory.Exists(_outputDirectory))
			Directory.Delete(_outputDirectory, true);
	}

	[Test]
	public void LibraryIsMarkedExecutableImage()
	{
		var characteristics = CharacteristicsOf(
			CompilerOutputType.Library, "lib.dll", "class Foo:\n\tpass\n");

		Assert.IsTrue(characteristics.HasFlag(Characteristics.Dll),
			"a library must be marked as a DLL");
		Assert.IsTrue(characteristics.HasFlag(Characteristics.ExecutableImage),
			"Windows will not load an image without ExecutableImage");
	}

	[Test]
	public void ExecutableIsMarkedExecutableImage()
	{
		var characteristics = CharacteristicsOf(
			CompilerOutputType.ConsoleApplication, "app.exe", "print 'hello'\n");

		Assert.IsTrue(characteristics.HasFlag(Characteristics.ExecutableImage));
		Assert.IsFalse(characteristics.HasFlag(Characteristics.Dll),
			"an executable is not a DLL");
	}

	[Test]
	public void CoreLibraryIsReferencedByItsReferenceName()
	{
		var references = AssemblyReferencesOf("corelib.dll", "class Foo:\n\tdef Bar(s as string) as string:\n\t\treturn s\n");

		// Roslyn will not resolve a type through the implementation assembly, so
		// naming it leaves a Boo assembly unusable from C# at compile time.
		CollectionAssert.DoesNotContain(references, "System.Private.CoreLib");
		CollectionAssert.Contains(references, "System.Runtime");
	}

	[Test]
	public void MarshallingDescriptorsSurviveEmission()
	{
		var source =
			"import System.Runtime.InteropServices\n" +
			"\n" +
			"class Native:\n" +
			"\t[DllImport('libc')]\n" +
			"\tstatic def puts([MarshalAs(UnmanagedType.LPStr)] s as string) as int:\n" +
			"\t\tpass\n";

		// Lose the FieldMarshal table and the call marshals its arguments the
		// default way rather than the declared way, which goes wrong at the
		// call and nowhere earlier.
		Assert.AreNotEqual(0, RowCountOf("marshal.dll", source, TableIndex.FieldMarshal));
	}

	private int RowCountOf(string fileName, string source, TableIndex table)
	{
		var output = Path.Combine(_outputDirectory, fileName);

		var compiler = new BooCompiler();
		compiler.Parameters.Input.Add(new StringInput(fileName, source));
		compiler.Parameters.Pipeline = new CompileToFile();
		compiler.Parameters.OutputType = CompilerOutputType.Library;
		compiler.Parameters.OutputAssembly = output;

		var result = compiler.Run();
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString());

		using (var stream = File.OpenRead(output))
		using (var reader = new PEReader(stream))
			return reader.GetMetadataReader().GetTableRowCount(table);
	}

	private string[] AssemblyReferencesOf(string fileName, string source)
	{
		var output = Path.Combine(_outputDirectory, fileName);

		var compiler = new BooCompiler();
		compiler.Parameters.Input.Add(new StringInput(fileName, source));
		compiler.Parameters.Pipeline = new CompileToFile();
		compiler.Parameters.OutputType = CompilerOutputType.Library;
		compiler.Parameters.OutputAssembly = output;

		var result = compiler.Run();
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString());

		using (var stream = File.OpenRead(output))
		using (var reader = new PEReader(stream))
		{
			var metadata = reader.GetMetadataReader();
			return metadata.AssemblyReferences
				.Select(h => metadata.GetString(metadata.GetAssemblyReference(h).Name))
				.ToArray();
		}
	}

	private Characteristics CharacteristicsOf(CompilerOutputType outputType, string fileName, string source)
	{
		var output = Path.Combine(_outputDirectory, fileName);

		var compiler = new BooCompiler();
		compiler.Parameters.Input.Add(new StringInput(fileName, source));
		compiler.Parameters.Pipeline = new CompileToFile();
		compiler.Parameters.OutputType = outputType;
		compiler.Parameters.OutputAssembly = output;

		var result = compiler.Run();
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString());

		using (var stream = File.OpenRead(output))
		using (var reader = new PEReader(stream))
			return reader.PEHeaders.CoffHeader.Characteristics;
	}
}
