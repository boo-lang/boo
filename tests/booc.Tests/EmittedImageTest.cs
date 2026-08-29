using System.IO;
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
