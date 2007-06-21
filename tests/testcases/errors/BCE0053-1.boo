"""
BCE0053-1.boo(8,14): BCE0053: Property 'Language.Name' is read only.
BCE0053-1.boo(9,1): BCE0053: Property 'Language.Name' is read only.
"""
class Language:
	[getter(Name)] _name as string
	
p = Language(Name: "boo")
p.Name = "boo"
