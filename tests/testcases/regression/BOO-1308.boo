"""
ok
"""
namespace regression

import Boo.Lang.Compiler
import Boo.Lang.Compiler.MetaProgramming

class Entity:
	public node = "ok"

code = [|
	import regression
	
	class XXX:
		def constructor():
			def find(e as Entity):
				return e.node
			print find(Entity())
			
	XXX()
|]

compile(code, typeof(Entity).Assembly).GetEntryPoint().Invoke(null, (null,))

