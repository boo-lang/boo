"""
button = Button()
button.Click += callable():
	print('clicked!')

if button:
	button.Click += callable():
		print('yes, it was!')

	if (3 > 2):
		button.Click += callable(sender):
			print("\${sender} clicked!")
"""
button = Button()
button.Click += callable:
	print("clicked!")
if button:
	button.Click += callable():
		print("yes, it was!")
	if 3 > 2:
		button.Click += callable(sender):
			print("${sender} clicked!")

