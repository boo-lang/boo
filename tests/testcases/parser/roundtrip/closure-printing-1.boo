"""
import System
import System.IO

def Run(action as Action):
	action()

Run():
	print 'a'
	print 'b'

NeedsHeader = def (path as string) as bool:
	using reader = StringReader(path):
		return (reader.ReadLine() is not null)

print NeedsHeader('x')
"""
import System
import System.IO

def Run(action as Action):
	action()

# A closure carrying a block cannot be written inline, and one passed as the
# last argument of a call statement has to be printed as a trailing block.
Run():
	print "a"
	print "b"

NeedsHeader = def(path as string) as bool:
	using reader = StringReader(path):
		return reader.ReadLine() is not null

print NeedsHeader("x")
