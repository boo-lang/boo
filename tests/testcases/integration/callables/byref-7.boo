"""
6 7 True True
11 12 True True
1 2 True True
"""

struct MyStruct:
	X as int
	Y as int
	def constructor(x as int, y as int):
		X = x
		Y = y

def teststruct(ref s as MyStruct):
	s.X = 6
	s.Y = 7
	
def teststruct2(ref s as MyStruct):
	s = MyStruct(11, 12)

def teststruct3(ref s as MyStruct):
	m as MyStruct = s
	m = m //just to avoid warning
	
s = MyStruct(1,2)
teststruct(s)
print s.X, s.Y, s.X==6, s.Y==7

s2 = MyStruct(1,2)
teststruct2(s2)
print s2.X, s2.Y, s2.X==11, s2.Y==12

s3 = MyStruct(1,2)
teststruct3(s3)
print s3.X, s3.Y, s3.X==1, s3.Y==2

