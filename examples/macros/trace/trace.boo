#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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

