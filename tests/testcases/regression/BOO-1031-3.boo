"""
Success::System.Collections.IEnumerable.GetEnumerator
Success2::System.Collections.IEnumerable.GetEnumerator
Success3::System.Collections.IEnumerable.GetEnumerator
"""

import System.Collections
import System.Collections.Generic

class B:
	pass

interface IGimmeEnumerator:
	def GetEnumerator() as object

interface IEnumeratorOuLaVie:
	def GetEnumerator() as string

#check that sensible return values are set (if not, peverify will warn)
class Success(B*,IGimmeEnumerator,IEnumeratorOuLaVie):
	def IEnumerable.GetEnumerator():
		print "Success::System.Collections.IEnumerable.GetEnumerator"
		return null
	def GetEnumerator():
		return null
	def IGimmeEnumerator.GetEnumerator():
		return null
	def IEnumeratorOuLaVie.GetEnumerator():
		return null

#make sure it works the other way too
class Success2(B*,IGimmeEnumerator,IEnumeratorOuLaVie):
	def GetEnumerator():
		return null
	def IGimmeEnumerator.GetEnumerator():
		return null
	def IEnumeratorOuLaVie.GetEnumerator():
		return null
	def IEnumerable.GetEnumerator():
		print "Success2::System.Collections.IEnumerable.GetEnumerator"
		return null

#make sure it works the other way too
class Success3(B*,IGimmeEnumerator,IEnumeratorOuLaVie):
	def IEnumeratorOuLaVie.GetEnumerator():
		return null
	def IGimmeEnumerator.GetEnumerator():
		return null
	def GetEnumerator():
		return null
	def IEnumerable.GetEnumerator():
		print "Success3::System.Collections.IEnumerable.GetEnumerator"
		return null

e as IEnumerable = Success()
assert null == e.GetEnumerator()
e = Success2()
assert null == e.GetEnumerator()
e = Success3()
assert null == e.GetEnumerator()

