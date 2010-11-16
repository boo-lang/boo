"""
unsafe-usage-2.boo(15,6): BCE0048: Type 'System.Byte*' does not support slicing.
unsafe-usage-2.boo(16,10): BCE0022: Cannot convert 'int' to 'System.Byte*'.
unsafe-usage-2.boo(17,8): BCE0051: Operator '*' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(18,8): BCE0051: Operator '/' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(19,13): BCE0051: Operator '*' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(20,8): BCE0051: Operator '<<' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'int'.
unsafe-usage-2.boo(21,13): BCE0051: Operator '+' cannot be used with a left hand side of type 'System.Byte*' and a right hand side of type 'System.Byte*'.
unsafe-usage-2.boo(24,1): BCE0005: Unknown identifier: 'bp'.
unsafe-usage-2.boo(25,1): BCE0118: Target of explode expression must be an array.
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

