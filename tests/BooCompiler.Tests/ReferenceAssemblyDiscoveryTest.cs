using System.IO;
using System.Reflection.Metadata.Ecma335;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Steps;
using NUnit.Framework;

namespace BooCompiler.Tests;

/// <summary>
/// Finding the reference pack, and what emission does without one.
/// </summary>
/// <remarks>
/// A machine that can run these tests has a pack and never takes the fallback,
/// so the trees here are fabricated to reach the cases that otherwise only
/// appear on a runtime-only install or one patched past its SDK.
/// </remarks>
[TestFixture]
public class ReferenceAssemblyDiscoveryTest
{
	private string _root;

	[SetUp]
	public void SetUp()
	{
		_root = Path.Combine(
			Path.GetTempPath(), "boo-tests", "reference-pack", Path.GetRandomFileName());
		Directory.CreateDirectory(_root);
	}

	[TearDown]
	public void TearDown()
	{
		if (Directory.Exists(_root))
			Directory.Delete(_root, true);
	}

	[Test]
	public void PrefersThePackMatchingTheRuntime()
	{
		Pack("10.0.5");
		Pack("10.0.11");

		Assert.AreEqual(Framework("10.0.11"), DirectoryFor("10.0.11"));
	}

	[Test]
	public void FallsBackToTheNewestOlderPatch()
	{
		Pack("10.0.5");
		Pack("10.0.8");

		Assert.AreEqual(Framework("10.0.8"), DirectoryFor("10.0.11"));
	}

	[Test]
	public void IgnoresAPackNewerThanTheRuntime()
	{
		Pack("10.0.20");

		Assert.IsNull(DirectoryFor("10.0.11"));
	}

	[Test]
	public void IgnoresAPackFromAnotherRelease()
	{
		Pack("9.0.11");

		Assert.IsNull(DirectoryFor("10.0.11"),
			"a reference assembly of the wrong release names the wrong framework");
	}

	[Test]
	public void FindsNothingWhenNoPackIsInstalled()
	{
		Assert.IsNull(DirectoryFor("10.0.11"));
	}

	[Test]
	public void WarnsAndLeavesTheMetadataAloneWithoutAnIndex()
	{
		var context = new CompilerContext();
		var metadata = new MetadataBuilder();

		Assert.AreSame(metadata, ImplementationReferences.Resolved(context, metadata, null),
			"with nothing to rewrite the image goes out as it was generated");
		Assert.AreEqual(1, context.Warnings.Count);
		Assert.AreEqual("BCW0032", context.Warnings[0].Code);
	}

	private string DirectoryFor(string runtime)
	{
		return ReferenceAssemblyIndex.ReferenceDirectory(CoreLibrary(runtime));
	}

	/// <summary>Where the shared framework puts the implementation assembly.</summary>
	private string CoreLibrary(string version)
	{
		var directory = Path.Combine(_root, "shared", "Microsoft.NETCore.App", version);
		Directory.CreateDirectory(directory);
		return Path.Combine(directory, "System.Private.CoreLib.dll");
	}

	/// <summary>The one target framework directory a pack holds.</summary>
	private string Framework(string version)
	{
		return Path.Combine(
			_root, "packs", "Microsoft.NETCore.App.Ref", version, "ref", "net10.0");
	}

	private void Pack(string version)
	{
		Directory.CreateDirectory(Framework(version));
	}
}
