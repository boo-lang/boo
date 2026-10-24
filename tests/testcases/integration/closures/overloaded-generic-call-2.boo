"""
2 True True
"""
import System
import System.Collections.Generic
import System.Linq.Enumerable

interface IThing:
	pass

class Item(IThing):
	pass

class Store[of T(IThing)]:
	_items = Dictionary[of int, Func[of T]]()

	def constructor(values as KeyValuePair[of int, Func[of T]]*):
		_items = values.ToDictionary({kv | kv.Key}, {kv | kv.Value})

	def Count():
		return _items.Count

	def Get(key as int) as T:
		return _items[key]()

pairs = List[of KeyValuePair[of int, Func[of Item]]]()
pairs.Add(KeyValuePair[of int, Func[of Item]](3, { return Item() }))
pairs.Add(KeyValuePair[of int, Func[of Item]](5, { return Item() }))
store = Store[of Item](pairs)
print store.Count(), store.Get(3) isa Item, store.Get(5) isa Item
