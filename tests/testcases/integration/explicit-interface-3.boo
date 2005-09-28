"""
A plant, a species of Morinda allied to the madder, the roots of which yield a red dye.
a water bucket.
"""
import System
import System.Collections

class OxfordEnglish(IDictionary):
	_keys = ArrayList()
	_values = Hashtable()

	Count as int:
		get:
			return _keys.Count
	
	IsSynchronized as bool:
		get:
			return _keys.IsSynchronized
	
	SyncRoot:
		get:
			return _keys.SyncRoot

	IsFixedSize as bool:
		get:
			return true

	IsReadOnly as bool:
		get:
			return false
	
	Item(key as object) as object:
		get:
			return _values[key]
		set:
			_values[key] = value
	
	IDictionary.Keys as ICollection:
		get:
			return _values.Keys
	
	IDictionary.Values as ICollection:
		get:
			return _values.Values

	def CopyTo(array as Array, index as int):
		print "COPY!"

	def Add(key, value):
		_keys.Add(key)
		_values.Add(key, value)
	
	def Clear():
		_keys.Clear()
		_values.Clear()
	
	def Contains(key) as bool:
		return _values.Contains(key)
	
	def IEnumerable.GetEnumerator() as IEnumerator:
		return OxfordEnglishEnumerator(_values.GetEnumerator())

	def IDictionary.GetEnumerator() as IDictionaryEnumerator:
		return _values.GetEnumerator()

	def Remove(key):
		_keys.Remove(key)
		_values.Remove(key)
	
	private class OxfordEnglishEnumerator(IEnumerator):
		private _enumerator as IDictionaryEnumerator
		
		def constructor(dictEnumerator as IDictionaryEnumerator):
			_enumerator = dictEnumerator

		IEnumerator.Current as object:
			get:
				return _enumerator.Value

		def IEnumerator.MoveNext():
			return _enumerator.MoveNext()

		def IEnumerator.Reset():
			_enumerator.Reset()


t = OxfordEnglish()
t.Add("Aal", "A plant, a species of Morinda allied to the madder, the roots of which yield a red dye.")
t.Add("Aam", "a water bucket.")

items = [obj for obj in t if obj is not null].Sort()
for item in items:
	print item
