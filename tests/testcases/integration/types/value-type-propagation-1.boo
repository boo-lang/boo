"""
10
"""
struct Point:
	public X as int
	public Y as int
	
class Transform:
	public Location = Point()
	
class Component:
	public Transform = Transform()
	
class Program:
	static _component = Component()
	
	def GetComponent():
		return _component
		
	def Run():
		GetComponent().Transform.Location.X = 10
		print _component.Transform.Location.X
		
Program().Run()
