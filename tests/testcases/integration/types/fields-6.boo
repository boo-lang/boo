
class Foo:

	public static spam = i*eggs for i in range(3)
	
	public static eggs = 2
	
assert "0 2 4" == join(Foo.spam)
assert 2 == Foo.eggs
Foo.eggs = 3
assert "0 3 6" == join(Foo.spam)
