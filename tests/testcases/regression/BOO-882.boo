"""
Indexer with generic parameter
"""

class C[of T]:
	self[arg as int]:
		get: return "Indexer with int parameter"

	self[arg as T]:
		get: return "Indexer with generic parameter"

c = C[of string]()
print c["foo"]
