"""
button = Button()
button.Click += do ():
	print('clicked!')

if button:
	button.Click += do ():
		print('yes, it was!')

	if (3 > 2):
		button.Click += do (sender):
			print("\${sender} clicked!")
"""
button = Button()
button.Click += do:
	print("clicked!")
if button:
	button.Click += do ():
		print("yes, it was!")
	if 3 > 2:
		button.Click += do (sender):
			print("${sender} clicked!")

