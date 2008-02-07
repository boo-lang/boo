"""
BCE0018-3.boo(6,2): BCE0064: No attribute with the name 'System.Runtim.InterpService.DllImpor' or 'System.Runtim.InterpService.DllImporAttribute' was found (attribute names are case insensitive). Did you mean 'System.Runtime.InteropServices.DllImportAttribute' ?
BCE0018-3.boo(10,2): BCE0064: No attribute with the name 'Nonsense' or 'NonsenseAttribute' was found (attribute names are case insensitive).
"""

[System.Runtim.InterpService.DllImpor("libc")]
def TestAttributeSuggestion():
	pass

[Nonsense("libc")]
def TestAttributeSuggestion():
        pass
