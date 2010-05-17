namespace ITL.Content

interface IContentItem:
	Parent as IContentItem:
		get:
			pass
			
	// Sintaxe alternativa para propriedades em
	// um interface (sem necessidade de bloco)
	Name as string:
		get
		set

	// Métodos podem ser declarados com um corpo vazio
	def SelectItem(expression as string) as IContentItem:
		pass
		
	// mas podem ser declarados tbém sem corpo
	def Validate()
	
	def OnRemove()
