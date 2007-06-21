"""
foo = 3
key = value
"""
h = { "key" : "value", "foo": 3 }

for key, value in [(item.Key, item.Value) for item in h].Sort():
	print key, "=", value
	
