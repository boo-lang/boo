
class Foo:

	public spam = i*eggs for i in range(3)
	
	public eggs = 2
	
f = Foo()

assert "0 2 4" == join(f.spam)
assert 2 == f.eggs
f.eggs = 3
assert "0 3 6" == join(f.spam)
