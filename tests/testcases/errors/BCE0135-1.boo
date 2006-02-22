"""
BCE0135-1.boo(25,7): BCE0135: Invalid name: '@C'
BCE0135-1.boo(28,13): BCE0135: Invalid name: '?a'
BCE0135-1.boo(28,24): BCE0135: Invalid name: '@b'
BCE0135-1.boo(28,13): BCE0135: Invalid name: '?a'
BCE0135-1.boo(28,24): BCE0135: Invalid name: '@b'
BCE0135-1.boo(28,10): BCE0135: Invalid name: '$c'
BCE0135-1.boo(31,11): BCE0135: Invalid name: '$E'
BCE0135-1.boo(32,5): BCE0135: Invalid name: '@z'
BCE0135-1.boo(33,15): BCE0135: Invalid name: '?a'
BCE0135-1.boo(34,15): BCE0135: Invalid name: '?a'
BCE0135-1.boo(33,9): BCE0135: Invalid name: '@doit'
BCE0135-1.boo(30,7): BCE0135: Invalid name: '?wuh'
BCE0135-1.boo(35,1): BCE0135: Invalid name: '$i'
BCE0135-1.boo(36,7): BCE0135: Invalid name: '$i'
BCE0135-1.boo(37,1): BCE0135: Invalid name: '?w'
BCE0135-1.boo(37,6): BCE0135: Invalid name: '?wuh'
BCE0135-1.boo(38,1): BCE0135: Invalid name: '?w'
BCE0135-1.boo(38,4): BCE0135: Invalid name: '@doit'
BCE0135-1.boo(38,10): BCE0135: Invalid name: '$i'
"""

import System

class @C:
	pass

callable $c(?a as int, @b as string) as int

class ?wuh:
	event $E as EventHandler
	@z as int
	def @doit(?a as int):
		print ?a
$i = 10
print $i / 2
?w = ?wuh()
?w.@doit($i)

print @/\s/.Split("a string")

