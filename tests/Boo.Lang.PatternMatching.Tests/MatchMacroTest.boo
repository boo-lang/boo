namespace Boo.Lang.PatternMatching.Tests

import NUnit.Framework
import Boo.Lang.PatternMatching
	
class Item:
	public static final Default = Item(Name: "default")
	
	[property(Name)] _name = ""
	[property(Child)] _child as Item
	
class Container[of T]:
	public value as T
	
class Collection:
	public Items as List
	
[TestFixture]
class MatchMacroTest:
	
	[Test]
	def TestFixedSizeCollection():
		Assert.AreEqual(1, lastItem(Collection(Items: [1])))
		Assert.AreEqual(2, lastItem(Collection(Items: [1, 2])))

	[Test]
	def TestNestedFixedSize():
		c = Collection(Items: [Collection(Items: [1, 2])])
		Assert.AreEqual(2, nestedLastItem(c))

	[Test]
	def TestFixedSizeCollectionTyped():
		Assert.AreEqual(1, lastIntItem(Collection(Items: [1])))
		Assert.AreEqual(2, lastIntItem(Collection(Items: [1, 2])))

	[Test]
	def TestFixedSizeCollectionLastItemBeforeCatchAll():
		Assert.AreEqual(1, lastIntItemBeforeCatchAll(Collection(Items: [1, "_", "__"])))
		Assert.AreEqual(2, lastIntItemBeforeCatchAll(Collection(Items: [1, 2, "_", "__"])))

	[Test]
	[ExpectedException(MatchError)]
	def TestMatchErrorOnFixedSize():
		lastItem(Collection(Items: [1, 2, 3]))
	
	[Test]
	def TestGenericMatch():
		Assert.AreEqual(42, genericMatch(Container of int(value: 21)))
		Assert.AreEqual("BOO", genericMatch(Container of string(value: 'boo')))
	
	[Test]
	def TestPropertyPattern():
		Assert.AreEqual("item foo", itemByName(Item(Name: "foo")))
		Assert.AreEqual("not foo", itemByName(Item(Name: "not foo")))
		
	[Test]
	def TestNestedPropertyPattern():
		Assert.AreEqual("foo:bar", nestedByName(
								Item(Name: "foo",
									Child: Item(Name: "bar"))))
								
	[Test]
	def TestStringValue():
		for s in ("foo", "bar"):
			Assert.AreEqual("*${s}*", matchStringValue(s))
		

	[Test]
	def TestQualifiedReference():
		Assert.AreEqual("default item", itemByQualifiedReference(Item.Default))
		Assert.AreEqual("foo", itemByQualifiedReference(Item(Name: "foo")))
		
	[Test]
	def TestImplicitPropertyPattern():
		Assert.AreEqual("FOO", itemByImplicitNameReference(Item(Name: "foo")))
											
	[Test]
	[ExpectedException(MatchError)]
	def TestMatchErrorOnPropertyPattern():
		itemByName(42)
		
	[Test]
	def TestMatchErrorMessageIncludesValue():
		try:
			itemByName(42)
		except e as MatchError:
			Assert.AreEqual("`o` (42) failed to match", e.Message)
		
	[Test]
	[ExpectedException(MatchError)]
	def TestMatchErrorOnNestedPropertyPattern():
		nestedByName(42)
		
	[Test]
	def TestCaseCapture():
		Assert.AreEqual("foo", caseCapture(Item(Name: "foo")))
		Assert.AreEqual(42, caseCapture(21))
		
	[Test]
	def TestOtherwise():
		Assert.AreEqual("int", matchIntOtherwise(42))
		Assert.AreEqual("otherwise", matchIntOtherwise("42"))
		
	[Test]
	def TestEitherPattern():
		Assert.AreEqual("yes", either(42))
		Assert.AreEqual("yes", either(-1))
		Assert.AreEqual("no", either(0))
		
	def either(value as int):
		match value:
			case 42 | -1:
				return "yes"
			otherwise:
				return "no"
		
	enum Foo:
		None
		Bar
		Baz
		
	[Test]
	def TestMemberReferenceIsTreatedAsValueComparison():
		Assert.AreEqual("Bar", matchMemberRef(Foo.Bar))
		Assert.AreEqual("Baz", matchMemberRef(Foo.Baz))
		
	[Test]
	[ExpectedException(MatchError)]
	def TestMatchErrorOnMemberReferencePattern():
		matchMemberRef(Foo.None)

	def itemByName(o):
		match o:
			case Item(Name: "foo"):
				return "item foo"
			case Item(Name: name):
				return name
				
	def nestedByName(o):
		match o:
			case Item(Name: outer, Child: Item(Name: inner)):
				return "${outer}:${inner}"
				
	def itemByQualifiedReference(o):
		match o:
			case Item(Name: Item.Default.Name):
				return "default item"
			case Item(Name: name):
				return name
				
	def itemByImplicitNameReference(o):
		match o:
			case Item(Name):
				return Name.ToUpper()
				
	def caseCapture(o):
		match o:
			case i = int():
				return i*2
			case item = Item():
				return item.Name
				
	def matchMemberRef(o as Foo):
		match o:
			case Foo.Bar:
				return "Bar"
			case Foo.Baz:
				return "Baz"
				
	def matchIntOtherwise(o):
		match o:
			case int():
				return "int"
			otherwise:
				return "otherwise"
				
	def matchStringValue(o):
		match o:
			case "foo":
				return "*foo*"
			case "bar":
				return "*bar*"
				
	def genericMatch(o):
		match o:
			case Container[of int](value: i):
				return i*2
			case Container[of string](value: s):
				return s.ToUpper()
				
	def lastItem(o):
		match o:
			case Collection(Items: (last,)):
				return last
			case Collection(Items: (_, last)):
				return last
				
	def nestedLastItem(o):
		match o:
			case Collection(Items: (Collection(Items: (_, last)),)):
				return last

	def lastIntItem(o):
		match o:
			case Collection(Items: (last = int(),)):
				return last
			case Collection(Items: (_ = int(), last = int(),)):
				return last

	def lastIntItemBeforeCatchAll(o):
		match o:
			case Collection(Items: (_ = int(), last = int(), *_)):
				return last
			case Collection(Items: (last = int(), *_)):
				return last
		return -1

