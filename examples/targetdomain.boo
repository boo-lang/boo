"""
This example demonstrates how to compile and execute code in a different
AppDomain.

This is actually less useful than it first seems because in-memory only
assemblies cannot cross AppDomain boundaries :-(

An alternative way would be to use AppDomain.DoCallback.
"""
import System
import System.Security.Policy
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

domain = AppDomain.CreateDomain("A Different AppDomain", AppDomain.CurrentDomain.Evidence)

compiler as BooCompiler = domain.CreateInstanceAndUnwrap("Boo", "Boo.Lang.Compiler.BooCompiler")
compiler.Parameters.Pipeline.Load("booi")

code = "print(System.AppDomain.CurrentDomain.FriendlyName)"
compiler.Parameters.Input.Add(StringInput("code", code))

print("running...")
result = compiler.Run()
print("done.")

if len(result.Errors):
	print(join(result.Errors, "\n"))
