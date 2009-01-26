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


namespace Boo.Lang.Useful.Collections

import System
import System.Collections
import System.Reflection

[DefaultMember("Item")]
[EnumeratorItemType(DictionaryEntry)]
class Cache(IDictionary):
"""
Cache class. Works like a Hash with a limited size. The least recently used item is removed when size overflows.

This class has the option to supply a creator method that will be used if an entry is not found in cache.
Furthermore it can fire an event if an entry is removed from cache due to overflow.

Usage

cache = Cache(2) #create a cache with size 2
cache.Add(1, "1")
cache.Add(2, "2")
cache.Add(3, "3") #overflow, least recently used item will be removed i.e. 1, "1"

assert cache.Count == 2
assert cache[3] == "3"
assert cache[2] == "2"

cache.Add(4,"4") #overflow, 3 will be removed (since 2 is most recently used)
assert cache.Count == 2
assert cache[4] == "4"
assert cache[2] == "2"

#add a creator method
cache.Creator = def(key):
	print 'Create entry: ' + key
	return key.ToString()

#add an removed event
cache.Removed += def(key):
	print "Removed " + key

assert cache[42] == "42" #will print: Create entry: 42. Removed 4 from cache

@author Edwin de Jonge
"""
	internal class LruItem:
	"""
	Least Recently Used Item stored as a circular double linked list
	"""
		_prev = self
		_next = self
		
		[getter(Key)]
		_key as object
		
		[getter(Value)]
		_value as object
		
		def constructor(key, value):
			_key = key
			_value = value
			
		def Insert(item as LruItem):
			_next._prev = item
			item._next = _next
			item._prev = self
			_next = item
			
		def Remove():
			_prev._next = _next
			_next._prev = _prev
			return self
			
		def RemovePrevious():
			return _prev.Remove()
			
		def Generator(): //cycle through the items
			item = _next
			while item._key: #iterate until head
				yield item
				item = item._next
				
	public callable ItemCreator(key as object) as object

	_hash = {}
	_maxSize = 100
	_head = LruItem(null, null)
	_creator as ItemCreator = def(key):   #default creator behaviour
		return null
		
	def constructor():
	"""
	Create Cache object with default size 100
	"""
		pass
		
		
	def constructor([required] maximumSize):
	"""
	Create cache with maximumSize
	"""
		self.MaximumSize = maximumSize

	def constructor([required] maximumSize, [required] creator as ItemCreator):
	"""
	Create cache with maximumSize
	If creator is supplied, the cache will use this function to create an entry in the cache during
	retrieval if it is not available.
	"""
		self.MaximumSize = maximumSize
		_creator = creator

	def constructor([required] hash as IDictionary):
		for item as DictionaryEntry in hash:
			Add(item.Key, item.Value)
			
	Creator as ItemCreator:
	"""
	Method that will be used if object cannot be found in cache
	"""
		set:
			_creator = value

	MaximumSize:
	"""
	Maximum size of cache. If value is set to a value less than current Count, cache will resize itself
	to MaximumSize
	"""
		get:
			return _maxSize
		set:
			_maxSize = value
			assert _maxSize > 0			
			CheckSize()

	Item[key]:
		get:
			item = _hash[key] as LruItem
			if item:
				item.Remove()
				_head.Insert(item)
				return item.Value
			#item not found, try to create using the creator function
			val = _creator(key)
			if val:
				Add(key, val)
			return val
			
		set:
			item = LruItem(key, value)
			if _hash.Contains(key):
				item = _hash[key]
				item.Remove() #remove from double linked list
				_head.Insert(item) #insert in begin of list
				#_hash[key] = item
			else:
				Add(key, value)
				
	Count:
	"""
	Current size of cache
	"""
		get:
			return _hash.Count
			

	def CopyTo(target as System.Array, index as int): #not sure if this is intended behaviour...
		_hash.CopyTo(target, index)

	SyncRoot:
		get:
			return _hash.SyncRoot

	IsSynchronized:
		get:
			return _hash.IsSynchronized
			
	def Remove(key):
	"""
	Removes item from cache
	"""
		item = _hash[key] as LruItem
		return if item is null
		item.Remove()
		_hash.Remove(key)
			
	def Clear():
	"""
	Empties cache
	"""
		_hash.Clear()
		_head = LruItem(null, null)
		
	internal class CacheEnumerator(IDictionaryEnumerator, IEnumerator):
		_e as IEnumerator
		
		def constructor(e as IEnumerable):
			_e = e.GetEnumerator()
			
		def Reset():
			_e.Reset()
			
		def MoveNext():
			return _e.MoveNext()
			
		Current:
			get:
				return self.Entry
			
		Entry:
			get:
				item = CurrentItem
				return DictionaryEntry(item.Key, item.Value)
				
		Key:
			get:
				return CurrentItem.Key
		
		Value:
			get:
				return CurrentItem.Value
				
		CurrentItem:
			get:
				return _e.Current as LruItem
		
	def IDictionary.GetEnumerator():
		return CacheEnumerator(_head.Generator())
		
	def IEnumerable.GetEnumerator():
		return cast(IDictionary, self).GetEnumerator()
		
	def Add(key, value):
	"""
	Adds a key value pair to cache object
	"""
		item = LruItem(key, value)
		_hash.Add(key, item)
		_head.Insert(item)
		CheckSize()
		
	private def CheckSize():
		while _hash.Count > _maxSize:
			last = _head.RemovePrevious()
			_hash.Remove(last.Key)
			if Removed:				#fire removed event
				Removed(last.Key)
				
	def Contains(key):
		return _hash.Contains(key)
		
	def ContainsKey(key):
		return _hash.ContainsKey(key)
		
	def ContainsValue(value):
	"""
	True is value is in cache
	"""
		for item as LruItem in _head.Generator():
			if item.Value == value:
				return true
		return false

	public event Removed as callable(object)
	"""
	This event is fired if an object is removed out of the cache due to overflow
	"""
	
	Keys as ICollection:
	"""
	List of keys in order of most recently used
	"""
		get:
			return [item.Key for item as LruItem in _head.Generator()]

	Values as ICollection:
	"""
	List of values in order of most recently used
	"""
		get:
			return [item.Value for item as LruItem in _head.Generator()]
			
	IsReadOnly:
	"""
	Always false
	"""
		get:
			return false

	IsFixedSize:
	"""
	Always false
	"""
		get:
			return false
