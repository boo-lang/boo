namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import WeifenLuo.WinFormsUI
import System
import System.IO
import System.ComponentModel
import System.Windows.Forms
import System.Drawing
import Boo.AntlrParser
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast

class WaitCursor(IDisposable):
	_saved
	_form as Form
	
	def constructor([required] form as Form):
		_form = form
		_saved = form.Cursor		
		form.Cursor = Cursors.WaitCursor
		
	def Dispose():
		_form.Cursor = _saved

class ConsoleCapture(IDisposable):	
	_console = StringWriter()
	_old
	
	def constructor():
		_old = Console.Out
		Console.SetOut(_console)
		
	override def ToString():
		return _console.ToString()
	
	def Dispose():
		Console.SetOut(_old)

class BooEditor(DockContent):

	_editor as TextEditorControl
	_main as MainForm

	[getter(FileName)]
	_fname as string

	[getter(IsDirty)]
	_dirty = false

	_moduleDirty = true

	[getter(Module)]
	_module as Module
	
	_compiler as BooCompiler

	def constructor(main as MainForm):
		_main = main
		_editor = TextEditorControl(Dock: DockStyle.Fill,
							Font: System.Drawing.Font("Lucida Console", 12.0),
							EnableFolding: true)

		_editor.Encoding = System.Text.Encoding.UTF8
		_editor.Document.FormattingStrategy = BooFormattingStrategy()
		_editor.Document.HighlightingStrategy = GetBooHighlighting()
		_editor.Document.DocumentChanged += _editor_DocumentChanged

		SuspendLayout()
		Controls.Add(_editor)
		self.HideOnClose = false
		self.DockableAreas = DockAreas.Document
		self.Text = GetSafeFileName()
		self.DockPadding.All = 1
		self.Menu = CreateMenu()
		ResumeLayout(false)

	TextArea:
		get:
			return _editor.ActiveTextAreaControl.TextArea

	TextContent:
		get:
			return _editor.Document.TextContent

	def GoTo(line as int):
		document = _editor.Document
		segment = document.GetLineSegment(line)
		wsLen = /\s*/.Match(document.GetText(segment)).Groups[0].Length
		_editor.ActiveTextAreaControl.JumpTo(line, wsLen)
		self.TextArea.Focus()
		self.TextArea.Select()

	def Save():
		if _fname:
			_editor.SaveFile(_fname)
			ClearDirtyFlag()
		else:
			SaveAs()

	def SaveAs():
		dlg = SaveFileDialog(AddExtension: true,
							DefaultExt: ".boo",
							OverwritePrompt: true,
							Filter: "boo files (*.boo)|*.boo")
		if DialogResult.OK == dlg.ShowDialog():
			_editor.SaveFile(dlg.FileName)
			_fname = dlg.FileName
			ClearDirtyFlag()

	def Open([required] fname as string):
		_editor.LoadFile(fname)
		_fname = fname
		ClearDirtyFlag()

	def ClearDirtyFlag():
		_dirty = false
		self.Text = _fname

	def _editor_DocumentChanged(sender, args as DocumentEventArgs):
		_moduleDirty = true
		if not _dirty:
			self.Text = "${GetSafeFileName()} (modified)"
			_dirty = true

	def _menuItemUndo_Click(sender, args as EventArgs):
		_editor.Undo()

	def _menuItemRedo_Click(sender, args as EventArgs):
		_editor.Redo()

	def _menuItemSplit_Click(sender, args as EventArgs):
		_editor.Split()

	def _menuItemRemoveTrailingWS_Click(sender, args as EventArgs):
		RemoveTrailingWS().Execute(TextArea)

	def _menuItemCut_Click(sender, args as EventArgs):
		Cut().Execute(TextArea)

	def _menuItemCopy_Click(sender, args as EventArgs):
		Copy().Execute(TextArea)

	def _menuItemPaste_Click(sender, args as EventArgs):
		Paste().Execute(TextArea)

	def _menuItemGoTo_Click(sender, args as EventArgs):
		dlg = PromptDialog(Text: GetSafeFileName(), Message: "Line number: ")
		if DialogResult.OK == dlg.ShowDialog():
			GoTo(int.Parse(dlg.Value)-1)

	def _menuItemRun_Click(sender, args as EventArgs):
		using WaitCursor(self):
			Run()

	private def Run():
		
		if _compiler is null:
			_compiler = BooCompiler()		
			_compiler.Parameters.Pipeline = CompileToMemory()
			_compiler.Parameters.References.Add(typeof(Form).Assembly)
			_compiler.Parameters.References.Add(typeof(System.Drawing.Size).Assembly)
			_compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
			
		_compiler.Parameters.Input.Add(StringInput(GetSafeFileName(), self.TextContent))

		try:
			using console=ConsoleCapture():
				
				started = date.Now
				result = _compiler.Run()
				finished = date.Now
				_main.StatusText = "Compilation finished in ${finished-started} with ${len(result.Errors)} error(s)."
		
				ClearTaskList()
				if len(result.Errors):
					UpdateTaskList(result.Errors)
				else:			
					try:
						result.GeneratedAssemblyEntryPoint.Invoke(null, (null,))
					except x:
						print(x)		
				
				UpdateOutputPane(console.ToString())
		ensure:
			_compiler.Parameters.Input.Clear()
	
	def UpdateOutputPane(text as string):
		_main.OutputPane.SetBuildText(text)
		_main.ShowOutputPane() if len(text)
		
	def UpdateTaskList(errors as CompilerErrorCollection):
		_main.TaskList.Clear()		
		for error in errors:
			_main.TaskList.AddCompilerError(error)
		_main.ShowTaskList()

	def ClearTaskList():
		_main.TaskList.Clear()

	def UpdateModule():
		return unless _moduleDirty

		fname = GetSafeFileName()
		code = self.TextContent
		try:
			_module = BooParser.ParseString(fname, code).Modules[0]
			_moduleDirty = false
		except x:
			print(x)

	override protected def OnClosing(args as CancelEventArgs):
		super(args)
		if (not args.Cancel) and _dirty:
			result = MessageBox.Show("Save changes to ${GetSafeFileName()}?",
									"File not saved",
									MessageBoxButtons.YesNoCancel)
			if DialogResult.Yes == result:
				Save()
			else:
				args.Cancel = DialogResult.Cancel == result

	def GetSafeFileName():
		return _fname if _fname
		return "untitled.boo"

	def CreateMenu():
		menu = MainMenu()

		edit = MenuItem(Text: "&Edit", MergeOrder: 1)
		edit.MenuItems.AddRange(
			(
				MenuItem("&Undo",
						Click: _menuItemUndo_Click,
						Shortcut: Shortcut.CtrlZ),
				MenuItem("&Redo",
						Click: _menuItemRedo_Click,
						Shortcut: Shortcut.CtrlY),
				MenuItem("-"),
				MenuItem("Cu&t",
						Click: _menuItemCut_Click,
						Shortcut: Shortcut.CtrlX),
				MenuItem("&Copy",
						Click: _menuItemCopy_Click,
						Shortcut: Shortcut.CtrlC),
				MenuItem("&Paste",
						Click: _menuItemPaste_Click,
						Shortcut: Shortcut.CtrlV),
				MenuItem("-"),
				MenuItem("&Go to line...",
						Shortcut: Shortcut.CtrlG,
						Click: _menuItemGoTo_Click),
				MenuItem("&Split", Click: _menuItemSplit_Click),
				MenuItem("Remove trailing whitespace", Click: _menuItemRemoveTrailingWS_Click)
			))

		script = MenuItem(Text: "&Script", MergeOrder: 2)
		script.MenuItems.Add(MenuItem(Text: "Run",
									Click: _menuItemRun_Click,
									Shortcut: Shortcut.F5))

		menu.MenuItems.AddRange((edit, script))
		return menu

	def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("boo")
		
	override protected def GetPersistString():
		return "BooEditor|${GetSafeFileName()}"


