"""
a love supreme
"""
class Song:
	static def op_Implicit(song as Song) as string:
		return song.Name
		
	[property(Name)]
	_name = ""
	
class Person:
	[property(Favorite)]
	_favorite as Song
	
p as duck = Person(Favorite: Song(Name: "a love supreme"))
s as string = p.Favorite
print s
