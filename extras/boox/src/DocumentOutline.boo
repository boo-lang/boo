namespace BooExplorer

import WeifenLuo.WinFormsUI
import System
import System.IO
import System.Windows.Forms
import System.Drawing
import Boo.Lang.Compiler.Ast

enum TypeIcon:
	Namespace
	PublicClass
	PublicInterface
	PublicEnum
	PublicField
	PublicProperty
	PublicMethod
	InternalClass
	InternalInterface
	InternalEnum
	InternalField
	InternalProperty
	InternalMethod
	ProtectedClass
	ProtectedInterface
	ProtectedEnum
	ProtectedField
	ProtectedProperty
	ProtectedMethod
	PrivateClass
	PrivateInterface
	PrivateEnum
	PrivateField
	PrivateProperty
	PrivateMethod

class TypeIconChooser:
	static def GetPropertyIcon(node as Property, inInterface as bool) as int:
		return cast(int, TypeIcon.PublicProperty) if inInterface
		if node.IsVisibilitySet:
			if node.IsInternal:
				return cast(int, TypeIcon.InternalProperty)
			if node.IsProtected:
				return cast(int, TypeIcon.ProtectedProperty)
			if node.IsPrivate:
				return cast(int, TypeIcon.PrivateProperty)
				
		return cast(int, TypeIcon.PublicProperty)

	static def GetFieldIcon(node as Field) as int:
		if node.IsVisibilitySet:
			if node.IsInternal:
				return cast(int, TypeIcon.InternalField)
			if node.IsPublic:
				return cast(int, TypeIcon.PublicField)
			if node.IsPrivate:
				return cast(int, TypeIcon.PrivateField)
				
		return cast(int, TypeIcon.ProtectedField)

class DocumentOutline(DockContent):

	_activeDocument as BooEditor
	_tree as TreeView
	_imageList as ImageList
	_treeViewVisitor as TreeViewVisitor
	_timer = Timer(Tick: _timer_Tick, Interval: 3s.TotalMilliseconds)
	_module as Module
	_resourceManager = System.Resources.ResourceManager(DocumentOutline)

	def constructor():
		InitImageList()
		InitTreeView()

		_treeViewVisitor = TreeViewVisitor(_tree)

		SuspendLayout()

		Controls.Add(_tree)
		self.DockableAreas = (
					DockAreas.Float |
					DockAreas.DockLeft |
					DockAreas.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)

		self.DockPadding.Bottom = 2
		self.DockPadding.Top = 26
		self.ShowHint = DockState.DockRight;
		self.Text = "Document Outline"
		self.HideOnClose = true
		ResumeLayout(false)

	def InitImageList():		
		_imageList = ImageList()
		_imageList.ImageStream = _resourceManager.GetObject("_imageList")

	def InitTreeView():
		_tree = TreeView(Dock: DockStyle.Fill,
						DoubleClick: _tree_DoubleClick,
						ImageIndex: cast(int, TypeIcon.Namespace),
						SelectedImageIndex: cast(int, TypeIcon.Namespace),
						ImageList: _imageList,
						Sorted: true)

	ActiveDocument as BooEditor:
		set:
			_activeDocument = value
			_timer.Enabled = value is not null
			Update() if self.Visible

	def Update():
		if _activeDocument is null:
			_tree.Nodes.Clear()
		else:
			_activeDocument.UpdateModule()
			UpdateTree(_activeDocument.Module)

	def UpdateTree(module as Module):
		if module is not _module:
			_module = module
			_treeViewVisitor.Switch(_module)
			
	def GoToNode([required] treeNode as TreeNode):
		return unless _activeDocument
		node as Node = treeNode.Tag
		if node is not null:
			info = node.LexicalInfo
			_activeDocument.GoTo(info.Line-1)

	def _timer_Tick(sender, args as EventArgs):
		Update()
		
	def _tree_DoubleClick(sender, args as EventArgs):

		if (treeNode = _tree.SelectedNode):
			GoToNode(treeNode)
			
	override protected def GetPersistString():
		return "DocumentOutline|"

class TreeViewVisitor(DepthFirstSwitcher):

	_tree as TreeView
	_current as TreeNode
	_inInterface as bool

	def constructor(tree):
		_tree = tree

	override def OnModule(node as Module):

		_current = TreeNode("root")
		Switch(node.Members)

		_tree.BeginUpdate()
		
		//state = SaveTreeViewState() if len(_current.Nodes)		
		_tree.Nodes.Clear()
		if len(_current.Nodes):
			_tree.Nodes.AddRange(array(TreeNode, _current.Nodes))
			_tree.ExpandAll()
			//RestoreTreeViewState(state) if len(state)
		_tree.EndUpdate()

	override def OnProperty(node as Property):
		Add(node.Name, TypeIconChooser.GetPropertyIcon(node, _inInterface), node)
				
	override def OnField(node as Field):
		Add(node.Name, TypeIconChooser.GetFieldIcon(node), node)

	override def OnInterfaceDefinition(node as InterfaceDefinition):
		OnTypeDefinition(node, cast(int, TypeIcon.PublicInterface))

	override def OnClassDefinition(node as ClassDefinition):
		OnTypeDefinition(node, cast(int, TypeIcon.PublicClass))

	override def OnEnumDefinition(node as EnumDefinition):
		OnTypeDefinition(node, cast(int, TypeIcon.PublicEnum))
		
	override def OnEnumMember(node as EnumMember):
		Add(node.Name, cast(int, TypeIcon.PublicField), node)

	def OnTypeDefinition(node as TypeDefinition, imageIndex as int):
		_inInterface = node isa InterfaceDefinition
		saved = _current

		_current = Add(node.Name, imageIndex, imageIndex, node)
		Switch(node.Members)

		_current = saved

	override def OnConstructor(node as Constructor):
		OnMethod(node)

	override def OnMethod(node as Method):
		name = "${node.Name}(${join([p.Name for p as ParameterDeclaration in node.Parameters], ', ')})"
		Add(name, cast(int, TypeIcon.PublicMethod), node)

	def Add(text as string, data):
		node = _current.Nodes.Add(text)
		node.Tag = data
		return node

	def Add(text as string, imageIndex as int, data):
		Add(text, imageIndex, imageIndex, data)

	def Add(text as string, imageIndex as int, selectedImageIndex as int, data):
		node = _current.Nodes.Add(text)
		node.Tag = data
		node.ImageIndex = imageIndex
		node.SelectedImageIndex = selectedImageIndex
		return node
		
	def SaveTreeViewState():
		return SaveTreeViewState([], _tree.Nodes)
		
	def SaveTreeViewState(state as List, nodes as TreeNodeCollection):
		for node in nodes:
			SaveTreeViewState(state, node)
		return state
		
	def SaveTreeViewState(state as List, node as TreeNode):
		if len(node.Nodes):
			state.Add((node.FullPath, node.IsExpanded))
			SaveTreeViewState(state, node.Nodes)
		
	def RestoreTreeViewState(state):		
		for fullpath as string, expanded as bool in state:
			if not expanded:
				node = SelectNode(fullpath)
				node.Collapse() if node
				
	def SelectNode(fullpath as string):
		parts = /\//.Split(fullpath)
		
		nodes = _tree.Nodes
		for part in parts:
			node = SelectNode(nodes, part)
			break if node is null
			nodes = node.Nodes
			
		return node
		
	def SelectNode(nodes as TreeNodeCollection, text as string):
		for node as TreeNode in nodes:
			if node.Text == text:
				return node
		return null

