#region license
// Copyright (c) 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
 
namespace Boo.Lang.Useful.Tests.Attributes

import System
import Boo.Lang.Useful.Attributes
import NUnit.Framework

[TestFixture]
class OnceAttributeTestFixture(AbstractAttributeTestFixture):
"""
@author Sorin Ionescu (sorin.ionescu@gmail.com)
"""
	private class Math:
		[once]
		static def Square(value as int):
			return value**2
		
		[once]
		def Cube(value as int):
			return value**3
			
	// TODO: test attribute expansion with AbstractAttributeTestFixture
	
	[Test]
	def TestStaticOnce():
		assert 100 == Math.Square(10)
		assert 100 == Math.Square(20)
		
	[Test]
	def TestInstanceOnce():
		m1 = Math()
		m2 = Math()
		
		assert 1000 == m1.Cube(10)
		assert 1000 == m1.Cube(20)
		assert 27 == m2.Cube(3)
		assert 27 == m2.Cube(10)
		
	[Test]
	def TestModuleMethod():
		code = """
import Useful.Attributes

[once]
def foo():
	return 3
"""

		expected = """
import Useful.Attributes

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class CodeModule(object):

	public static def foo() as int:
		if not CodeModule.___foo_cached:
			System.Threading.Monitor.Enter(CodeModule.___foo_lock)
			try:
				if not CodeModule.___foo_cached:
					CodeModule.___foo_returnValue = 3
					CodeModule.___foo_cached = true
			ensure:
				System.Threading.Monitor.Exit(CodeModule.___foo_lock)
		return CodeModule.___foo_returnValue

	private static ___foo_cached as bool

	private static ___foo_lock as object

	private def constructor():
		super()

	public static def constructor():
		CodeModule.___foo_lock = object()

	private static ___foo_returnValue as int
"""
		RunTestCase(expected, code)
