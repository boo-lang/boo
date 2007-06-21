struct Vector3:
	x as single
	y as single
	z as single
	
class Transform:
	public position = Vector3(x: 0.0, y: 0.0, z: 0.0)
	
t = Transform()
t.position.x += 10
t.position.y += 20

assert 10 == t.position.x
assert 20 == t.position.y
assert 0 == t.position.z


