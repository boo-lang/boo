"""
Bar1.get_Count
1
Bar1.Zeng
Bar2.get_Count
2
Bar2.Zeng
"""
interface IFoo:
	Count as int:
		get
		
interface IBar(IFoo):
	def Zeng()
	
class Bar1(IBar):
	Count as int:
		get:
			print("Bar1.get_Count")
			return 1
	def Zeng():
		print("Bar1.Zeng")
		
class Bar2(IBar):
	Count as int:
		get:
			print("Bar2.get_Count")
			return 2
	def Zeng():
		print("Bar2.Zeng")
		
def use(bar as IBar):
	print(bar.Count)
	bar.Zeng()
	
use(Bar1())
use(Bar2())
