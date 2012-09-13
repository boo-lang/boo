

class Sketch:
	public Name as string
	
class Favorites:
	public static Sketches = (
						Sketch(Name: "The Ministry of Silly Walks"),
						Sketch(Name: "Dirty Hungarian Phrase Book"),
						Sketch(Name: "Silly Job Interview"))
s = Favorites.Sketches

assert 3 == len(s)
for index, name in enumerate(("The Ministry of Silly Walks",
							"Dirty Hungarian Phrase Book",
							"Silly Job Interview")):
							
	assert name == s[index].Name

	
