"""
button = Button()
button.Click += def ():
	print('clicked!')

button.Click += def ():
	print('yes, it was!')

button.Click += def (sender):
	print("\${sender} clicked!")
"""
button = Button()
button.Click += def:
	print("clicked!")

button.Click += def ():
	print("yes, it was!")
	
button.Click += def (sender):
	print("${sender} clicked!")
