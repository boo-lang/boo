import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import System.Drawing
import System.IO
import System.Threading

class BooFormattingStrategy(DefaultFormattingStrategy):
	override def SmartIndentLine(area as TextArea, line as int) as int:
		document = area.Document
		previousLine = document.GetLineSegment(line-1)
		
		if document.GetText(previousLine).EndsWith(":"):
			currentLine = document.GetLineSegment(line)
			indentation = GetIndentation(area, line-1)
			indentation += Tab.GetIndentationString(document)
			document.Replace(currentLine.Offset,
							currentLine.Length,
							indentation+document.GetText(currentLine))
			return len(indentation)
		
		return super(area, line)
		
def GetAssemblyFolder():
	return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)

Thread.CurrentThread.ApartmentState = ApartmentState.STA

manager = HighlightingManager.Manager
manager.AddSyntaxModeFileProvider(FileSyntaxModeProvider(GetAssemblyFolder()))
booHighlighter = manager.FindHighlighter("boo")

editor = TextEditorControl(Dock: DockStyle.Fill,
							Font: Font("Lucida Console", 13.0),
							EnableFolding: true)

editor.Document.FormattingStrategy = BooFormattingStrategy()
editor.Document.HighlightingStrategy = booHighlighter

f = Form(Text: "Boo Explorer", Size: Size(800, 600))
f.Controls.Add(editor)

if len(argv):
	editor.LoadFile(Path.GetFullPath(argv[0]))

Application.Run(f)
