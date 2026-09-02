namespace BooCompiler.Tests.SupportingClasses

import System

class VarArgs:
	public def Method():
		Console.WriteLine("VarArgs.Method")

	public def Method(*args as (object)):
		Console.WriteLine("VarArgs.Method({0})", Boo.Lang.Builtins.join(args, ", "))
