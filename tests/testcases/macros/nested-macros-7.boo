#ignore Use of external extension within another external extension does not work yet
"""
>>>
x:foo2
 y:root
<<<
"""
import BooSupportingClasses.NestedMacros #foo foo2 foo2.root are in BooSupportingClasses.dll

/*TODO:
namespace BooMacros #foo2.root.x foo2.root.x.y are in BooMacros.dll
macro foo2.root.x:
	yield [| print "x:${$(foo2.Name)}" |]
	yield

macro foo2.root.x.y:
	yield [| print " y:${$(root.Name)}" |]
	yield
*/

foo2 foo:
	root:
		x:
			y

