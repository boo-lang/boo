namespace BooCompiler.Tests.SupportingClasses

import System

class Clickable:
	public event Click as EventHandler

	public static event Idle as EventHandler

	def constructor():
		pass

	public def RaiseClick():
		if Click is not null:
			Click(self, EventArgs.Empty)

	public static def RaiseIdle():
		if Idle is not null:
			Idle(null, EventArgs.Empty)
