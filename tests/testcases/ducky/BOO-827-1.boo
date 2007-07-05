"""
string Hello
stuff A
play Hello A
"""
enum Stuff:
	A
	B
	C
	
def play(stuff as Stuff):
	print "stuff", stuff

def play(s as string):
	print "string", s
	
def play(s as string, stuff as Stuff):
	print "play", s, stuff

a as duck = "Hello"
play(a)

b as duck = Stuff.A
play(b)

play(a, b)

