namespace ITL.WebTools.Controls

import System
import System.Web
import System.Web.UI
import System.Web.UI.WebControls
import Boo.Web


[ParseChildren(ChildrenAsProperties: true)]
class MediaViewer(Control, INamingContainer):
"""
<summary>
Cria a tag HTML apropriada para visualização de qualquer tipo
de arquivo.
</summary>
"""
	[Template(Container: MediaViewer)]
	FlashTemplate			

	[ViewState]
	Path as string

	[ViewState(Default: 70)]
	Width as int

	[ViewState(Default: 70)]
	Height as int
	
	override def OnDataBinding(e as EventArgs):
		super(e)
		EnsureChildControls()
	
	override def CreateChildControls():
		super()
		
		return unless Path
		
		extension = System.IO.Path.GetExtension(path).ToLower()
		given extension:
			when ".jpg", ".gif", ".bmp", ".png":
				Controls.Add(LiteralControl("<img src='${Path}' width='${Width}' height='${Height}' />"))
				
			when ".swf":
				if FlashTemplate:
					FlashTemplate.InstantiateIn(self)
				else:
					Controls.Add(
						LiteralControl("<embed src='${Path}' quality='low' width='${Width}' height='${Height}'></embed>")
						)
						
			otherwise:
				Controls.Add(LiteralControl("<b>A extensão ${extension} não é suportada.</b>"))
