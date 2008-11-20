"""
007
"""

public class Class:
	event Event as callable(int) as void
	
	def FireEvent():
		Event(7)

c = Class()
c.Event += { i | print i.ToString("000") }
c.FireEvent()
