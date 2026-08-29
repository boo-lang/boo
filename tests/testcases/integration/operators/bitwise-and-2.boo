enum Flags:
	None = 0
	One = 1
	Two = 2
	Four = 4
	All = 7
	
for flag in Flags.One, Flags.Two, Flags.Four:
	assert Flags.None == Flags.None & flag	
	assert flag == Flags.All & flag
	
assert Flags.None == Flags.All & Flags.None
assert Flags.None == Flags.None & Flags.All

