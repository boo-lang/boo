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

namespace Boo.Lang.Useful.Collections

import System
import System.Collections

class Set(ICollection, IEnumerable):
"""
Mathematical Set class.

From any enumerable a Set can be created. The Set will not contain duplicate itemects (it is not a multiset).
The Set class implements intersection, union, subset and (left/right)difference

The Set class supports the following syntax:

a = Set((1,2))              # Create set a
b = Set((2,3))				# Create set b

intersection = a & b        # == {2}
union = a | b               # == {1,2,3}
leftdiff = a - b            # == {1}
rightdiff = b - a           # == {3}
diff = a.Differrence(b)     # == {1,3}, this should be a ^ b, but Boo doesn't support this yet

isSubSet = a <= union       # == true
isStrictSubSet = a < union  # == true

"""
	_items = {}
	
	def constructor():
		pass

	def constructor([required] other as Set):
		_items = Hash(other._items)
	
	def constructor([required] enumerable):
		AddRange(enumerable)
	
	def Contains(item) as bool:
		return _items.Contains(item)
	
	def HasMember(item) as bool:
		return Contains(item)
	
	def Add([required] item):
		_items[item] = null
	
	def AddRange([required] enumerable):
		for item in iterator(enumerable):
			Add(item)
		
	def Remove([required] item):
		_items.Remove(item)
	
	def Intersection([required] other as Set) as Set:
		retval = Set()
		for item in other:
			if Contains(item):
				retval.Add(item)
		return retval
	
	def Union([required] other as Set) as Set:
		retval = Set(self)
		retval.AddRange(other)
		return retval
	
	def LeftDifference(other as Set) as Set:
		return op_Subtraction(self, other) # i.e. self - other
		
	def RightDifference(other as Set) as Set:
		return op_Subtraction(other,self) # i.e. other - self

	static def op_Subtraction(a as Set, b as Set) as Set:
		diff = Set()
		for item in a:
			if not b.Contains(item):
				diff.Add(item)
		return diff

	static def op_BitwiseOr(a as Set, b as Set) as Set:
		return a.Union(b)
	
	def Difference(other as Set) as Set:
		diff = RightDifference(other) | LeftDifference(other)
		return diff
		
	static def op_Member(item, s as Set) as bool:
		return s.Contains(item)
		
	static def op_NotMember(item, s as Set) as bool:
		return not s.Contains(item)
	
	static def op_BitwiseXor(a as Set, b as Set) as Set: #this is not yet supported....
		return a.Difference(b)
	
	static def op_BitwiseAnd(a as Set, b as Set) as Set:
		return a.Intersection(b)
	
	static def op_LessThanOrEqual(a as Set, b as Set) as bool:
		return b.IsSubSet(a)
	
	static def op_LessThan(a as Set, b as Set) as bool:
		return (a.Count < b.Count and b.IsSubSet(a))
	
	static def op_GreaterThanOrEqual(a as Set, b as Set) as bool:
		return a.IsSubSet(b)
	
	static def op_GreaterThan(a as Set, b as Set) as bool:
		return (a.Count > b.Count and a.IsSubSet(b))
	
	def IsSubSet(a as Set) as bool:
		if a.Count > Count:
			return false
		
		for item in a:
			if not Contains(item):
				return false
		return true
	
	Count:
		get:
			return _items.Count
	
	SyncRoot:
		get:
			return _items.SyncRoot
	
	IsSynchronized as bool:
		get:
			return _items.IsSynchronized
	
	def CopyTo(arr as System.Array, index as int):
		_items.Keys.CopyTo(arr, index)
	
	def GetEnumerator() as System.Collections.IEnumerator:
		return _items.Keys.GetEnumerator()
	
	def Clone() as object:
		return Set(self)
