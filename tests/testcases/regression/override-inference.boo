"""
foo
"""
class PegRule:
	pass
	
class PegRuleState:
	def EnterRule():
		return PegRuleStateNested(self)
	
	virtual def LastMatchFor(rule as PegRule):
		return string.Empty
		
	virtual def LeaveRule(rule as PegRule, success as bool) as PegRuleState:
		assert false
		
class PegRuleStateNested(PegRuleState):

	_parent as PegRuleState
		
	def constructor(parent as PegRuleState):
		_parent = parent
		
	override def LeaveRule(rule as PegRule, success as bool):
		if success: return PegRuleStateMatched(_parent, rule)
		return _parent
	
	override def LastMatchFor(rule as PegRule):
		return _parent.LastMatchFor(rule)
		
class PegRuleStateMatched(PegRuleStateNested):

	_rule as PegRule
	
	def constructor(parent as PegRuleState, rule as PegRule):
		super(parent)
		_rule = rule
		
	override def LastMatchFor(rule as PegRule):
		if rule is _rule: return "foo"
		return super(rule)
	
rule1 = PegRule()
rule2 = PegRule()
state = PegRuleStateMatched(PegRuleState(), rule1)
assert string.Empty == state.LastMatchFor(rule2)
print state.LastMatchFor(rule1)
