namespace BooExplorer

interface ITextInterceptor:
	Name as string:
		get

	def Process(ch as System.Char, manipulator as TextManipulator) as bool
		
