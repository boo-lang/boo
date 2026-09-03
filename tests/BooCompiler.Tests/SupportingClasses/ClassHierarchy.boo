#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace BooCompiler.Tests.SupportingClasses

import System

class AmbiguousBase:
	public def Path(empty as string) as string:
		return "Base"

class AmbiguousSub1(AmbiguousBase):
	new public Path as string:
		get: return "Sub1"

class AmbiguousSub2(AmbiguousSub1):
	pass

class BaseClass:
	protected def constructor():
		pass

	protected def constructor(message as string):
		Console.WriteLine("BaseClass.constructor('{0}')", message)

	public virtual def Method0():
		Console.WriteLine("BaseClass.Method0")

	public virtual def Method0(text as string):
		Console.WriteLine("BaseClass.Method0('{0}')", text)

	public virtual def Method1():
		Console.WriteLine("BaseClass.Method1")

	#for BOO-632 regression test
	protected _protectedfield as int = 0

	protected ProtectedProperty as int:
		get: return _protectedfield
		set: _protectedfield = value

class DerivedClass(BaseClass):
	def constructor():
		pass

	public def Method2():
		Method0()
		Method1()

class ClassWithNewMethod(DerivedClass):
	new public def Method2():
		Console.WriteLine("ClassWithNewMethod.Method2")
