"""
adding item: 0
adding item: 1
adding item: 2
adding item: 3
adding item: 4
Did we make it?
"""

list = []

while len(list) < 5:
	print 'adding item:', len(list)
	list.Add(len(list))
or:
	print "List already big!"
	
print "Did we make it?"