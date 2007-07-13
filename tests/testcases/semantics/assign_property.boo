"""
public class Person(object):

	protected _name as string

	public Name as string:
		public get:
			return self._name
		public set:
			self._name = value

	public def constructor():
		super()

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Assign_propertyModule(object):

	private static def Main(argv as (string)) as void:
		p = Person()
		p.Name = 'boo'
		Boo.Lang.Builtins.print(p.get_Name())

	private def constructor():
		super()
"""
class Person:
	_name as string
	
	Name:
		get:
			return _name
		set:
			_name = value
			
p = Person()
p.Name = "boo"
print(p.Name)
