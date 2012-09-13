

class Person:
	[getter(FirstName)]
	_fname as string
	
	[getter(LastName)]
	_lname as string
	
	def constructor([required] lname, [required] fname):
		_lname = lname
		_fname = fname
		
class Employee(Person):
	[getter(HourlyRate)]
	_hourly_rate as int
	
	def constructor([required] lname, [required] fname, [required] hourly_rate):
		super(lname, fname)
		_hourly_rate = hourly_rate

emp = Employee("Joe", "Blow", 10)
assert "Blow" == emp.FirstName
assert "Joe" == emp.LastName
assert 10 == emp.HourlyRate

