import NUnit.Framework
import System.Reflection

class Person:
	
	public FirstName as string	
	
	protected _address as object
	
	internal _birthdate as date
	
	transient private _age as int
	
flags = BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic
type = Person

fname = type.GetField("FirstName", flags)
Assert.IsNotNull(fname, "FirstName")
Assert.IsTrue(fname.IsPublic)
Assert.AreSame(string, fname.FieldType)
Assert.IsFalse(fname.IsNotSerialized, "IsNotSerialized")

address = type.GetField("_address", flags)
Assert.IsNotNull(address, "_address")
Assert.IsTrue(address.IsFamily)
Assert.AreSame(object, address.FieldType)
Assert.IsFalse(address.IsNotSerialized, "IsNotSerialized")

age = type.GetField("_age", flags)
Assert.IsNotNull(age, "_age")
Assert.IsTrue(age.IsPrivate)
Assert.AreSame(int, age.FieldType)
Assert.IsTrue(age.IsNotSerialized, "IsNotSerialized must return true for transient field")

birthdate = type.GetField("_birthdate", flags)
Assert.IsNotNull(birthdate, "_birthdate")
Assert.IsTrue(birthdate.IsAssembly)
Assert.AreSame(date, birthdate.FieldType)
Assert.IsFalse(birthdate.IsNotSerialized, "IsNotSerialized")
