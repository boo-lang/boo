"""
adding item: 0
adding item: 1
adding item: 2
adding item: 3
adding item: 4
We finished!
Did we make it?
"""

list = []

while len(list) < 5:
	print 'adding item:', len(list)
	list.Add(len(list))
then:
	print "We finished!"
	
print "Did we make it?"