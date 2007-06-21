"""
A => Avish
B => Bamboo
"""
import System.Collections.Generic

public class Person:
	public Name as string

	public def constructor(name as string):
		Name = name

strings = Dictionary[of string, string]()

strings.Add("A", "Avish")
strings.Add("B", "Bamboo")

for kvp /* as KeyValuePair[of string, Person] */ in strings:
	print "${kvp.Key} => ${kvp.Value}"

