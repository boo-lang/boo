
/*
An extraneous trailing comma is allowed for array literals.
It makes it easier for when you comment out parts of code.
*/

a1 = (of int: 1,2,3)
a2 = (of int: 1,2,3,)
a3 = (of int:
	1,
	2,
	3,
	)
a4 = (of int:,)
//a5 = (of int:) //not supported
a6 = (,)

b1 = (1,2,3)
b2 = (1,2,3,)

c1 = [1,2,3]
c2 = [1,2,3,]


d1 = {1:true,
	2:false,
	3:true}
d2 = {1:true,
	2:false,
	3:true,
	#4:false
	}

