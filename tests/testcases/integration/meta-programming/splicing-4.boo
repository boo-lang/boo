"""
def foo():
	print 'before'
	try:
		print('Hello, world')
	ensure:
		print 'after'
"""
method = [|
	def foo():
		print("Hello, world")
|]

method.Body = [|
	print "before"
	try:
		$(method.Body)
	ensure:
		print "after"
|]

print method.ToCodeString()
