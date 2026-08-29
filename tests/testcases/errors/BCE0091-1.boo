"""
BCE0091-1.boo(9,16): BCE0091: Event reference 'Button.Click' cannot be used as an expression.
"""
import System

class Button:
	event Click as EventHandler
	
print(Button().Click)
