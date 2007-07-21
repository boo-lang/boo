"""
Derived.SetProp
Base.SetProp
"""

import System

class Base:
	virtual Prop as int: 
		get: return 0
		set: print "Base.SetProp"

class Derived(Base):
	def Method():
		self.Prop = 1

	override Prop as int:
		get: return super.Prop
		set: 
			print "Derived.SetProp" 
			super.Prop = value

d = Derived()
d.Method()
