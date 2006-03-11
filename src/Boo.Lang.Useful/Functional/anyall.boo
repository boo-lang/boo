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


namespace Boo.Lang.Useful.Functional
import System
import System.Collections
import System.Reflection
final class AnyObject(IQuackFu):
"""The result of an any() or all() call.
Passed and Failed will contain a list of elements that passed or failed the last operation
performed on this object.
You can also access individual fields and properties with this syntax:
any([1, 2, 3]).MaxValue > 3 #true!"""
	_list = []
	_passed = []
	_failed = []
	#This is for daisy-chaining or whatever its called.
	#Eg, someone went "any(list).Width > 5" and 2 elements failed.
	#So, who did those 2 width values belong to?
	#We can tell by using _innerParent instead of _list,
	#to populate _passed and _failed.
	_innerList = []
	_any as bool
	_isDirty = true
	callable FilterCheck(input as object) as bool
	internal def constructor(any as bool, list as IEnumerable):				
		_any = any
		_list.Extend(list)
		_innerList = _list	
	private def result():
		check() if _isDirty		
		return len(Passed) > 0 if _any		
		return len(Failed) == 0		
	private def isValid(element, filter as FilterCheck):
		return false unless element is not null #element must be valid		
		return filter(element)
	private def check(filter as FilterCheck):
		_isDirty = false
		_passed.Clear()
		_failed.Clear()
		#parent might not be the same as elements if the user
		#used this syntax: any(list).Width > 5.
		#Parent will then be elements in _innerList,
		#while _list will be Width values.
		try:			
			for element, parent in zip(_list, _innerList):
				if isValid(element, filter):					
					_passed.Add(parent)					
				else:					
					_failed.Add(parent)				
		except e:
			raise InvalidOperationException("One or more operations are not supported by all contained elements.", e)
	private def check():
		#default check
		check({it| return true if it })
	public def ToString():
		return "Passed (${len(Passed)}): ${Passed}\nFailed (${len(Failed)}): ${Failed}"
	static def op_Equality(lhs as AnyObject, rhs):
		return false if lhs is null		
		lhs.check( {it as duck| return it == rhs} )
		//return lhs.result()
		res = lhs.result();
		return res
	static def op_Equality(lhs as AnyObject, rhs as bool):		
		return false if lhs is null
		lhs.check()
		return lhs.result() == rhs
	static def op_LessThan(lhs as AnyObject, rhs):
		lhs.check( {it as duck| return it < rhs} )
		return lhs.result()
	static def op_LessThanOrEqual(lhs as AnyObject, rhs):
		lhs.check( {it as duck| return it <= rhs} )
		return lhs.result()
	static def op_GreaterThan(lhs as AnyObject, rhs):
		lhs.check( {it as duck| return it > rhs} )
		return lhs.result()
	static def op_GreaterThanOrEqual(lhs as AnyObject, rhs):
		lhs.check( {it as duck| return it >= rhs} )
		return lhs.result()
	static def op_Match(lhs as AnyObject, rhs as regex):
		return false unless rhs
		lhs.check( {it as duck|return rhs.IsMatch(it)} )
		return lhs.result()
	static def op_Match(lhs as AnyObject, rhs as string):
		return false unless rhs
		return op_Match(lhs, regex(rhs))		
	static def op_NotMatch(lhs as AnyObject, rhs as regex):
		return not op_Match(lhs, rhs)
	static def op_NotMatch(lhs as AnyObject, rhs as string):
		return false unless rhs
		return not op_Match(lhs, regex(rhs))
	#Boo has its op_Member stuff hardcoded; uncomment when we can create our own.
#	static def op_Member(lhs as AnyObject, rhs as IList):
#		return false unless rhs
#		lhs.check( {it as duck| return it in rhs} )
#		return lhs.result()
#	static def op_Member(lhs as AnyObject, rhs as IDictionary):
#		return false unless rhs
#		lhs.check( {it as duck| return it in rhs} )
#		return lhs.result()
#	static def op_Member(lhs as AnyObject, rhs as IEnumerable):
#		return false unless rhs
#		lhs.check( {it as duck| return it in rhs} )
#		return lhs.result()
	static def op_Implicit(obj as AnyObject):		
		return false if obj is null		
		return obj.result()
		
	def Equals(obj):
		return false if obj is null		
		if obj.GetType() == bool: #Boo 598 strikes again! op_Equality(AnyObject, bool) should be called
			return op_Equality(self, cast(bool, obj))
		return op_Equality(self, obj)	
	def QuackSet(name as string, obj):
		raise NotImplementedException("Cannot set values.")		
	def QuackGet(name as string):		
		return runtime(name, null)
	def QuackInvoke(name as string, *args as (object)):		
		return runtime(name, args)
		
	def runtime(name, parameters as (object)):
		#If the member does not exist, it might exist in the elements.
		#Let's work around BOO-598 first.
		this = GetType()
		if method = this.GetMethod(name):
			return method.Invoke(self, parameters)
		elif property = this.GetProperty(name):
			return property.GetValue(self, parameters)
		elif field = this.GetField(name):
			return field.GetValue(self)
		#It might be contained inside of the elements then
		subset = []
		types = array(Type, element.GetType() for element in _list if element)
		for type as Type, element in zip(types, _list):			
			if property = type.GetProperty(name):
				value = property.GetValue(element, null)
			elif field = type.GetField(name):
				value = field.GetValue(element)
			else:
				#TODO: Support method invocations when Boo gets that ability again.
				raise ArgumentException("Only support fields, properties; ${type}.${name} is neither, sorry.")
			subset.Add(value)
		obj = AnyObject(_any, subset)
		obj._innerList = _list
		return obj
	Passed:
		get:
			check() if _isDirty
			return _passed
	Failed:
		get:
			check() if _isDirty
			return _failed	
	
def any(thing as IEnumerable):
	return AnyObject(true, thing)
def all(thing as IEnumerable):
	return AnyObject(false, thing)
