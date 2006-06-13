"""
object: System.Object
Base: Base
Derived: Derived
Derived: DDerived
"""
import System.Collections

class Base:
	pass
	
class Derived(Base):
	pass
	
class DDerived(Derived):
	pass

def use(obj):
	print("object: " + obj.GetType())

def use(obj as Base):
	print("Base: " + obj.GetType())

def use(obj as Derived):
	print("Derived: " + obj.GetType())

use(object())
use(Base())
use(Derived())
use(DDerived())
