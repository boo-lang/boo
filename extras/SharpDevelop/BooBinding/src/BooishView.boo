namespace BooBinding

import System
import System.Drawing
import System.Windows.Forms
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.Core.Services
import booish.gui

class BooishView(AbstractPadContent):
	
	_box = PromptBox(Font: System.Drawing.Font("Lucida Console", 10))
	
	def constructor():
		super("booish")
		
		_box.Interpreter.SetValue("Workbench", WorkbenchSingleton.Workbench)
		
	override Control as Control:
		get:
			return _box

