"""
[XmlElement('Person', Namespace: 'urn:foo:bar')]
class Person:

	enum Status:

		[description('Rich People')]
		A

		[description('SoSo People')]
		B

		[description('Poor People')]
		C

		[description('Les Miserable')]
		D

	[accessors(FirstName)]
	_fname as string

	[accessors(LastName)]
	_lname as string

	[accessors(DateOfBirth)]
	_dof as date

	[accessors(Status)]
	_class as Status

	def constructor([required] fname as string, [required] lname as string):
		_fname = fname
		_lname = lname

	[memoize]
	[before((null != _fname))]
	[before((null != _lname))]
	def GetName() [capitalize]  as string:
		return "\${_fname} \${_lname}"

	[XmlAttribute]
	Age as int:
		[memoize]
		get:
			return (_dof - date.Now).TotalDays

	[XmlIgnore]
	Property[[required] key as string]:
		get:
			return null

"""
[XmlElement('Person', Namespace: 'urn:foo:bar')]
class Person:

	enum Status:

		[description('Rich People')]
		A

		[description('SoSo People')]
		B

		[description('Poor People')]
		C

		[description('Les Miserable')]
		D
	end

	[accessors(FirstName)]
	_fname as string

	[accessors(LastName)]
	_lname as string

	[accessors(DateOfBirth)]
	_dof as date

	[accessors(Status)]
	_class as Status

	def constructor([required] fname as string, [required] lname as string):
		_fname = fname
		_lname = lname
	end

	[memoize]
	[before((null != _fname))]
	[before((null != _lname))]
	def GetName() [capitalize] as string:
		return "${_fname} ${_lname}"
	end

	[XmlAttribute]
	Age as int:
		[memoize]
		get:
			return (_dof - date.Now).TotalDays
		end
	end

	[XmlIgnore]
	Property[[required] key as string]:
		get:
			return null
		end
	end
end

