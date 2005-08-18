namespace Boo.Lang.Useful.Tests.Collections

import NUnit.Framework
import Useful.Collections

[TestFixture]
class CacheTextFixture:
	cache as Cache
	
	[SetUp]
	def Setup():
		cache = Cache(3)
	
	private def AddThreeValues():
		cache.Add(1, "one")
		cache.Add(2, "two")
		cache.Add(3, "three")
	
	[Test]
	def Add():
		assert cache.Count == 0
		cache.Add(1, "one")
		assert cache.Count == 1
		cache.Add(2, "two")
		assert cache.Count == 2
		cache.Add(3, "three")
		assert cache.Count == 3
		
		cache.Add(4, "four")
		assert cache.Count == 3 #overflow
		
		assert cache.ContainsKey(2)
		assert cache.ContainsKey(3)
		assert cache.ContainsKey(4)		
		assert not cache.ContainsKey(1)
	
	[Test]
	def Item():
		AddThreeValues()
		assert cache[1] == "one" #put 1 below on the deletion list
		assert cache.Count == 3
		cache.Add(4, "four") #"2" will be deleted...
		assert cache.Count == 3
		assert cache.ContainsKey(1)
		assert cache.ContainsKey(3)
		assert cache.ContainsKey(4)
		assert not cache.ContainsKey(2)
	
	[Test]
	def Generator():
		AddThreeValues()
		three, two, one = cache
		assert three.Key == 3
		assert three.Value == "three"
		assert two.Key == 2
		assert two.Value == "two"
		assert one.Key == 1
		assert one.Value == "one"
		
	[Test]
	def MaximumSize():
		AddThreeValues()
		cache.MaximumSize = 2
		assert cache.Count == 2
		assert not cache.ContainsKey(1)
		cache.Add(4, "four")
		assert cache.Count == 2
		assert cache.ContainsKey(4)
		cache.MaximumSize = 4
		assert cache.Count == 2
		cache.Add(5, "five")
		cache.Add(6, "six")
		assert cache.Count == 4
		cache.Add(7, "seven")
		assert cache.Count == 4
	
	[Test]
	def Remove():
		AddThreeValues()
		assert cache.Count == 3
		cache.Remove(2)
		assert cache.Count == 2
		assert not cache.ContainsKey(2)
	
	[Test]
	def Keys():
		AddThreeValues()
		coll= cache.Keys as List
		assert coll[0] == 3
		assert coll[1] == 2
		assert coll[2] == 1
		
		s = cache[2] as string #put 2 in front
		assert s == "two"
		
		coll= cache.Keys as List
		assert coll[0] == 2
		assert coll[1] == 3
		assert coll[2] == 1

	[Test]	
	def Values():
		AddThreeValues()
		coll= cache.Values as List
		assert coll[0] == "three"
		assert coll[1] == "two"
		assert coll[2] == "one"
		
		s = cache[2] as string #put 2 in front
		assert s == "two"
		
		coll= cache.Values as List
		assert coll[0] == "two"
		assert coll[1] == "three"
		assert coll[2] == "one"
		
	[Test]
	def Removed():
		removed = 0
		cache.Removed += def(key):
			removed = cast(int,key)
		AddThreeValues()
		cache.Add(4, "four")
		assert removed == 1
	
	[Test]
	def Creator():
		cache.Creator = def(key):
			return "#" + key.ToString()
		assert cache.Count == 0
		
		assert cache[1] == "#1"
		assert cache.Count == 1
		
		assert cache[2] == "#2"
		assert cache.Count == 2
		
		assert cache[3] == "#3"
		assert cache.Count == 3
		
		assert cache[4] == "#4"    #overflow
		assert cache.Count == 3 
		
		#and now using the constructor
		creator = def(key):
			return "#" + key.ToString()
		cache = Cache(3, creator)
		assert cache.Count == 0
		
		assert cache[1] == "#1"
		assert cache.Count == 1
		
		assert cache[2] == "#2"
		assert cache.Count == 2
		
		assert cache[3] == "#3"
		assert cache.Count == 3
		
		assert cache[4] == "#4"    #overflow
		assert cache.Count == 3 
