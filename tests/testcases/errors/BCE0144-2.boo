"""
BCE0144-2.boo(10,13): BCE0144: 'BooCompiler.Tests.SupportingClasses.ReallyObsoleteClass' is obsolete. I am really obsolete
BCE0144-2.boo(13,18): BCE0144: 'BooCompiler.Tests.SupportingClasses.ReallyObsoleteClass' is obsolete. I am really obsolete
"""


import BooCompiler.Tests.SupportingClasses

class C:
	rocf as ReallyObsoleteClass
	rocif as ReallyObsoleteClass.Inner

	def M(roc as ReallyObsoleteClass, roci as ReallyObsoleteClass.Inner):
		pass