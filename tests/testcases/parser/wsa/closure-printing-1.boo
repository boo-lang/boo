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
end

Run():
	print 'a'
	print 'b'
end

NeedsHeader = def (path as string) as bool:
	using reader = StringReader(path):
		return (reader.ReadLine() is not null)
	end
end

print NeedsHeader('x')
