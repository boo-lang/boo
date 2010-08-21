"""
return [|
	[Boo.Lang.ExtensionAttribute]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
	static def Foo(parent as \$(parent.FullName)) as \$(extension):
		return \$(ReferenceExpression(extension))(context)
|]
"""
return [|
	[Boo.Lang.ExtensionAttribute]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
	static def Foo(parent as $(parent.FullName)) as $(extension):
		return $(ReferenceExpression(extension))(context)
	end
|]
