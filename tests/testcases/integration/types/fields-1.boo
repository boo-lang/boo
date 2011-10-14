
import System.Reflection

class Person:
	
	public FirstName as string	
	
	protected _address as object
	
	internal _birthdate as date
	
	transient private _age as int
	
flags = BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic
type = Person

fname = type.GetField("FirstName", flags)
assert fname is not null
assert fname.IsPublic
assert string is fname.FieldType
assert not fname.IsNotSerialized, "IsNotSerialized"

address = type.GetField("_address", flags)
assert address is not null
assert address.IsFamily
assert object is address.FieldType
assert not address.IsNotSerialized, "IsNotSerialized"

age = type.GetField("_age", flags)
assert age is not null
assert age.IsPrivate
assert int is age.FieldType
assert age.IsNotSerialized, "IsNotSerialized must return true for transient field"

birthdate = type.GetField("_birthdate", flags)
assert birthdate is not null
assert birthdate.IsAssembly
assert date is birthdate.FieldType
assert not birthdate.IsNotSerialized, "IsNotSerialized"
