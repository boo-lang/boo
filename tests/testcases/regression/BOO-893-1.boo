"""
Foo.ctor(crunch)
Bar.Baz
Foo.Baz
"""
import System

class Foo(IQuackFu):
	def constructor(s as string):
		print "Foo.ctor(${s})"
		
	def QuackGet(s as string, args as (object)):
		pass
		
	def QuackSet(s as string, args as (object), v as object):
		pass
		
	def QuackInvoke(s as string, args as (object)):
		print "Foo.QuackInvoke(${s}, ${join(args, ', ')})"
		
	virtual def Baz():
		print "Foo.Baz"

class Bar(Foo):

	def constructor():
		super("crunch")
		
	override def Baz():
		print "Bar.Baz"
		super()

Bar().Baz()

