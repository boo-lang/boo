"""
object: System.Object
IFoo: Foo
IBar: Bar
"""
import System.Collections

interface IFoo:
	pass
	
interface IBar(IFoo):
	pass
	
class Foo(IFoo):
	pass
	
class Bar(IBar):
	pass

def use(obj):
	print("object: ${obj}")

def use(obj as IFoo):
	print("IFoo: ${obj}")

def use(obj as IBar):
	print("IBar: ${obj}")

use(object())
use(Foo())
use(Bar())
