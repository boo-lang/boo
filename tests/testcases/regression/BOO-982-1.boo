"""
QUACK(Fu)! Foo.Do(it 2 day)
QUACK(Fu)! Foo.Try(1 time)
Successful compilation and passing test implies proper constructor chaining within IQuackFu!
"""

class Foo(IQuackFu):
	final _foo as (string)

	def constructor(*foo as (string)):
		self(1, *foo)
	
	def constructor(bar as int, *foo as (string)):
		self(1, 2, *foo)
	
	def constructor(bar as int, baz as int, *foo as (string)):
		_foo = foo
	
	def TryIt():
		self.Try(1, 'time')
	
	def QuackGet(name as string, params as (object)):
		raise System.NotImplementedException()
	
	def QuackSet(name as string, params as (object), value as object):
		raise System.NotImplementedException()
	
	def QuackInvoke(name as string, args as (object)):
		print "QUACK(${join(_foo,'|')})! ${self.GetType().FullName}.${name}(${join(args)})"

tester = Foo('Fu')

tester.Do('it', 2, 'day')
tester.TryIt()

print 'Successful compilation and passing test implies proper constructor chaining within IQuackFu!'