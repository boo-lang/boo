"""
I have no item
adding item: 0
I have an item!
adding item: 1
I have an item!
adding item: 2
I have an item!
adding item: 3
Did we make it?
"""

list = []

while len(list) < 5:
	for i in list: print 'I have an item!' ; break
	then: print 'I have no item'
	print 'adding item:', len(list)
	list.Add(len(list))
	break if list.Contains(3)
then:
	print "We finished!"
	
print "Did we make it?"