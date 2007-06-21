"""
2
1
"""
class foo:
	virtual Sunny:
		get:
			return 1

class bar(foo):
	override Sunny:
		get:
			return 2
			
	def rock():
		print Sunny
		print super.Sunny
		
bar().rock()
