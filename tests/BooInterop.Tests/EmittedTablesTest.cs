using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using BooInteropLib;
using NUnit.Framework;

namespace BooInterop.Tests;

/// <summary>
/// The metadata tables a Boo assembly is built from.
/// </summary>
/// <remarks>
/// The emitted metadata is copied table by table on its way out of the
/// compiler, and a table the copy forgets loses its rows rather than failing.
/// BooInteropLib is written to populate each of these, so a table that comes
/// back empty says one went missing. Row counts are checked rather than
/// contents: the point is to notice the loss, not to restate the schema.
/// </remarks>
[TestFixture]
public class EmittedTablesTest
{
	private MetadataReader _metadata;
	private PEReader _reader;
	private FileStream _stream;

	[SetUp]
	public void SetUp()
	{
		var path = typeof(Edges).Assembly.Location;
		_stream = File.OpenRead(path);
		_reader = new PEReader(_stream);
		_metadata = _reader.GetMetadataReader();
	}

	[TearDown]
	public void TearDown()
	{
		_reader?.Dispose();
		_stream?.Dispose();
	}

	[TestCase(TableIndex.Constant, "literal fields")]
	[TestCase(TableIndex.CustomAttribute, "attributes")]
	[TestCase(TableIndex.FieldMarshal, "the marshalled P/Invoke parameter")]
	[TestCase(TableIndex.ClassLayout, "the explicit struct layout")]
	[TestCase(TableIndex.FieldLayout, "the overlapping field offsets")]
	[TestCase(TableIndex.StandAloneSig, "local variable signatures")]
	[TestCase(TableIndex.EventMap, "the event")]
	[TestCase(TableIndex.Event, "the event")]
	[TestCase(TableIndex.PropertyMap, "the properties")]
	[TestCase(TableIndex.Property, "the properties")]
	[TestCase(TableIndex.MethodSemantics, "property and event accessors")]
	[TestCase(TableIndex.MethodImpl, "the interface implementations")]
	[TestCase(TableIndex.ModuleRef, "the P/Invoke library")]
	[TestCase(TableIndex.TypeSpec, "the closed generic types")]
	[TestCase(TableIndex.ImplMap, "the P/Invoke declaration")]
	[TestCase(TableIndex.InterfaceImpl, "the implemented interfaces")]
	[TestCase(TableIndex.NestedClass, "the nested types")]
	[TestCase(TableIndex.GenericParam, "the generic parameters")]
	[TestCase(TableIndex.MethodSpec, "the generic method calls")]
	[TestCase(TableIndex.GenericParamConstraint, "the generic constraints")]
	public void TableIsPopulated(TableIndex table, string because)
	{
		Assert.AreNotEqual(0, _metadata.GetTableRowCount(table),
			$"{table} should hold {because}");
	}

	[Test]
	public void NamesNoImplementationAssembly()
	{
		foreach (var handle in _metadata.AssemblyReferences)
			Assert.AreNotEqual("System.Private.CoreLib",
				_metadata.GetString(_metadata.GetAssemblyReference(handle).Name),
				"C# cannot resolve a type through the implementation assembly");
	}

	[Test]
	public void CarriesItsSymbols()
	{
		var entries = _reader.ReadDebugDirectory();

		var types = System.Linq.Enumerable.ToArray(
			System.Linq.Enumerable.Select(entries, e => e.Type));

		CollectionAssert.Contains(types, DebugDirectoryEntryType.CodeView,
			"a reader finds the embedded symbols through the CodeView entry");
		CollectionAssert.Contains(types, DebugDirectoryEntryType.PdbChecksum,
			"a reader matches the symbols against the checksum entry");
		CollectionAssert.Contains(types, DebugDirectoryEntryType.EmbeddedPortablePdb);

		foreach (var entry in entries)
			if (entry.Type == DebugDirectoryEntryType.PdbChecksum)
			{
				var checksum = _reader.ReadPdbChecksumDebugDirectoryData(entry);
				Assert.AreEqual("SHA256", checksum.AlgorithmName);
				Assert.AreEqual(32, checksum.Checksum.Length, "an empty checksum matches nothing");
			}
	}

	[Test]
	public void KeepsAnAttributeArgumentThatIsAnArray()
	{
		var tags = typeof(Edges).GetCustomAttribute<TagsAttribute>();

		Assert.IsNotNull(tags);
		Assert.AreEqual(new[] { "alpha", "beta" }, tags.Names);
	}

	/// <summary>
	/// A System.Type argument to an attribute is a name in a blob rather than a
	/// type reference, so repointing the references does not reach it.
	/// </summary>
	/// <remarks>
	/// Known gap, pinned here rather than fixed. Attribute values are copied
	/// byte for byte because re-encoding one needs the constructor signature to
	/// say which arguments are types, and reading that wrong corrupts every
	/// attribute in the assembly. Reflection resolves the name at runtime, so
	/// only a C# compile-time read of the argument is affected. Delete this
	/// test when the blobs are rewritten.
	/// </remarks>
	[Test]
	public void StillNamesTheImplementationAssemblyInsideAnAttributeBlob()
	{
		Assert.AreEqual(typeof(System.Collections.Generic.Dictionary<string, int>),
			typeof(Annotated).GetCustomAttribute<MarkerAttribute>().Subject,
			"the argument still resolves at runtime");

		Assert.IsTrue(AnyAttributeValueMentions("System.Private.CoreLib"),
			"the blob names the implementation assembly, which C# cannot resolve");
	}

	private bool AnyAttributeValueMentions(string text)
	{
		foreach (var handle in _metadata.CustomAttributes)
		{
			var value = _metadata.GetCustomAttribute(handle).Value;
			if (value.IsNil)
				continue;

			var bytes = _metadata.GetBlobBytes(value);
			if (System.Text.Encoding.UTF8.GetString(bytes).Contains(text))
				return true;
		}

		return false;
	}

	[Test]
	public void KeepsTheGenericConstraint()
	{
		var parameter = typeof(Registry<>).GetGenericArguments()[0];

		CollectionAssert.Contains(parameter.GetGenericParameterConstraints(), typeof(IShape));
	}

	[Test]
	public void KeepsThePInvokeDeclaration()
	{
		var method = typeof(Native).GetMethod("strlen");

		Assert.IsNotNull(method);
		Assert.IsTrue(method.Attributes.HasFlag(MethodAttributes.PinvokeImpl));
	}
}
