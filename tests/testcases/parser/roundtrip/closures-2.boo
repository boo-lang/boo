"""
button = Button()
button.Click += callable():
	print('clicked!')

button.Click += callable():
	print('yes, it was!')

button.Click += callable(sender):
	print("\${sender} clicked!")
"""
button = Button()
button.Click += callable:
	print("clicked!")

button.Click += callable():
	print("yes, it was!")
	
button.Click += callable(sender):
	print("${sender} clicked!")
