"""
f00f!
"""
import System

interface IFoo:
	event DoFoo as EventHandler
	def Fire()

class AbstractFoo (IFoo):
	event DoFoo as EventHandler

	def Fire():
		DoFoo(null, null)

foo as IFoo = AbstractFoo()
foo.DoFoo += { print "f00f!" }
foo.Fire()
