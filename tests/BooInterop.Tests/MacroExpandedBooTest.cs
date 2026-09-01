using System.Collections.Generic;
using BooInteropLib;
using NUnit.Framework;

namespace BooInterop.Tests;

/// <summary>
/// Boo code the compiler produced by running macros, consumed from C#.
/// </summary>
/// <remarks>
/// Macros run inside the compiler, out of assemblies the compiler itself
/// emitted, so a macro that still expands is evidence that an emitted image
/// loads and executes as well as it compiles.
/// </remarks>
[TestFixture]
public class MacroExpandedBooTest
{
	[Test]
	public void ReadsAPropertyAMacroGenerated()
	{
		Assert.AreEqual("macro", new Macros().Title);
	}

	[Test]
	public void MatchesStructurally()
	{
		var macros = new Macros();

		Assert.AreEqual("origin", macros.Classify(new Vector(0, 0)));
		Assert.AreEqual("vector:3,4", macros.Classify(new Vector(3, 4)));
		Assert.AreEqual("greeting", macros.Classify("hello"));
		Assert.AreEqual("other", macros.Classify(17));
	}

	[Test]
	public void RunsTheLockAndCheckedAndUsingMacros()
	{
		Assert.AreEqual(8, new Macros().Guarded(new List<string> { "abc", "de", "fgh" }));
		Assert.AreEqual(6, Macros.Sum(new[] { 1, 2, 3 }));
		Assert.AreEqual("first", Macros.Read("first\nsecond"));
	}
}
