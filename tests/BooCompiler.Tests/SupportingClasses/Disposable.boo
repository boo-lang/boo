namespace BooCompiler.Tests.SupportingClasses

import System

class Disposable(System.IDisposable):
	def constructor():
		Console.WriteLine("Disposable.constructor")

	public def foo():
		Console.WriteLine("Disposable.foo")

	def System.IDisposable.Dispose():
		Console.WriteLine("Disposable.Dispose")
