"""
public bar as string:
	get:
		return _bar
"""
import Boo.Lang.Compiler

name = "bar"
type = "string"
value = [| _bar |]
code = [|
	public $name as $type:
		get:
			return $value
|]
print code.ToCodeString()


