"""
[Nested1, Nested2]
"""
partial class C:
	class Nested1:
		pass
	
partial class C:
	class Nested2:
		pass
		
nested = [t.Name for t in typeof(C).GetNestedTypes()].Sort()
print nested
