"""
button = Button()
button.Click += { print('clicked!') }
if button:
	button.Click += { print('yes, it was!') }
	if 3 > 2:
		button.Click += { sender | print("\${sender} clicked!") }
"""
button = Button()
button.Click += def:
	print("clicked!")
if button:
	button.Click += def ():
		print("yes, it was!")
	if 3 > 2:
		button.Click += def (sender):
			print("${sender} clicked!")

