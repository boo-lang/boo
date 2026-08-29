"""
setter refused null.
getter refused null.
but 'foo' worked!
"""
import System
import System.Reflection

[DefaultMember("Item")]
class Properties:
	h = {}
	
	Item([required] key):
		get:
			return h[key]
			
		set:
			h[key] = value
			
p = Properties()
try:
	p[null] = "bar"
except x as ArgumentNullException:
	print("setter refused null.")
	
try:
	print(p[null])
except x as ArgumentNullException:
	print("getter refused null.")

p["foo"] = "but 'foo' worked!"
print(p["foo"])
