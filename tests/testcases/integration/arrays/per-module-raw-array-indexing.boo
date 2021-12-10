"""
it worked!
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.MetaProgramming

module = [|
	namespace Test
	
	a = (1, 2, 3)
	print a[-1]
|]

AstAnnotations.MarkRawArrayIndexing(module)

try:
	compile(module).GetEntryPoint().Invoke(null, (null,))
except x:
	assert x.InnerException isa System.IndexOutOfRangeException
	print "it worked!"
	
