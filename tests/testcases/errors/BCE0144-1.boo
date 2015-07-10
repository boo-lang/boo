"""
BCE0144-1.boo(11,9): BCE0144: 'BooCompiler.Tests.SupportingClasses.ReallyObsoleteClass' is obsolete. I am really obsolete
BCE0144-1.boo(12,9): BCE0144: 'BooCompiler.Tests.SupportingClasses.ReallyObsoleteClass' is obsolete. I am really obsolete
BCE0144-1.boo(14,9): BCE0144: 'BooCompiler.Tests.SupportingClasses.ReallyObsoleteClass' is obsolete. I am really obsolete
"""

import BooCompiler.Tests.SupportingClasses

class C:
	def M():
		ReallyObsoleteClass.si = 42
		ReallyObsoleteClass.Inner.si = 42
		ReallyObsoleteClass().i = 42
		ReallyObsoleteClass.Inner().i = 42