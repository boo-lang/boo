"""
1
1, 2
"""
macro varargs:
	case [| varargs $name |]:
		yield [|
			def $name(*args):
				print join(args, ', ')
		|]
		
varargs foo
foo 1
foo 1, 2
	
	

