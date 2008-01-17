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
namespace Boo.Lang.Interpreter.Tests

import System
import NUnit.Framework
import Boo.Lang.Interpreter

class MockInterpreter(AbstractInterpreter):
	
	_Expected = []
	
	override def Declare(name as string, type as System.Type):
		AssertExpected("Declare(${name})")
		
	override def Lookup(name as string):
		AssertExpected("Lookup(${name})")
		return string
		
	override def GetValue(name as string):
		AssertExpected("GetValue(${name})")
		return "${name}'s value"
		
	override def SetValue(name as string, value as object):
		AssertExpected("SetValue(${name})")
		return value
		
	def Expect(call as string):
		_Expected.Add(call)
		
	def PopExpected():
		return _Expected.Pop() if len(_Expected)
		
	def AssertExpected(call as string):
		Assert.AreEqual(_Expected.Pop(0), call)

[TestFixture]
class AbstractInterpreterTestFixture:
	[Test]
	def TestEval():
		mi = MockInterpreter()
		mi.Expect("Lookup(foo)")
		mi.Expect("Lookup(bar)")
		mi.Expect("SetValue(foo)")
		mi.Expect("GetValue(bar)")
		mi.Expect("SetValue(foo)")
		
		result = mi.Eval("foo = 'foo'; foo = bar")
		Assert.Fail(result.Errors.ToString(true)) if len(result.Errors)
		Assert.IsNull(mi.PopExpected())
		
		
