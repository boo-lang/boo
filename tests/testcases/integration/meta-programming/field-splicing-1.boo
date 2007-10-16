"""
public bar as string = ''
"""
fieldName = "bar"
fieldType = "string"
fieldInitializer = ""
field = [|
	public $fieldName as $fieldType = $fieldInitializer
|]
print field.ToCodeString()


