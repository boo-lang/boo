class Foo:
	
	[property(Version)]
	static _version = "1.0"
	
	def GetVersion():
	"""instance method accessing static property"""
		return Version

f = Foo()

assert "1.0" == Foo.Version
assert "1.0" == f.GetVersion()
assert "1.0" == f.Version

