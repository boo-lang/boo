"""
1
"""
def z():
	iters = array(System.Collections.IEnumerator, 0)
	
	s = [it.MoveNext() for it in iters]
	
	yield 1

for i in z():
	print i
