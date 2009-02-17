"""
File.Name
Person.Name
"""
class File:
	Name:
		get: return "File.Name"
		
class Person:
	Name:
		get: return "Person.Name"
		
for item as duck in [File(), Person()]:
	print item.Name
