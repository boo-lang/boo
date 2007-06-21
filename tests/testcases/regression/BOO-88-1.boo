"""
cast works
and so does construction
"""
callable OutputHandler(message as string) # this baby is a delegate

def test(message as string):
	print message

handler as OutputHandler
handler = test

handler("cast works")

handler = OutputHandler(test)
handler("and so does construction")

