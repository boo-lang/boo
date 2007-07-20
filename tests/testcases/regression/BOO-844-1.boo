"""
Derived.Method
Base.Method
Derived.SetProp
Base.SetProp
"""

import System

class Base:
	virtual Prop as int: 
		get: return 0
		set: print "Base.SetProp"

	virtual def Method():
		print "Base.Method"

class Derived(Base):
	override Prop as int:
		get: return super.Prop
		set: 
			print "Derived.SetProp" 
			super.Prop = value

	override def Method():
		print "Derived.Method"
		super.Method()

d = Derived()
d.Method()
d.Prop = 1
