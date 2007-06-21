"""
start
inside
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
	goto start if ++i < 2
ensure:
	print("leaving...")
print("done")
