"""
{ print 'Merry Christmas, I don\'t want to fight tonite' }()
"""
import Boo.Lang.Compiler

block = [|
	block:
		print "Merry Christmas, I don't want to fight tonite"
|].Block
print [| $block() |]
