"""
type = Foo
"""
import Boo.Lang.Compiler

klass = [|
	class Foo:
		pass
|]

code = [| type = $klass |]
print code.ToCodeString()
