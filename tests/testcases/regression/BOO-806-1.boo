"""
4
8
"""

#dummy test just to check if it compiles and emits valid IL
class Test:
	[property(X)]
	[volatile]
	public x as int = 23

	[volatile]
	public static sx as int = 42


t = Test()
t.x = 4
Test.sx = 8

print t.x
print Test.sx

