namespace BooMacros

import BooSupportingClasses.NestedMacros #for foo2.root

macro foo2.root.x:
	yield [| print "x:${$(foo2.Name)}" |]
	yield

