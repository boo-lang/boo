
closures = [{ return i } for i in range(3)]
	
for expected, closure as callable in zip(range(3), closures):
    assert expected == closure(), "for variables are not shareable"
