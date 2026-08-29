class Size(System.ValueType):
	public Width as int
	public Height as int
	
actual = (
			Size(Width: 320, Height: 200),
			Size(Width: 640, Height: 480))
			
r0 = actual[0]
r1 = actual[1]
actual[0] = r1
actual[1] = r0

expected = (
			Size(Width: 640, Height: 480),
			Size(Width: 320, Height: 200))

assert expected == actual

