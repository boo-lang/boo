"""
1
after 1
2
after 2
2
"""
struct Vector3:
    x as single
    y as single
    z as single

class Test:
	public transform = Transform()
	
	class Transform:
		[property(position)]
		m_Position = Vector3()
		
	def go():
		yield 1
		transform.position.x += 1
		print("after 1")
		yield 2
		transform.position.x += 1
		print("after 2")
		
test = Test()
for item in test.go():
	print item
print test.transform.position.x
