"""
class Console:

	def WriteLine(format as string, *args):
		print(string.Format(format, args))

	def WriteLn(format as string, *args as (object)):
		print(string.Format(format, args))
"""
class Console:
	def WriteLine(format as string, *args):
		print(string.Format(format, args))
		
	def WriteLn(format as string, *args as (object)):
		print(string.Format(format, args))
		
