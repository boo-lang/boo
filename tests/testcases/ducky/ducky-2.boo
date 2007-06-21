"""
Homer Simpson
John Cleese
"""
class Celebrity:
	[property(Name)] _name = ""
	
def dump(obj):
	print obj.Name
	
people = [Celebrity(Name: "Homer Simpson"), Celebrity(Name: "John Cleese")]
for person in people:
	dump(person)
