"""
event0.boo(9,19): BCE0032: The event 'Boo.Lang.Compiler.Tests.Clickable.Click' expects a callable reference compatible with 'System.EventHandler'.
"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

def click(sender, arg as int):
	pass
	
Clickable().Click += click

