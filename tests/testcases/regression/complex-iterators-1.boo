import System.Collections.Generic

enum node_iter:
	answer

def x_iter():
	yield node_iter.answer

class b:
	public iter as IEnumerator of node_iter
	
	node as string
	
	semobj as string
	
	public cycle as bool?
	
	def iter_node():
	
		semobj = node //   exception here
		
		cycle = null
		
		yield node_iter.answer
	
	def setIter():
	
		if node:
			iter = iter_node().GetEnumerator()
		else:
			iter = x_iter().GetEnumerator()
	
	def constructor():
		node = "1"
		setIter()

bo = b()
while bo.iter.MoveNext():
	pass
