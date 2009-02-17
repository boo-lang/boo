"""
42
"""
namespace Boo.Lang

class OutOfOrder:
	def Run():
		print List().Value

class List:
	Value:
		get: return 42
		
OutOfOrder().Run()
