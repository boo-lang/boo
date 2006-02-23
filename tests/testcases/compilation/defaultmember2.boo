"""
1
2
two
3
4
four
5
five
"""

class C1:
	self[i as int] as int:
		get:
			return i
		set:
			pass
			
class C2:
	self[i as int] as int:
		get:
			return i
	self[i as string] as string:
		get:
			return i

	
class C3:
	_items = matrix(int, 2,3)
	self[one as int, two as int] as int:
		get:
			return _items[one,two]
		set:
			_items[one,two] = value

class C4:
	_items = matrix(int, 2,3)
	_strings = {"a":"one", "b":"two", "d":"four"}
	
	self[one as int, two as int] as int:
		get:
			return _items[one,two]
		set:
			_items[one,two] = value
			
	self[s as string] as string:
		get:
			return _strings[s]
			
class C5:
	NamedIndexer1[one as int, two as int] as int:
		get:
			return 5
			
	NamedIndexer2[s as string] as string:
		get:
			return "five"

c1 = C1()
print c1[1]
c1[1] = 2

c2 = C2()
print c2[2]
print c2["two"]

c3 = C3()
c3[1,2] = 3
print c3[1,2]

c4 = C4()
c4[0,0] = 4
print c4[0,0]
print c4["d"]

c5 = C5()
print c5.NamedIndexer1[2,3]
print c5.NamedIndexer2["5"]

