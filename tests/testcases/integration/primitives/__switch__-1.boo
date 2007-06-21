def foo(value as int):
	__switch__(value, label1, label2, label3)
	goto end
	:label1
	return 1
	:label2
	return 2
	:label3
	return 3
	:end
	return -1
	
assert 1 == foo(0)
assert 2 == foo(1)
assert 3 == foo(2)
assert -1 == foo(3)
assert -1 == foo(10)
assert 1 == foo(0)
