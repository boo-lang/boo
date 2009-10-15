"""
BCE0032-1.boo(9,19): BCE0032: The event 'BooCompiler.Tests.SupportingClasses.Clickable.Click' expects a callable reference compatible with 'System.EventHandler'.
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

def click(sender, arg as int):
	pass
	
Clickable().Click += click

