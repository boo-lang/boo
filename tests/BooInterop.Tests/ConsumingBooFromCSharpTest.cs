using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooInteropLib;
using NUnit.Framework;

namespace BooInterop.Tests;

/// <summary>
/// C# declaring, constructing, calling and deriving from Boo types.
/// </summary>
/// <remarks>
/// Roslyn resolves a type through the assembly the reference names, so an
/// emitted assembly naming the implementation core library can be named from
/// C# but nothing more. Every test here is a compile-time reference to Boo
/// metadata, which is what nothing else in the suite has: the one existing
/// use is a typeof, and naming a type is exactly the part that keeps working
/// when the references are wrong.
/// </remarks>
[TestFixture]
public class ConsumingBooFromCSharpTest
{
	/// <summary>A C# type deriving from a Boo abstract class.</summary>
	private sealed class Square : Shape
	{
		public double Side;

		public override double Area()
		{
			return Side * Side;
		}
	}

	/// <summary>A C# type with no Boo relationship, for Boo to duck type.</summary>
	private sealed class Talker
	{
		public string Speak()
		{
			return "csharp";
		}
	}

	[Test]
	public void ConstructsAndCallsABooClass()
	{
		var circle = new Circle(2);

		Assert.AreEqual(Math.PI * 4, circle.Area(), 1e-12);
		Assert.AreEqual("Circle:12.566370614359172", circle.Describe());
	}

	[Test]
	public void DerivesFromABooAbstractClass()
	{
		var square = new Square { Side = 3 };

		Assert.AreEqual(9, square.Area());
		Assert.AreEqual("Square:9", square.Describe());
		Assert.IsInstanceOf<IShape>(square, "the Boo interface comes with the Boo base class");
	}

	[Test]
	public void ClosesABooGenericOverATypeItsConstraintAllows()
	{
		var registry = new Registry<IShape>();
		registry.Add(new Circle(1));
		registry.Add(new Square { Side = 2 });

		Assert.AreEqual(2, registry.Count);
		Assert.AreEqual(Math.PI + 4, registry.Total(), 1e-12);
	}

	[Test]
	public void CallsABooGenericMethodWithAConstraint()
	{
		Assert.AreEqual(5, Edges.Largest(new[] { 1, 5, 3 }));
		Assert.AreEqual("pear", Edges.Largest(new[] { "apple", "pear", "fig" }));
	}

	[Test]
	public void UsesABooIndexerPropertyAndEvent()
	{
		var edges = new Edges();
		edges.Fill("first", "second");

		Assert.AreEqual("edges", edges.Label);
		Assert.AreEqual("second", edges[1]);

		edges[1] = "changed";
		Assert.AreEqual("changed", edges[1]);

		var heard = new List<string>();
		edges.Ping += heard.Add;
		edges.Fire("ping");
		Assert.AreEqual(new[] { "ping" }, heard);
	}

	[Test]
	public void UsesABooOperatorOverload()
	{
		var sum = new Vector(1, 2) + new Vector(30, 40);

		Assert.AreEqual("(31,42)", sum.ToString());
	}

	[Test]
	public void CallsABooVarargsAndByRefMethod()
	{
		Assert.AreEqual("a-b-c", Edges.Join("a", "b", "c"));

		var left = 1;
		var right = 2;
		Edges.Swap(ref left, ref right);
		Assert.AreEqual(2, left);
		Assert.AreEqual(1, right);
	}

	[Test]
	public void CallsBooMembersTypedByOtherImplementationAssemblies()
	{
		// Uri is defined by System.Private.Uri and XmlDocument by
		// System.Private.Xml. Naming either in a signature is enough to make
		// the member unusable from C# when the references are not repointed.
		var edges = new Edges();

		Assert.AreEqual("example.com", edges.HostOf(new Uri("https://example.com/x")));
		Assert.AreEqual("root", edges.Document().DocumentElement.Name);
	}

	[Test]
	public void EnumeratesABooGenerator()
	{
		Assert.AreEqual(new[] { 3, 2, 1 }, new Edges().Countdown(3).ToArray());
	}

	[Test]
	public async Task AwaitsABooAsyncMethod()
	{
		Assert.AreEqual(42, await new Edges().DelayedAsync(21));
	}

	[Test]
	public void ReadsBooMultiDimensionalAndJaggedArrays()
	{
		var grid = new Edges().Grid();
		Assert.AreEqual(2, grid.Rank);
		Assert.AreEqual(9, grid[1, 2]);

		var jagged = new Edges().Jagged();
		Assert.AreEqual(2, jagged.Length);
		Assert.AreEqual(new[] { 3 }, jagged[1]);
	}

	[Test]
	public void ReadsBooLiteralFields()
	{
		Assert.AreEqual(42, Edges.Answer);
		Assert.AreEqual(3.14, Edges.Pi);
		Assert.AreEqual('B', Edges.Letter);
	}

	[Test]
	public void UsesABooNestedType()
	{
		Assert.AreEqual("nested", new Edges.Nested().Ping());
	}

	[Test]
	public void UsesABooStructWithAnExplicitLayout()
	{
		var overlay = new Overlay { AsBytes = 0 };
		overlay.AsInt = 7;

		Assert.AreEqual(7, overlay.AsBytes, "the two fields share an offset");
	}

	[Test]
	public void UsesABooEnumAndCallableType()
	{
		Assert.AreEqual(Kind.Flat, (Kind) 2);

		Transform shout = text => text.ToUpper();
		Assert.AreEqual("HI", shout("hi"));
	}

	[Test]
	public void CallsABooExtensionMethodStatically()
	{
		Assert.AreEqual("HI!", Extensions.Shout("hi"));
	}

	[Test]
	public void DuckTypesACSharpObjectFromBoo()
	{
		// The Boo side takes object, assigns it to a duck, and calls Speak.
		// Nothing in the C# type is known to Boo at compile time.
		Assert.AreEqual("csharp", Ducks.Speak(new Talker()));
	}

	[Test]
	public void DispatchesThroughIQuackFu()
	{
		var bag = new Bag();
		bag.QuackSet("key", null, "value");

		Assert.AreEqual("value", bag.QuackGet("key", null));
		Assert.AreEqual("Whatever/2", Ducks.Poke(bag));
	}

	[Test]
	public void OmitsABooDeclaredOptionalArgument()
	{
		// The default has to reach metadata, not just Boo's own call sites, or
		// no other language can leave the argument out.
		Assert.AreEqual("Hi, Jude!", Defaults.Greet("Jude"));
		Assert.AreEqual("Hey, Jude!", Defaults.Greet("Jude", "Hey"));
		Assert.AreEqual(6, Defaults.Sum(1));
		Assert.AreEqual(9, Defaults.Sum(1, 5));
	}

	[Test]
	public void NamesABooDeclaredParameter()
	{
		Assert.AreEqual("Yo, Jude!", Defaults.Greet(greeting: "Yo", name: "Jude"));
		Assert.AreEqual(8, Defaults.Sum(1, c: 5));
	}

	[Test]
	public void CallsThroughABooPInvokeDeclaration()
	{
		// The marshalling descriptor is the whole point: lose it and the string
		// goes over as UTF-16 and the length comes back wrong or the call dies.
		Assert.AreEqual(5, Native.Length("hello"));
	}
}
