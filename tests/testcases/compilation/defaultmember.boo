"""
C1.ItemOne
set C1.ItemOne
Base.ItemBase
set Base.ItemBase
Base.ItemBase
set Base.ItemBase
C5.C5One
set C5.C5One
C6.ItemOne
C6.ItemThree
set C6.ItemOne
set C6.ItemThree
"""


import System.Reflection

[DefaultMember("ItemBase")]
class Base:
	ItemBase(index as int) as string:
		get:
			return "Base.ItemBase"
		set:
			print "set Base.ItemBase"
			
[DefaultMember("ItemOne")]
interface IOne:
        ItemOne(index as int) as string:
                get
                set

[DefaultMember("ItemTwo")]
interface ITwo:
        ItemTwo(index as int) as string:
                get
                set
		
class C1(IOne):
        ItemOne(index as int) as string:
                get:
                        return "C1.ItemOne"
                set:
                        print "set C1.ItemOne"

class C2(Base):
	pass
	
class C3(Base, IOne, ITwo): //base defaultmember trumps interfaces
        ItemOne(index as int) as string:
                get:
                        return "C3.ItemOne"
                set:
                        print "set C3.ItemOne"
        ItemTwo(index as int) as string:
                get:
                        return "C3.ItemTwo"
                set:
                        print "set C3.ItemTwo"
	
//see tests/testcases/errors/bce0004-2.boo for C4 test case

[DefaultMember("C5One")]
class C5(Base, IOne, ITwo): //current class trumps base class and interfaces
	C5One(index as int) as string:
		get:
			return "C5.C5One"
		set:
			print "set C5.C5One"
	ItemOne(index as int) as string:
		get:
			return "C3.ItemOne"
		set:
			print "set C3.ItemOne"
	ItemTwo(index as int) as string:
		get:
			return "C3.ItemTwo"
		set:
			print "set C3.ItemTwo"
			
[DefaultMember("ItemThree")] //index is a string instead of int
interface IThree:
        ItemThree(index as string) as string:
                get
                set
		
class C6(IOne, IThree):  //properly distinguishes indexers when types differ
	ItemOne(index as int) as string:
		get:
			return "C6.ItemOne"
		set:
			print "set C6.ItemOne"
	ItemThree(index as string) as string:
		get:
			return "C6.ItemThree"
		set:
			print "set C6.ItemThree"
	
	
c1 = C1()
print c1[1]
c1[1] = "1"

c2 = C2()
print c2[2]
c2[1] = "1"

c3 = C3()
print c3[3]
c3[1] = "1"

c5 = C5()
print c5[5]
c5[1] = "1"

c6 = C6()
print c6[6]
print c6["6"]
c6[1] = "1"
c6["1"] = "1"

