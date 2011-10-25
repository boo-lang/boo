
closures = []
for i in range(3):
	closures.Add({ return i })
	
for expected, closure as callable in zip(range(3), closures):
	assert expected == closure(), "for variables are not shareable"
