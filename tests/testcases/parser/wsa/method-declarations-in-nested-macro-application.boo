"""
myclass Foo:
	override def ToString():
		pass
	myclass Bar:
		override def Equals(other):
			return false
	override def Equals(o):
		return true
"""
myclass Foo:
	override def ToString():
	end
	myclass Bar:
		override def Equals(other):
			return false
		end
	end
	override def Equals(o):
		return true
	end
end
