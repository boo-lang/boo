"""
no param
one param
"""
class SmartCallableAdaptionTest():
	_thingy as callable;
	callable Parameterfull(parameter)
	callable Parameterless()
	def constructor(start as Parameterless):
		_thingy = start
	def constructor(start as Parameterfull):
		_thingy = start
	def Go():
		_thingy()
	def Go(argument):
		_thingy(argument)
adaption = SmartCallableAdaptionTest({ print 'no param' })
adaption.Go()
adaption = SmartCallableAdaptionTest({ param | print "${param} param"})
adaption.Go('one')
