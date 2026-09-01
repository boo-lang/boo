namespace BooInteropLib

import System
import System.Collections.Generic

public interface IShape:
	def Area() as double

public abstract class Shape(IShape):
	public abstract def Area() as double:
		pass

	public def Describe() as string:
		return "${GetType().Name}:${Area()}"

public class Circle(Shape):
	public Radius as double

	public def constructor():
		pass

	public def constructor(radius as double):
		Radius = radius

	public override def Area() as double:
		return Math.PI * Radius * Radius

public class Registry[of T(IShape)]:
	_items = List[of T]()

	public def Add(item as T):
		_items.Add(item)

	public def Total() as double:
		total = 0.0
		for item in _items:
			total += item.Area()
		return total

	public Count as int:
		get: return _items.Count
