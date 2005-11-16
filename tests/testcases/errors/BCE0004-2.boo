"""
BCE0004-2.boo(32,9): BCE0004: Ambiguous reference 'get_ItemOne': IOne.get_ItemOne(System.Int32), ITwo.get_ItemTwo(System.Int32).
BCE0004-2.boo(33,3): BCE0004: Ambiguous reference 'set_ItemOne': IOne.set_ItemOne(System.Int32, System.String), ITwo.set_ItemTwo(System.Int32, System.String).
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

