"""
Este módulo implementa várias classes interessantes.
"""
namespace Foo.Bar
"""
Esta é a docstring do namespace.
"""

class Person:
"""
Esta é a docstring da classe
que pode conter várias linhas.
"""
	_fname as string
	"""Campos tbém podem ter docstring"""
	
	def constructor([required] fname as string):
	"""
	Um método qualquer também pode especificar docstrings.
	"""
		_fname = fname
		
	FirstName as string:
	"""Uma propriedade, por que não?"""
		get:
			return _fname
