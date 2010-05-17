"""
p = { text | print(text) }
tp = { text as string | print(text) }
if 2 > 3:
	print('dough!')
p('Hello')
tp('World!')
"""
p = def (text):
	print(text)
tp = def (text as string):
	print(text)
if 2 > 3:
	print("dough!")
p("Hello")
tp("World!")
