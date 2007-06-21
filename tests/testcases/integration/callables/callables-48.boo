def foo():
	pass
	
// only a single definition for BeginInvoke should be ever
// emitted for compatibility with ms.net 2.0

type = foo.GetType()
methods = type.GetMember("BeginInvoke")
assert 1 == len(methods), "Expected: 1, Actual: ${len(methods)}"
