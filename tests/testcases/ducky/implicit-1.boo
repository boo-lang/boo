"""
a love supreme
"""
class Song:
	static def op_Implicit(song as Song) as string:
		return song.Name
		
	[property(Name)]
	_name = ""
	
song as duck = Song(Name: "a love supreme")
s as string = song
print s
