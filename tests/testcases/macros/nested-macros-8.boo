"""
SPAM! SPAM! SPAM!
"""
namespace Services

import Boo.Lang.PatternMatching
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

macro service:
	case [| service $name |]:
		yield [|
			class $name:
				def constructor():
					$(service.Body)
		|]
		
macro service.onMessage:
	case [| onMessage $name |]:
		yield [|
			def $name():
				$(onMessage.Body)
		|]
		
macro service.onMessageRec:
	case [| onMessageRec $name |]:
		yield [|
			onMessage $name:
				$(onMessageRec.Body)
		|]
		
spam = [|
	import Services
	
	service Spam:
		onMessageRec Go:
			print "SPAM! " * 3
|]

eggs = [|
	import Services
	
	service Eggs:
		pass
|]
		

serviceType = compile(CompileUnit(spam, eggs), typeof(ServiceMacro).Assembly).GetType("Spam")
service as duck = serviceType()
service.Go()
