namespace Boo.Lang.PatternMatching.Tests
	
class Item:
	public static final Default = Item(Name: "default")
	
	[property(Name)] _name = ""
	[property(Child)] _child as Item