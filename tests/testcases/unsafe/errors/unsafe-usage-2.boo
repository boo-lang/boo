"""
unsafe-usage-2.boo(19,6): BCE0048: Type 'System.Byte*' does not support slicing.
unsafe-usage-2.boo(19,5): BCE0049: Expression cannot be assigned to.
unsafe-usage-2.boo(20,10): BCE0022: Cannot convert 'int' to 'System.Byte*'.
unsafe-usage-2.boo(21,8): BCE0051: Operator '*' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(22,8): BCE0051: Operator '/' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(23,13): BCE0051: Operator '*' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(24,8): BCE0051: Operator '<<' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(25,13): BCE0051: Operator '+' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'System.Byte*'.
unsafe-usage-2.boo(28,1): BCE0005: Unknown identifier: 'bp'.
unsafe-usage-2.boo(28,1): BCE0049: Expression cannot be assigned to.
unsafe-usage-2.boo(29,1): BCE0118: Target of explode expression must be an array.
unsafe-usage-2.boo(29,1): BCE0049: Expression cannot be assigned to.
"""


bytes = array[of byte](16)
unsafe bp as byte = bytes, bp2 as byte = bytes:
	*bp[1] = 1
	bp = 1
	bp *= 1
	bp /= 2
	bp = bp * 1
	bp <<= 1
	bp = bp + bp2
	xbp = bp

bp++
*xbp = 0

