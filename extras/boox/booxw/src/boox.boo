namespace BooExplorer

import System
import System.Reflection
import System.IO
import System.Threading
import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
	
def GetAssemblyFolder():
	return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

[STAThread]
def Main(argv as (string)):
	HighlightingManager.Manager.AddSyntaxModeFileProvider(
		FileSyntaxModeProvider(GetAssemblyFolder()))

	Application.Run(MainForm(argv))
