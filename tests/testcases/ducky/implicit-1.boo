"""
a love supreme
a love supreme
"""
class Song:
	static def op_Implicit(song as Song) as string:
		return song.Name
		
	[property(Name)]
	_name = ""
	
class Movie:
	public title as string
	
song as duck = Song(Name: "a love supreme")
s as string = song
print s

m as duck = Movie()
m.title = song
print m.title
