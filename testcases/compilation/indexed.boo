"""
bar
"""
import System.Reflection

[DefaultMember("Item")]
class Properties:
	h = {}
	
	Item(key):
		get:
			return h[key]
			
		set:
			h[key] = value
			
p = Properties()
p["foo"] = "bar"

print(p["foo"])
