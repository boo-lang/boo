struct Vector3:
	x as single
	y as single
	z as single
	
class Transform:

	[property(position)]
	_position = Vector3(x: 0.0, y: 0.0, z: 0.0)
	
class Base:
	
	[getter(transform)]
	_t = Transform()
	
class Application(Base):
	
	def Run():
		transform.position.x += 10
		transform.position.y += 20
		
		assert 10 == transform.position.x
		assert 20 == transform.position.y
		assert 0 == transform.position.z

		transform.position.z = 42
		assert 42 == transform.position.z
		
Application().Run()


