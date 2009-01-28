"""
base
nested
nestedNestedExtension
extensionNested < base
nestedExtension
nestedExtensionExtension < base
"""

macro base:
	macro nested:
		yield [| print "nested" |]
		yield
	yield [| print "base" |]
	yield

macro base.nestedExtension:
	yield [| print "nestedExtension" |]
	yield

macro base.nested.nestedNestedExtension:
	assert base.Name == "base"
	assert nested.Name == "nested"

	macro extensionNested:
		assert base.Name == "base"
		assert nested.Name == "nested"
		assert nestedNestedExtension.Name == "nestedNestedExtension"
		yield [| print "extensionNested ${$(base.Arguments[0])}" |]

	yield [| print "nestedNestedExtension" |]
	yield

macro base.nestedExtension.nestedExtensionExtension:
	assert base.Name == "base"
	assert nestedExtension.Name == "nestedExtension"
	yield [| print "nestedExtensionExtension ${$(base.Arguments[0])}" |]


base "< base":
	nested:
		nestedNestedExtension:
			extensionNested
	nestedExtension:
		nestedExtensionExtension

