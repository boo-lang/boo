import System.Collections
import System.Reflection

[DefaultMemberAttribute("Item")]
class Properties:
	l = {}
	
	Item(key):
		get:
			return l[key]
			
		set:
			l[key] = value
			
p = Properties()
p["foo"] = "bar"

print(p["bar"])
print(p.Item["bar"])
