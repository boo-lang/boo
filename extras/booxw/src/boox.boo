namespace BooExplorer

import System.Reflection
import System.IO
import System.Threading
import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
	
def GetAssemblyFolder():
	return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

Thread.CurrentThread.ApartmentState = ApartmentState.STA

HighlightingManager.Manager.AddSyntaxModeFileProvider(
		FileSyntaxModeProvider(GetAssemblyFolder()))

Application.Run(MainForm(argv))
