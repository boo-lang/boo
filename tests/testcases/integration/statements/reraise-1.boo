"""
throw
caught exception; reraising
reraise successfull
"""
import BooCompiler.Tests from BooCompiler.Tests

try:
	try:
		print("throw")
		raise "exception"
	except e:
		print("caught ${e.Message}; reraising")
		raise
		print("reraise failed")
except:
	print("reraise successfull")

