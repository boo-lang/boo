"""
Testing...
"""
callable OutputHandler(message as string)

def print(message as string):
	System.Console.WriteLine(message)
	
handler as OutputHandler
handler = print
handler("Testing...")
