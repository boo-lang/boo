"""
it was a string.
it wasn't a string.

"""
def test(reference):
	s = reference as string
	if s:
		print("it was a string.")
	else:
		print("it wasn't a string.")
		
test("a string")
test(1)
