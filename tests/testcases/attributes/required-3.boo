"""
class A(object):

	_fld1 = 'abc'

	Property1:
		get:
			return _fld1
		set:
			raise System.ArgumentNullException('value') if (value is null)
			_fld1 = value

	_fld2 = 'def'

	Property2:
		get:
			return _fld2
		set:
			raise System.ArgumentException('Expected: (len(value) < 10)', 'value') unless (len(value) < 10)
			_fld2 = value
"""
class A:
	_fld1 = "abc"
	[Required]
	Property1:
		get:
			return _fld1
		set:
			_fld1 = value
			
	_fld2 = "def"
	[Required(len(value) < 10)]
	Property2:
		get:
			return _fld2
		set:
			_fld2 = value

