using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using BooInteropLib;
using NUnit.Framework;

namespace BooInterop.Tests;

/// <summary>
/// The symbols of a Boo assembly belong to the methods they say they do.
/// </summary>
/// <remarks>
/// A portable PDB finds a method's symbols by the method's row number, and the
/// framework only writes a row for a method with a body. Each interface, abstract
/// or extern method then moves the symbols of every method after it onto the
/// method before, so that stepping into one lands in another method, in another
/// file. The compiler gives the bodiless methods rows of their own; this checks
/// that it does.
/// </remarks>
[TestFixture]
public class DebugInformationTest
{
	private PEReader _image;
	private MetadataReader _metadata;
	private MetadataReader _symbols;

	[SetUp]
	public void SetUp()
	{
		_image = new PEReader(File.OpenRead(typeof(Edges).Assembly.Location));
		_metadata = _image.GetMetadataReader();
		var entry = _image.ReadDebugDirectory().Single(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
		_symbols = _image.ReadEmbeddedPortablePdbDebugDirectoryData(entry).GetMetadataReader();
	}

	[TearDown]
	public void TearDown()
	{
		_image?.Dispose();
	}

	[Test]
	public void EveryMethodHasARowOfSymbols()
	{
		Assert.AreEqual(_metadata.MethodDefinitions.Count, _symbols.MethodDebugInformation.Count);
	}

	[Test]
	public void AMethodWithoutABodyHasNoSequencePoints()
	{
		var bodyless = _metadata.MethodDefinitions
			.Where(h => _metadata.GetMethodDefinition(h).RelativeVirtualAddress == 0)
			.ToList();
		Assert.IsNotEmpty(bodyless, "BooInteropLib should have interface or abstract methods, or this tests nothing");

		foreach (var handle in bodyless)
		{
			var points = _symbols.GetMethodDebugInformation(handle.ToDebugInformationHandle()).GetSequencePoints().ToList();
			Assert.IsEmpty(points, $"{Describe(handle)} has no body, but has sequence points");
		}
	}

	[Test]
	public void AMethodAfterTheBodilessOnesKeepsItsOwnSymbols()
	{
		var firstBodyless = _metadata.MethodDefinitions
			.First(h => _metadata.GetMethodDefinition(h).RelativeVirtualAddress == 0);

		// Every method after the first without a body is one that would have been moved.
		var later = _metadata.MethodDefinitions
			.Where(h => MetadataTokens.GetRowNumber(h) > MetadataTokens.GetRowNumber(firstBodyless))
			.Where(h => _metadata.GetMethodDefinition(h).RelativeVirtualAddress != 0);

		foreach (var handle in later)
		{
			var information = _symbols.GetMethodDebugInformation(handle.ToDebugInformationHandle());
			// The generated constructors and the like have no source, so a method without points is allowed;
			// one that has them must say it came from the source files of this assembly.
			foreach (var point in information.GetSequencePoints().Where(p => !p.IsHidden))
			{
				var document = _symbols.GetString(_symbols.GetDocument(point.Document).Name);
				StringAssert.EndsWith(".boo", document, $"{Describe(handle)} points into {document}");
			}
		}
	}

	private string Describe(MethodDefinitionHandle handle)
	{
		var method = _metadata.GetMethodDefinition(handle);
		var type = _metadata.GetTypeDefinition(method.GetDeclaringType());
		return $"{_metadata.GetString(type.Name)}.{_metadata.GetString(method.Name)}";
	}
}
