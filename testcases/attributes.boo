[XmlElement("Person", Namespace="urn:foo:bar")]
class Person:
	
	enum Class:
		[description("Rich People")]
		A
		
		[description("SoSo People")]
		B
		
		[description("Poor People")]
		C
		
		[description("Les Miserable")]
		D

	[accessors(FirstName)]
	_fname as string
	
	[accessors(LastName)]
	_lname as string
	
	[accessors(DateOfBirth)]
	_dof as date
	
	[accessors(Class)]
	_class as Class
	
	def constructor(
				[required] fname as string,
				[required] lname as string):
		_fname = fname
		_lname = lname		
	
	[memoize]
	[
		before(null != _fname),
		before(null != _lname)
	]
	def GetName() as string [capitalize]:
		return "${_fname} ${_lname}"
		
	[XmlAttribute]
	Age as int:
		[memoize]
		get:
			return (_dof - date.Now).TotalDays
