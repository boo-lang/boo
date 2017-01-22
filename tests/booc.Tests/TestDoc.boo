// a small class to demonstrate the generation of XML doc files.

namespace MyNamespace
"""
This namespace does not contain any useful things but is,
however, documented.
"""

import System

class MyClass:
"""Performs specific duties."""
	_rand as Random
	"""
	A documented field.
	"""
	
	Rand as Random:
	"""
	A documented property.
	"""
		get: return self._rand
	
	def constructor():
	"""Initializes an instance of <MyClass>"""
		_rand = Random()
	
	
	def Commit():
	"""
	Commits an action.
	See: Generic
	See also: Generic[of T1, T2]
	"""
		pass
	
	def CalculateDouble(i as int) as int:
	"""
	Returns double the value of [i].
	Parameter i: An <int> to be doubled. Or in other words: An <Int32> not as a <System.Double> but doubled.
	Returns: Double the value of [i] but not <Double>.
	"""
		return i * 2
	
	def CauseError(irrelevantArg1 as Collections.ICollection, irrelevantArg2 as String, irrelevantArg3, irrelevantArg4 as Collections.Generic.ICollection of int):
	"""
	Causes an error. Put the overall summary always on top.
	Remarks: This method has not been implemented.
		If you want to add more lines, you should indent.
	However, this line will also be added to the remarks. But you
	will have the problem to read this in the source code.
	Raises NotImplementedException: This has not been implemented yet.
	"""
		return NotImplementedException("CauseError() is not implemented")
	
	def Generic[of T1, T2](arg as T1) as T2:
	"""
	A generic method that does nothing but receiving argument [arg].
	Param arg: An argument
	"""
		pass
