import System
import System.Reflection

class SimpleAttribute(System.Attribute):
	pass
	
enum testEnums:
	[Simple]
	Thang

target = typeof(testEnums)
members = target.GetMember("Thang")
assert len(members) == 1
member = members[0] as FieldInfo
attributes = member.GetCustomAttributes(false)
assert SimpleAttribute in (obj.GetType() for obj in attributes)
