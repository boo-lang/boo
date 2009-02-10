"""
Description:
	This examples shows how to register and use a lightweight compiler
	service	that maintains a list of locations of `trace` macro usages
	and write that list to a file _only_ when compilation succeeds.

	First you need to compile the library containing the service with:
	`booc -t:library TraceService.boo`

	Then compile with `booc -r:TraceService.dll -o:trace.exe trace.boo`.
	A file `trace.exe.traces` will be created with this content:
	trace.boo(37,9) : Example.Foo
	trace.boo(40,9) : Example.Bar

Exercise:
	Modify the macro (and/or TraceService) to allow at most one `trace`
	by method and return a compiler error otherwise.
	(hints: TraceServices.locations and CompilerContext.Errors)
"""

import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching


macro trace(message as string):
	try:
		service = Context.GetService[of TraceService]()
	except as System.ArgumentException:
		Context.RegisterService[of TraceService](service = TraceService(Context))

	service.AddLocation(trace.LexicalInfo, trace.GetAncestor[of Method]())
	yield [| System.Diagnostics.Trace.WriteLine($message) |]


static class Example:
	def Foo():
		trace "entered foo..."

	def Bar():
		trace "entered bar..."


#NB: if you execute this example you won't see anything displayed unless you
#have compiled with -d:TRACE and add a trace listener.
#http://msdn.microsoft.com/en-us/library/system.diagnostics.trace.aspx
Example.Foo()
Example.Bar()

