"""
BCE0004-2.boo(32,9): BCE0004: Ambiguous reference 'get_ItemOne': IOne.get_ItemOne(int), ITwo.get_ItemTwo(int).
BCE0004-2.boo(33,3): BCE0004: Ambiguous reference 'set_ItemOne': IOne.set_ItemOne(int, string), ITwo.set_ItemTwo(int, string).
"""
import System.Reflection

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
		
class C4(IOne, ITwo):
        ItemOne(index as int) as string:
                get:
                        return "C4.ItemOne"
                set:
                        pass
        ItemTwo(index as int) as string:
                get:
                        return "C4.ItemTwo"
                set:
                        pass
			
c4 = C4()
print c4[4]
c4[3] = "2"

