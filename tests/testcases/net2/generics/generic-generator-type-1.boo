"""
2
4
6
"""
import System.Collections
import System.Collections.Generic

class Reversed[of T](T*):

	_array as (T)
	
	def constructor(array as (T)):
		_array = array

	def GetEnumerator():
		return Enumerator[of T](_array)
		
	def IEnumerable.GetEnumerator():
		return (self as T*).GetEnumerator()
		
	class Enumerator[of T](IEnumerator of T):
		
		_array as (T)
		_i as int
		
		def constructor(array as (T)):
			_array = array
			_i = len(array)
			
		def MoveNext():
			if _i - 1 >= 0:
				_i -= 1
				return true
			return false
			
		Current:
			get: return _array[_i]
			
		IEnumerator.Current:
			get: return _array[-1]
			
		def Dispose():
			pass
			
for i in Reversed[of int]((3, 2, 1)):
	print i * 2
