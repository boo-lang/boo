"""
OnIdle1
OnIdle2
closure
"""
import BooCompiler.Tests

class Test:
	
	def constructor():
		Clickable.Idle += self.OnIdle1
		Clickable.Idle += OnIdle2
		Clickable.Idle += { print("closure") }
		
	def OnIdle1(sender as object, e as System.EventArgs):
        print("OnIdle1")
		
	def OnIdle2():
		print("OnIdle2")
		
t = Test()
Clickable.RaiseIdle()
