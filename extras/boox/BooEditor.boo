namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import WeifenLuo.WinFormsUI
import System
import System.ComponentModel
import System.Windows.Forms
import System.Drawing
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipeline.Definitions
import Boo.AntlrParser
import Boo.Lang.Compiler.Ast

class BooEditor(Content):

	_editor as TextEditorControl
	_main as MainForm

	[getter(FileName)]
	_fname as string

	[getter(IsDirty)]
	_dirty = false

	_moduleDirty = true

	[getter(Module)]
	_module as Module

	def constructor(main as MainForm):
		_main = main
		_editor = TextEditorControl(Dock: DockStyle.Fill,
							Font: System.Drawing.Font("Lucida Console", 13.0),
							EnableFolding: true)

		_editor.Encoding = System.Text.Encoding.UTF8
		_editor.Document.FormattingStrategy = BooFormattingStrategy()
		_editor.Document.HighlightingStrategy = GetBooHighlighting()
		_editor.Document.DocumentChanged += _editor_DocumentChanged

		SuspendLayout()
		Controls.Add(_editor)
		self.HideOnClose = true
		self.AllowedStates = ContentStates.Document
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
		savedCursor = Cursor
		self.Cursor = Cursors.WaitCursor
		try:
			Run()
		ensure:
			self.Cursor = savedCursor

	private def Run():
		
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput(GetSafeFileName(), self.TextContent))
		compiler.Parameters.Pipeline.Load(BoomPipelineDefinition)
		compiler.Parameters.References.Add(typeof(Form).Assembly)
		compiler.Parameters.References.Add(typeof(System.Drawing.Size).Assembly)

		started = date.Now
		result = compiler.Run()
		finished = date.Now
		_main.StatusText = "Compilation finished in ${finished-started} with ${len(result.Errors)} error(s)."

		if len(result.Errors):
			print(join(result.Errors, "\n"))
		else:
			try:
				result.GeneratedAssemblyEntryPoint.Invoke(null, (null,))
			except x:
				print(x)

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


