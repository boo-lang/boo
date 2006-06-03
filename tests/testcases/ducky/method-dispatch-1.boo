"""
tom
tom
crash
"""
class Drum:
	pass
	
class Cymbal:
	pass
	
def play(drum as Drum):
	print "tom"
	
def play(cymbal as Cymbal):
	print "crash"
	
for item as object in [Drum(), Drum(), Cymbal()]:
	play(item)
