namespace BooSupportingClasses.NestedMacros

macro foo:
	yield

macro foo2:
	macro root:
		yield [| print ">>>" |]
		yield
		yield [| print "<<<" |]
	yield

