"""
bce0053.boo(8,14): BCE0053: Property 'Language.Name' is read only.
bce0053.boo(9,1): BCE0053: Property 'Language.Name' is read only.
"""
class Language:
	[getter(Name)] _name as string
	
p = Language(Name: "boo")
p.Name = "boo"
