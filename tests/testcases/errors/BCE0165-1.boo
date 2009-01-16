"""
BCE0165-1.boo(17,11): BCE0165: 'System.ArgumentException' is already handled by except block for 'System.Exception' at (15,11).
BCE0165-1.boo(19,11): BCE0165: 'System.ArgumentNullException' is already handled by except block for 'System.ArgumentException' at (17,11).
BCE0165-1.boo(21,11): BCE0165: 'System.NotImplementedException' is already handled by except block for 'System.Exception' at (15,11).
BCE0165-1.boo(23,11): BCE0165: 'System.ArgumentException' is already handled by except block for 'System.ArgumentException' at (17,11).
"""
import System

x = 1

try:
	print "foo"
except as InvalidOperationException:
	pass
except as Exception:
	pass
except as ArgumentException:
	pass
except as ArgumentNullException:
	pass
except as NotImplementedException:
	pass
except as ArgumentException: #dupe
	pass

try:
	print "bar"
except as ArgumentException if x == 1:
	pass
except as ArgumentException: #not a dupe (filter)
	pass

