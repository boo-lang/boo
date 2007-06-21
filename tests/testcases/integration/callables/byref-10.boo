"""
3 True
4 True
"""


x = do(ref x as int):
	x = 3
	
y = { ref x as int | x = 4; return 4 }

a = 1
b = 2

x(a)
print a, a==3
y(b)
print b, b==4

//correctly throws error:
/*
y2 = {ref x as object | x = 5}
b = 2
y2(b)
print b, b==5
*/

