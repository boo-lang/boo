"""
start
inside
uh, oh
leaving...
start
inside
leaving...
done
"""
i = 0
:start
print("start")
try:
	print("inside")
	try:
		raise "uh, oh" if ++i < 2
	except x:
		print(x.Message)		
		goto start
ensure:
	print("leaving...")
print("done")
