"""
Boo.Lang.List`1[TestFuncs.MyCallable]
"""
namespace TestFuncs

callable MyCallable(i as int) as MyCallable

class MyClass:
	public list = List[of MyCallable]()
		
print MyClass().list.GetType()
