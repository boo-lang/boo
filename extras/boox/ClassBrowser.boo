namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing
import Boo.AntlrParser
import Boo.Lang.Compiler.Ast

class ClassBrowser(Content):
	
	_tree = TreeView(Dock: DockStyle.Fill)
	_treeViewVisitor = TreeViewVisitor(_tree)
	
	def constructor():
		
		SuspendLayout()
		
		Controls.Add(_tree)
		self.AllowedStates = (
					ContentStates.Float |
					ContentStates.DockLeft |
					ContentStates.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)
		
		self.DockPadding.Bottom = 2
		self.DockPadding.Top = 26
		self.HideOnClose = true
		self.ShowHint = WeifenLuo.WinFormsUI.DockState.DockRight
		self.Text = /*self.TabText =*/ "Class Browser"
		ResumeLayout(false)
		
	ActiveDocument as BooEditor:
		set:
			Update(value.Text, value.TextContent)
			
	def Update(fname as string, text as string):
		try:
			UpdateTree(BooParser.ParseString(fname, text))
		except x:
			print(x)
	
	def UpdateTree(cu as CompileUnit):
		_treeViewVisitor.Switch(cu)

class TreeViewVisitor(DepthFirstSwitcher):
	
	_tree as TreeView
	_current as TreeNode
	
	def constructor(tree):
		_tree = tree
		
	override def OnCompileUnit(node as CompileUnit):
		_tree.SuspendLayout()
		try:
			_tree.Nodes.Clear()
			Switch(node.Modules)
			_tree.ExpandAll()
		ensure:
			_tree.ResumeLayout(false)
		
	override def OnModule(node as Module):
		if node.Namespace:
			name = node.Namespace.Name
		else:
			name = node.Name
		_current = _tree.Nodes.Add(name)
		Switch(node.Members)
		
	override def OnProperty(node as Property):
		Add(node.Name, node)		
		
	override def OnField(node as Field):
		Add(node.Name, node)
		
	override def OnClassDefinition(node as ClassDefinition):
		saved = _current
		
		_current = Add(node.Name, node)
		Switch(node.Members)
		
		_current = saved
		
	override def OnMethod(node as Method):		
		Add(node.Name, node)
		
	def Add(text as string, data):
		node = _current.Nodes.Add(node.Name)
		node.Tag = data
		return node
