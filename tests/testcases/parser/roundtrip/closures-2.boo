"""
button = Button()
button.Click += do ():
	print('clicked!')

button.Click += do ():
	print('yes, it was!')

button.Click += do (sender):
	print("\${sender} clicked!")
"""
button = Button()
button.Click += do:
	print("clicked!")

button.Click += do ():
	print("yes, it was!")
	
button.Click += do (sender):
	print("${sender} clicked!")
