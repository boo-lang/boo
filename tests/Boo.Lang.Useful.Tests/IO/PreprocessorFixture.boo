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

namespace Boo.Lang.Useful.IO.Tests

import NUnit.Framework
import Boo.Lang.Useful.IO

[TestFixture]
class PreprocessorTestFixture:
	
	[Test]
	def TestNoDefines():
		code = "print '#ifdef'\n"
		AssertPreProcessor(code, code)
		
	[Test]
	def TestElse():
		code = """
#if FOO
print 'foo'
#else
print 'else'
#endif
"""
		AssertPreProcessor("\nprint 'foo'\n", code, "FOO")
		AssertPreProcessor("\nprint 'else'\n", code)
		
	[Test]
	def TestNestedDefines():
		code = """
#if FOO
print 'foo'
#if BAR
print 'bar'
#endif
print 'foo again'
#endif
print 'outer foo'
"""
		expected = """
print 'foo'
print 'foo again'
print 'outer foo'
"""
		AssertPreProcessor(expected, code, "FOO")
		
	[Test]
	def TestSimpleDefines():
		code = """
#if FOO
print 'foo'
#endif
#if BAR
print 'bar'
#endif"""

		expected = """
print 'bar'
"""
		AssertPreProcessor(expected, code, "BAR")
		
		expected = """
print 'foo'
"""
		AssertPreProcessor(expected, code, "FOO")
		
		expected = """
print 'foo'
print 'bar'
"""
		AssertPreProcessor(expected, code, "FOO", "BAR")
		
		expected = """
"""
		AssertPreProcessor(expected, code)
		
	[Test]
	def TestNot():
		code = """
#if !FOO_1_0 // comments are ignored
print 'not foo'
#else 
print 'foo'
#endif
print 'either'
#if FOO_1_0
print 'foo again'
	#if BAR
	print 'bar'
	#else
	print 'not bar'
	#endif
#else
print 'not foo again'
#endif
print 'either again'
"""
		expected = """
print 'not foo'
print 'either'
print 'not foo again'
print 'either again'
"""
		AssertPreProcessor(expected, code)
		
		expected = """
print 'foo'
print 'either'
print 'foo again'
	print 'not bar'
print 'either again'
"""
		AssertPreProcessor(expected, code, "FOO_1_0")
		
		expected = """
print 'foo'
print 'either'
print 'foo again'
	print 'bar'
print 'either again'
"""
		AssertPreProcessor(expected, code, "FOO_1_0", "BAR")

	[Test]
	def TestExpressions():
		code = """
#if FOO || BAR
print 'foo or bar'
#endif
#if FOO && BAR
print 'foo and bar'
#endif
#if (FOO || BAR) && BAZ
print '(foo or bar) and BAZ
#endif
"""

		expected = """
print 'foo or bar'
print '(foo or bar) and BAZ
"""
		AssertPreProcessor(expected, code, "FOO", "BAZ")
		
		expected = """
print 'foo or bar'
"""
		AssertPreProcessor(expected, code, "BAR")

		
	def AssertPreProcessor(expected, actual, *defines as (string)):
		pp = PreProcessor()
		for d in defines:
			pp.Define(d)
		Assert.AreEqual(expected, pp.Process(actual).Replace("\r\n", "\n"))
