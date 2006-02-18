"""
StatemachineBase.constructor
StatemachineBase._S_OFF_
StatemachineBase._S_AUTO_
---
StatemachineBase.constructor
StatemachineDerived._S_OFF_
StatemachineDerived._S_AUTO_
StatemachineDerived.constructor
StatemachineDerived._S_AUTO_
StatemachineDerived._S_OFF_
"""
class StatemachineBase:
	
	state as callable = self._S_OFF_
	
	def constructor():
		print "StatemachineBase.constructor"
		state()
		state()
	
	virtual def _S_OFF_():
		print "StatemachineBase._S_OFF_"
		self.state = self._S_AUTO_
	
	virtual def _S_AUTO_():
		print "StatemachineBase._S_AUTO_"
		self.state = _S_OFF_

class StatemachineSubclass(StatemachineBase):
	pass
	
class StatemachineDerived(StatemachineSubclass):
	def constructor():
		super()
		print "StatemachineDerived.constructor"
		self.state = self._S_AUTO_ //prev. gave ambiguous error
		state()
		state()
	
	def _S_OFF_():
		print "StatemachineDerived._S_OFF_"
		//super._S_OFF_()
		self.state = self._S_AUTO_ //prev. gave ambiguous error
	
	def _S_AUTO_():
		print "StatemachineDerived._S_AUTO_"
		//super._S_AUTO_()
		self.state = _S_OFF_ //prev. gave ambiguous error

b = StatemachineBase()
print "---"
b = StatemachineDerived()


