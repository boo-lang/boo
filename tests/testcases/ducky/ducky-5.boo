"""
one : 4
two : 2
three : 2
four : 2
five : 1
six : 5
"""

l=('one', 'two', 'one', 'three', 'one', 'three', 'two', 'one',
'four', 'six', 'six', 'five', 'six', 'six', 'six', 'four')
d={}
for i in l:
	if i not in d:
		d[i] = 0
	d[i] += 1

for key in ("one","two","three","four","five","six"):
	print key, ":", d[key]

