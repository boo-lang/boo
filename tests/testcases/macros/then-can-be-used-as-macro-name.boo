"""
something happens
"""
macro then(what as string):
	yield [| print $what |]
	
then "something happens"
