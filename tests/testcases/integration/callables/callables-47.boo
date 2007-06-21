"""
Testing...
Testing B...
Testing 2...
"""

callable OutputHandler(message as string)

callable OutputHandler2(message as string, message2 as string)

def printit(message as string):
	System.Console.WriteLine(message)
	
handler as OutputHandler = printit
handler("Testing...")

handlerb = printit as OutputHandler
handlerb("Testing B...")

handler2 = printit as OutputHandler2
handler2("Testing 2...", "huh?")

