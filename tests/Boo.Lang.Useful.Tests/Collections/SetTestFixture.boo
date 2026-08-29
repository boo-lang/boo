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

namespace Boo.Lang.Useful.Tests.Collections

import Boo.Lang.Useful.Collections
import NUnit.Framework

[TestFixture]
class TestSet:
"""
@author Edwin de Jonge
"""
	a as Set
	b as Set
	
	[SetUp]
	def SetUp():
		a = Set((1,2))
		b = Set((2,3))
		
	[Test]
	def Create():
		assert a.Count == 2
		assert b.Count == 2
		assert len(a) == 2
		assert len(b) == 2
		
	[Test]
	def Intersection():
		c = a & b
		assert c.Count == 1
		assert c.Contains(2)
		
		d = b & a
		assert d.Count == 1
		assert d.Contains(2)
		assert 2 in d
		assert 3 not in d
		assert 1 not in d
		
		c = a.Intersection(b)
		assert c.Count == 1
		assert c.Contains(2)
		assert 2 in c
		assert 1 not in c
		
		c = b.Intersection(a)
		assert c.Count == 1
		assert c.Contains(2)
		
	[Test]
	def Union():
		c = a | b
		assert c.Count == 3
		assert c.Contains(1)
		assert c.Contains(2)
		assert c.Contains(3)

		c = b | a
		assert c.Count == 3
		assert c.Contains(1)
		assert c.Contains(2)
		assert c.Contains(3)
		
	[Test]
	def IsPartOf():
		c = a & b
		assert c < a
		assert c < b
		assert a > c
		assert b > c
		
		assert not c > a
		assert not a < c
		assert not b < a
		assert not a < b
		
		assert a <= a
		assert a >= a
		
		assert c <= a
		assert a >= c
	
	[Test]
	def Add():
		c = Set()
		assert c.Count == 0
		c.Add(1)
		assert c.Count == 1
		assert c.Contains(1)
		
	[Test]
	def Remove():
		a.Remove(1)
		assert a.Count == 1
		assert 1 not in a
		assert 2 in a
		a.Remove(2)
		assert 0 == len(a)

	[Test]		
	def AddRange():
		d = Set()
		d.AddRange(a)
		assert d.Count == a.Count
		for obj in a:
			assert d.Contains(obj)
			
	[Test]
	def Difference():
		c = a - b
		assert c.Count == 1
		assert c.Contains(1)
		
		c = b - a
		assert c.Count == 1
		assert c.Contains(3)
		
		c = b.Difference(a)
		assert c.Count == 2
		assert not c.Contains(2)
		assert c.Contains(1)
		assert c.Contains(3)
		
		c = a.Difference(b)
		assert c.Count == 2
		assert not c.Contains(2)
		assert c.Contains(1)
		assert c.Contains(3)
		
		c = a.Difference(a)
		assert c.Count == 0
