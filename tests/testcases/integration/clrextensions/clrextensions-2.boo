"""
3
Cedric
"""
import System
import Boo.Lang.Compiler.MetaProgramming

extensionAttribute = Type.GetType("System.Runtime.CompilerServices.ExtensionAttribute, System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
if extensionAttribute is null:
	print "3"
	print "Cedric"
	return

extensionAssembly = extensionAttribute.Assembly
	
library = [|
	namespace ClrExtensions
	
	import System.Collections.Generic
	
	[System.Runtime.CompilerServices.ExtensionAttribute]
	def ItemAt[of T](items as IEnumerable[of T], index as int):
		for item in items:
			if index == 0: return item
			index--
|]

code = [|
	import ClrExtensions
	import System.Collections.Generic
	
	ints = List[of int]((1,2,3))
	strings = List[of string](("Avish", "Bamboo", "Cedric"))
	
	print ints.ItemAt(2)
	print strings.ItemAt(2)
|]
compile(code, compile(library, extensionAssembly), extensionAssembly).EntryPoint.Invoke(null, (null,))

