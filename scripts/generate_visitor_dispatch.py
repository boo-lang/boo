# *-* coding: utf-8 *-*

import sys

class NodeInfo:
	def __init__(self, line):
		node = map(str.strip, line.split(':'))
		self.name = node[0]
		if len(node) > 1 and len(node[1]):
			self._children = map(str.strip, node[1].split())
		else:
			self._children = None

	def isComposite(self):
		return not self._children is None			
	
	def getChildrenInfo(self):
		"""
		Enumera todos os filhos e retorna para cada um a
		tupla (IsCollection as bool, Name as string)
		"""
		for child in self._children:
			# quando o nome começa com um * significa que é
			# uma coleção
			if child[0] == "*":
				yield (True, child[1:])
			else:
				yield (False, child)
		
	def __str__(self):
		return "<<name=%s, children=%s>>" % (self.name, self.children)
		
	def __repr__(self):
		return self.__str__()		

		
fname = sys.argv[1]
# Lê todas as linhas com exceção das que são comentários
lines = [s.rstrip() for s in open(fname).readlines() if not s.startswith('#')]
nodes = map(NodeInfo, lines)

print("""
#region Implementação automática
/// Atenção! Este código foi gerado automaticamente pelo script %s!
/// Não altere o código nesta região!
""" % sys.argv[0])

print("static AbstractVisitor()")
print("{")
print("\t_dispatchTable = new Hashtable();")

for node in nodes:
	name = node.name
	print("\t_dispatchTable.Add(typeof(Boo.Ast.%s), new OnNodeDispatcher(AbstractVisitor.On%s));" % (name, name))
	
print("}")

for node in nodes:
	name = node.name
	print
	print("static void On%s(AbstractVisitor visitor, Boo.Ast.Node node)" % name)
	print("{")
	print("\tvisitor.On%s((Boo.Ast.%s)node);" % (name, name))
	print("}")
	if node.isComposite():
		print
		print("protected virtual void On%s(Boo.Ast.%s node)" % (name, name))
		print("{")
		print("\tif (Enter%s(node))" % name)
		print("\t{")
		
		for isCollection, member in node.getChildrenInfo():
			if isCollection:
				print("\t\tOnNodeCollection(node.%s);" % member)
			else:
				print("\t\tOnNode(node.%s);" % member)
				
		print("\t\tLeave%s(node);" % name)
		print("\t}")
		print("}")
		print
		print("protected virtual bool Enter%s(Boo.Ast.%s node)" % (name, name))
		print("{")
		print("\treturn true;")
		print("}")
		print
		print("protected virtual void Leave%s(Boo.Ast.%s node)" % (name, name))
		print("{")
		print("}")
	else:
		print
		print("protected virtual void On%s(Boo.Ast.%s node)" % (name, name))
		print("{")
		print("}")
		
print("#endregion")
