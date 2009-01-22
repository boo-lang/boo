"""
Coffee
Tea
Booze
"""

enum Beverage:
	Coffee
	Tea
	Booze

class DrinkAttribute(System.Attribute):
	def constructor(beverage as Beverage):
		_beverage = beverage

	_beverage as Beverage

	override def ToString():
		return _beverage.ToString()


class Period:
	def constructor():
		print self.GetType().GetCustomAttributes(DrinkAttribute, false)[0]

[Drink(Beverage.Coffee)]
class Morning (Period):
	pass

[Drink(Beverage.Tea)]
class Afternoon (Period):
	pass

[Drink(Beverage.Booze)]
class Evening (Period):
	pass

Morning()
Afternoon()
Evening()

