#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

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
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import BooExplorer.Common

class WaitCursor(IDisposable):
	_saved
	_form as Form
	
	def constructor([required] form as Form):
		_form = form
		_saved = form.Cursor		
		form.Cursor = Cursors.WaitCursor
		
	def Dispose():
		_form.Cursor = _saved

class BooEditor(DockContent):

	_editor as BooxTextAreaControl

	[getter(Main)]
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
		settings = main.Settings
		_editor = BooxTextAreaControl(Dock: DockStyle.Fill,
							Font: settings.TextFont,
							EnableFolding: settings.EnableFolding,
							ShowLineNumbers: settings.ShowLineNumbers,
							ShowSpaces: settings.ShowSpaces,
							ShowTabs: settings.ShowTabs,
							ShowEOLMarkers: settings.ShowEOLMarkers,
							IndentStyle: settings.IndentStyle)

		_editor.Editor = self
		_editor.Encoding = System.Text.Encoding.UTF8
		_editor.Document.FormattingStrategy = BooFormattingStrategy()
		_editor.Document.HighlightingStrategy = GetBooHighlighting()
		_editor.Document.DocumentChanged += _editor_DocumentChanged
		_editor.Document.FoldingManager.FoldingStrategy = BooFoldingStrategy.Instance

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
			
		set:
			_editor.Document.TextContent = value

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
			
	def _menuItemExpand_Click(sender, args as EventArgs):
		using WaitCursor(self):
			Expand()
			
	private def Expand():
		_main.Expand(self.GetSafeFileName(), self.TextContent)
		
	private def Run():
		
		if _compiler is null:
			_compiler = BooCompiler()		
			// enable duck typing
			_compiler.Parameters.Ducky = _main.Settings.Ducky
			_compiler.Parameters.Pipeline = CompileToMemory()
			_compiler.Parameters.References.Add(typeof(Form).Assembly)
			_compiler.Parameters.References.Add(typeof(System.Drawing.Size).Assembly)
			_compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
			_compiler.Parameters.References.Add(typeof(Boo.Lang.Extensions.MacroMacro).Assembly)
			
		_compiler.Parameters.Input.Add(StringInput(GetSafeFileName(), self.TextContent))

		try:
			using console=Boo.Lang.Interpreter.ConsoleCapture():
				
				started = date.Now
				result = _compiler.Run()
				finished = date.Now
				_main.StatusText = "Compilation finished in ${finished-started} with ${len(result.Errors)} error(s)."
		
				_main.UpdateTaskList(result.Errors)
				
				if len(result.Errors):
					UpdateOutputPane(result.Errors.ToString(true))
				else:
					
					AppDomain_AssemblyResolve = def (sender, args as ResolveEventArgs):
						name = GetSimpleName(args.Name)
						compiledName = GetSimpleName(result.GeneratedAssembly.FullName)
						return result.GeneratedAssembly if name == compiledName
						
					current = AppDomain.CurrentDomain
					try:
						current.AssemblyResolve += AppDomain_AssemblyResolve
						result.GeneratedAssembly.EntryPoint.Invoke(null, (null,))
					except x:
						print(x)		
					ensure:
						current.AssemblyResolve -= AppDomain_AssemblyResolve				
					UpdateOutputPane(console.ToString())
		ensure:
			_compiler.Parameters.Input.Clear()
			
	private def GetSimpleName(name as string):
		return /,\s*/.Split(name)[0]
	
	def UpdateOutputPane(text as string):
		_main.OutputPane.SetBuildText(text)
		_main.ShowOutputPane() if len(text)
		
	def UpdateFoldings(fname as string, parseInformation):
		_editor.Document.FoldingManager.UpdateFoldings(fname, parseInformation)
		
	def UpdateModule():
		return unless _moduleDirty

		fname = GetSafeFileName()
		code = self.TextContent
		try:
			_module = _main.ParseString(fname, code).Modules[0]
			_moduleDirty = false
			UpdateFoldings(fname, _module)
		except x:
			print(x)

	override protected def OnClosing(args as CancelEventArgs):
		super(args)
		return if args.Cancel or (not _dirty) or _main.IsQuitting
		
		result = MessageBox.Show("Save changes to ${GetSafeFileName()}?",
								"File not saved",
								MessageBoxButtons.YesNoCancel)
		if DialogResult.Yes == result:
			Save()
			
		if result in (DialogResult.Yes, DialogResult.No):
			_dirty = false
			args.Cancel = false
		else:
			args.Cancel = true

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

		tools = MenuItem(Text: "&Tools", MergeOrder: 2, MergeType: MenuMerge.MergeItems)
		tools.MenuItems.AddRange(
			(				
				MenuItem(Text: "Run",
						Click: _menuItemRun_Click,
						Shortcut: Shortcut.F5),
				MenuItem(Text: "Expand",
						Click: _menuItemExpand_Click,
						Shortcut: Shortcut.CtrlE),
				MenuItem(Text: "-")
			))

		menu.MenuItems.AddRange((edit, tools))
		return menu

	def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("Boo")
		
	override protected def GetPersistString():
		return "BooEditor|${GetSafeFileName()}"


