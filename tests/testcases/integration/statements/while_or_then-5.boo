"""
adding item: 4
Did we make it?
"""

list = [1, 2, 3, 4]

while len(list) < 5:
	print 'adding item:', len(list)
	list.Add(len(list))
	break if list.Contains(3)
or:
	print "List already big!"
then:
	print "We made it!"
	
print "Did we make it?"