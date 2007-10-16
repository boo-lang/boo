"""
public bar as string
"""
fieldName = "bar"
field = [|
	public $fieldName as string
|]
print field.ToCodeString()


