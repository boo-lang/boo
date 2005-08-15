#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

// The boo forms designer is written by Doug Holton and Daniel Grunwald.

namespace BooBinding

/*
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps
*/

import System
import System.Text
import System.Text.RegularExpressions
import System.IO
import System.Collections
import System.Diagnostics
import System.Drawing
import System.Drawing.Design
import System.Reflection
import System.Windows.Forms
import System.Drawing.Printing
import System.ComponentModel
import System.ComponentModel.Design
import System.ComponentModel.Design.Serialization
import System.Xml
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Internal.Undo
import ICSharpCode.SharpDevelop.Gui.Components
import ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
import ICSharpCode.Core.Properties
import ICSharpCode.Core.AddIns
import ICSharpCode.Core.Services
import ICSharpCode.SharpDevelop.Services
import SharpDevelop.Internal.Parser

import ICSharpCode.SharpDevelop.FormDesigner.Services
import ICSharpCode.SharpDevelop.FormDesigner.Hosts
import ICSharpCode.SharpDevelop.FormDesigner.Util
import ICSharpCode.Core.AddIns.Codons
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import System.CodeDom
import System.CodeDom.Compiler

import Boo.Lang.CodeDom
import ICSharpCode.SharpDevelop.FormDesigner

class BooDesignerDisplayBinding(AbstractFormDesignerSecondaryDisplayBinding):
	protected override Extension as string:
		get:
			return ".boo"
			
	public override def CreateSecondaryViewContent(viewContent as IViewContent) as (ISecondaryViewContent):
		return (BooDesignerDisplayBindingWrapper(viewContent),)


class BooDesignerDisplayBindingWrapper(FormDesignerDisplayBindingBase, ISecondaryViewContent):
	failedDesignerInitialize as bool
	c as IClass
	initializeComponents as IMethod
	viewContent as IViewContent
	textAreaControlProvider as ITextEditorControlProvider
	compilationErrors as string
	
	override FileName as string:
		get:
			fname = textAreaControlProvider.TextEditorControl.FileName
			if fname is null:
				return viewContent.UntitledName
			return fname

	override ClipboardHandler as IClipboardHandler:
		get:
			return self
	
	override Control as Control:
		get:
			return super.designPanel
			
	override IsDirty as bool:
		get:
			if viewContent is null:
				return false
			return viewContent.IsDirty
		set:
			if not viewContent is null:
				viewContent.IsDirty = value
				
	Document as IDocument:
		get:
			return textAreaControlProvider.TextEditorControl.Document
			
	def constructor(view as IViewContent):
		self(view, true)
		
	def constructor(view as IViewContent, secondary as bool):
		self.viewContent = view
		self.textAreaControlProvider = view as ITextEditorControlProvider
		InitializeComponents(secondary)

	private def InitializeComponents(secondary as bool):
		failedDesignerInitialize = false
		undoHandler.Reset()
		Reload()
		UpdateSelectableObjects()
		if designPanel != null and secondary == true:
			designPanel.Disable()
			
	protected override def CreateDesignerHost():
		super.CreateDesignerHost()
		host.AddService(typeof(CodeDomProvider), Boo.Lang.CodeDom.BooCodeProvider()) //Boo.CodeDom
	
	_parseErrors as string = null
	
	private def OnParserError(e as antlr.RecognitionException):
		_parseErrors += "Line ${e.getLine()}: ${e.getErrorMessage()}\n"
	
	override def Reload():
		try:
			Initialize()
		except ex as Exception:
			Console.WriteLine('Initialization exception : ' + ex)
		
		dirty as bool = viewContent.IsDirty
		if host != null and c != null:
			super.host.SetRootFullName(c.FullyQualifiedName)
		
		try:
			compileUnit = Boo.Lang.Compiler.Ast.CompileUnit()
			_parseErrors = null
			Boo.Lang.Parser.BooParser.ParseModule(1, compileUnit, "designerIntegrityCheck", StringReader(Document.TextContent), OnParserError)
			
			failedDesignerInitialize = _parseErrors != null
			if failedDesignerInitialize:
				compilationErrors = _parseErrors
				return
			
			classString = GenerateClassString(Document)
			/* TODO: use this block of code when ICodeParser has been implemented.
			parser = BooCodeProvider().CreateParser()
			if parser == null:
				failedDesignerInitialize = true
				compilationErrors = 'Boo.CodeDom.BooCodeProvider.CreateParser() returned null!!!\nBoo.CodeDom.dll needs to implement ICodeParser!'
				return
			
			codeCompileUnit = parser.Parse(StringReader(classString))
			*/
			
			
			compileUnit = Boo.Lang.Compiler.Ast.CompileUnit()
			Boo.Lang.Parser.BooParser.ParseModule(1, compileUnit, "designerLoadClass", StringReader(classString), null)
			visitor = CodeDomVisitor()
			compileUnit.Accept(visitor)
			codeCompileUnit = visitor.OutputCompileUnit
			
			Microsoft.CSharp.CSharpCodeProvider().CreateGenerator().GenerateCodeFromCompileUnit(codeCompileUnit, Console.Out, null);
			
			if host != null and c != null:
				super.host.SetRootFullName(c.FullyQualifiedName)
			
			serializationManager as CodeDomDesignerSerializetionManager = host.GetService(typeof(IDesignerSerializationManager))
			serializationManager.Initialize()
			
			baseType as Type = typeof(System.Windows.Forms.Form)
			for codeNamespace as CodeNamespace in codeCompileUnit.Namespaces:
				if codeNamespace.Types.Count > 0:
					baseType = host.GetType(codeNamespace.Types[0].BaseTypes[0].BaseType)
					break
				
			rootSerializer as CodeDomSerializer = serializationManager.GetRootSerializer(baseType)
			if rootSerializer == null:
				raise Exception('No root serializer found')
			
			for codeNamespace as CodeNamespace in codeCompileUnit.Namespaces:
				if codeNamespace.Types.Count > 0:
					designerResourceService as DesignerResourceService = host.GetService(typeof(System.ComponentModel.Design.IResourceService))
					if designerResourceService != null:
						designerResourceService.SerializationStarted(false)
					
					try:
						rootSerializer.Deserialize(serializationManager, codeNamespace.Types[0])
					except e as Exception:
						Console.WriteLine(e)
						compilationErrors = "Can't deserialize form. Possible reason: Initialize component method was changed manually.\n${e}"
						failedDesignerInitialize = true
						return
					
					serializationManager.OnSerializationComplete()
					if designerResourceService != null:
						designerResourceService.SerializationEnded(false)
					
					designPanel.SetRootDesigner()
					designPanel.Enable()
					break
				
			
			failedDesignerInitialize = false
			undoHandler.Reset()
		except ex as Exception:
			Console.WriteLine("Got exception : ${ex.Message}\n${ex.StackTrace}")
			compilationErrors = ex.ToString()
			failedDesignerInitialize = true
		
		viewContent.IsDirty = dirty
	
	protected virtual def AppendUsings(builder as StringBuilder, usings as IUsingCollection):
		for u as IUsing in usings:
			for usingString as string in u.Usings:
				if usingString.StartsWith('System'):
					builder.Append('import ' + usingString + '\n')
	
	private def GenerateClassString(document as IDocument) as string:
		Reparse(document.TextContent)
		builder as StringBuilder = StringBuilder()
		//if c.Namespace != null and c.Namespace.Length > 0:
		//	builder.Append('namespace ')
		//	builder.Append(c.Namespace)
		//	builder.Append('\n')
		AppendUsings(builder, c.CompilationUnit.Usings)
		className as string = c.Name
		builder.Append('class ')
		builder.Append(className)
		builder.Append('(')
		builder.Append(ExtractBaseClass(c))
		builder.Append('):\n')
		fields as ArrayList = GetUsedFields(document, c, initializeComponents)
		for field as IField in fields:
			fieldLine as LineSegment = document.GetLineSegment(field.Region.BeginLine - 1)
			builder.Append(document.GetText(fieldLine.Offset, fieldLine.Length))
			builder.Append('\n')
		
		builder.Append('\tdef constructor():\n')
		builder.Append('\t\tpass\n')
		//builder.Append('\t\tself.')
		//builder.Append(initializeComponents.Name)
		//builder.Append('()\n')
		builder.Append('\t\n')
		initializeComponentsString as string = GetInitializeComponentsString(document, initializeComponents)
		builder.Append(initializeComponentsString)
		return builder.ToString()
	
	private def GetInitializeComponentsString(doc as IDocument, initializeComponents as IMethod) as string:
		beginLine as LineSegment = doc.GetLineSegment(initializeComponents.Region.BeginLine - 1)
		endLine as LineSegment = doc.GetLineSegment(initializeComponents.BodyRegion.EndLine - 1)
		startOffset as int = beginLine.Offset + initializeComponents.Region.BeginColumn - 1
		endOffset as int = endLine.Offset + initializeComponents.BodyRegion.EndColumn - 1
		return doc.GetText(startOffset, endOffset - startOffset)
	
	private def GetUsedFields(doc as IDocument, c as IClass, initializeComponents as IMethod) as ArrayList:
		InitializeComponentsString as string = GetInitializeComponentsString(doc, initializeComponents)
		fields as ArrayList = ArrayList()
		for field as IField in c.Fields:
			if InitializeComponentsString.IndexOf('self.' + field.Name + ' ') >= 0:
				fields.Add(field)
			
		
		return fields
	
	private def DeleteFormFields(doc as IDocument):
		fields as ArrayList = GetUsedFields(doc, c, initializeComponents)
		i as int = fields.Count - 1
		while i >= 0:
			field as IField = fields[i]
			fieldLine as LineSegment = doc.GetLineSegment(field.Region.BeginLine - 1)
			doc.Remove(fieldLine.Offset, fieldLine.TotalLength)
			--i
		
	
	protected virtual def MergeFormChanges():
		if self.failedDesignerInitialize:
			return
		
		writer = StringWriter()
		CodeDOMGenerator(self.host, BooCodeProvider()).ConvertContentDefinition(writer);
		currentForm as string = writer.ToString()
		designerResourceService as DesignerResourceService = host.GetService(typeof(System.ComponentModel.Design.IResourceService))
		if designerResourceService != null:
			self.resources = Hashtable()
			if designerResourceService.Resources != null and designerResourceService.Resources.Count != 0:
				for entry as DictionaryEntry in designerResourceService.Resources:
					self.resources[entry.Key] = DesignerResourceService.ResourceStorage(cast(DesignerResourceService.ResourceStorage, entry.Value))
				
			
		MessageBox.Show(currentForm)
		/*
		generatedInfo as IParseInformation = parserService.ParseFile(FileName, currentForm, false)
		cu as ICompilationUnit = generatedInfo.BestCompilationUnit
		if cu.Classes == null or cu.Classes.Count == 0:
			return
		
		generatedClass as IClass = cu.Classes[0]
		generatedInitializeComponents as IMethod = GetInitializeComponents(cu.Classes[0])
		newDoc as IDocument = DocumentFactory().CreateDocument()
		newDoc.TextContent = currentForm
		newInitializeComponents as string = GetInitializeComponentsString(newDoc, generatedInitializeComponents)
		textArea as TextEditorControl = textAreaControlProvider.TextEditorControl
		textArea.BeginUpdate()
		marker as (FoldMarker) = textArea.Document.FoldingManager.FoldMarker.ToArray(typeof(FoldMarker))
		textArea.Document.FoldingManager.FoldMarker.Clear()
		oldDoc as IDocument = DocumentFactory().CreateDocument()
		oldDoc.TextContent = textArea.Document.TextContent
		Reparse(oldDoc.TextContent)
		DeleteFormFields(oldDoc)
		Reparse(oldDoc.TextContent)
		beginLine as LineSegment = oldDoc.GetLineSegment(initializeComponents.Region.BeginLine - 1)
		startOffset as int = beginLine.Offset + initializeComponents.Region.BeginColumn - 1
		oldDoc.Replace(startOffset, GetInitializeComponentsString(oldDoc, initializeComponents).Length, newInitializeComponents)
		Reparse(oldDoc.TextContent)
		lineNr as int = c.Region.BeginLine - 1
		while true:
			if lineNr >= textArea.Document.TotalNumberOfLines - 2:
				break
			
			curLine as LineSegment = oldDoc.GetLineSegment(lineNr)
			if oldDoc.GetText(curLine.Offset, curLine.Length).Trim().EndsWith('{'):
				break
			
			++lineNr
		
		beginLine = oldDoc.GetLineSegment(lineNr + 1)
		insertOffset as int = beginLine.Offset
		for field as IField in generatedClass.Fields:
			fieldLine as LineSegment = newDoc.GetLineSegment(field.Region.BeginLine - 1)
			oldDoc.Insert(insertOffset, newDoc.GetText(fieldLine.Offset, fieldLine.TotalLength))
		
		oldCaretPos as Point = textArea.ActiveTextAreaControl.Caret.Position
		textArea.Document.TextContent = oldDoc.TextContent
		textArea.ActiveTextAreaControl.Caret.Position = oldCaretPos
		parseInfo as IParseInformation = parserService.ParseFile(FileName, textArea.Document.TextContent, false)
		textArea.Document.FoldingManager.UpdateFoldings(FileName, parseInfo)
		parseInfo = null
		i as int = 0
		while i < marker.Length and i < textArea.Document.FoldingManager.FoldMarker.Count:
			cast(FoldMarker, textArea.Document.FoldingManager.FoldMarker[i]).IsFolded = marker[i].IsFolded
			++i
		
		viewContent.IsDirty = dirty
		textArea.Document.UndoStack.ClearAll()
		textArea.EndUpdate()
		textArea.OptionsChanged()
		*/
	
	protected def Reparse(content as string):
		parserService as IParserService = ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService))
		info as IParseInformation = parserService.ParseFile(self.FileName, content, false)
		cu as ICompilationUnit = info.BestCompilationUnit
		for c as IClass in cu.Classes:
			if IsBaseClassDesignable(c):
				initializeComponents = GetInitializeComponents(c)
				if initializeComponents != null:
					self.c = c
					break
	
	
	private def GetInitializeComponents(c as IClass) as IMethod:
		for method as IMethod in c.Methods:
			if (method.Name == 'InitializeComponents' or method.Name == 'InitializeComponent') and method.Parameters.Count == 0:
				return method
			
		
		return null
	
	override def ShowSourceCode():
		self.WorkbenchWindow.SwitchView(0)
	
	override def ShowSourceCode(lineNumber as int):
		ShowSourceCode()
		textAreaControlProvider.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber, 255)
	
	protected static def GenerateParams(edesc as EventDescriptor, paramNames as bool) as string:
		t as System.Type = edesc.EventType
		mInfo as MethodInfo = t.GetMethod('Invoke')
		param as string = ''
		csa as IAmbience = null
		try:
			csa = cast(IAmbience, AddInTreeSingleton.AddInTree.GetTreeNode('/SharpDevelop/Workbench/Ambiences').BuildChildItem('Boo', typeof(BooDesignerDisplayBindingWrapper)))
		except:
			pass
		
		i as int = 0
		while i < mInfo.GetParameters().Length:
			pInfo as ParameterInfo = mInfo.GetParameters()[i]
			typeStr as string = pInfo.ParameterType.ToString()
			if csa != null:
				typeStr = csa.GetIntrinsicTypeName(typeStr)
			
			param += typeStr
			if paramNames == true:
				param += ' '
				param += pInfo.Name
			
			if i + 1 < mInfo.GetParameters().Length:
				param += ', '
			
			++i
		
		return param
	
	protected def InsertComponentEvent(component as IComponent, edesc as EventDescriptor, eventMethodName as string, body as string, position as int) as bool:
		if self.failedDesignerInitialize:
			position = 0
			return false
		
		Reparse(Document.TextContent)
		for method as IMethod in c.Methods:
			if method.Name == eventMethodName:
				position = method.Region.BeginLine + 1
				return true
			
		
		Deselected()
		MergeFormChanges()
		Reparse(Document.TextContent)
		position = c.Region.EndLine + 1
		offset as int = Document.GetLineSegment(c.Region.EndLine - 1).Offset
		text as string = "\tprivate def ${eventMethodName}(${GenerateParams(edesc, true)}):\n\t\t${body}\n\t\n"
		Document.Insert(offset, text)
		//Document.FormattingStrategy.IndentLines(self.textAreaControlProvider.TextEditorControl.ActiveTextAreaControl.TextArea, c.Region.EndLine - 1, c.Region.EndLine + 3)
		return false
	
	override def ShowSourceCode(component as IComponent, edesc as EventDescriptor, eventMethodName as string):
		position as int
		InsertComponentEvent(component, edesc, eventMethodName, 'pass', position)
		ShowSourceCode(position)
	
	override def GetCompatibleMethods(edesc as EventDescriptor) as ICollection:
		Reparse(Document.TextContent)
		compatibleMethods as ArrayList = ArrayList()
		methodInfo as MethodInfo = edesc.EventType.GetMethod('Invoke')
		for method as IMethod in c.Methods:
			if method.Parameters.Count == methodInfo.GetParameters().Length:
				found as bool = true
				i as int = 0
				while i < methodInfo.GetParameters().Length:
					pInfo as ParameterInfo = methodInfo.GetParameters()[i]
					p as IParameter = method.Parameters[i]
					if p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString():
						found = false
						break
					
					++i
				
				if found:
					compatibleMethods.Add(method.Name)
				
			
		
		return compatibleMethods
	
	override def GetCompatibleMethods(edesc as EventInfo) as ICollection:
		//Reparse(Document.TextContent)
		compatibleMethods as ArrayList = ArrayList()
		methodInfo as MethodInfo = edesc.GetAddMethod()
		pInfo as ParameterInfo = methodInfo.GetParameters()[0]
		eventName as string = pInfo.ParameterType.ToString().Replace('EventHandler', 'EventArgs')
		for method as IMethod in c.Methods:
			if method.Parameters.Count == 2:
				found as bool = true
				p as IParameter = method.Parameters[1]
				if p.ReturnType.FullyQualifiedName != eventName:
					found = false
				
				if found:
					compatibleMethods.Add(method.Name)
				
			
		return compatibleMethods
	
	override def Selected():
		isFormDesignerVisible = true
		Reload()
		if not failedDesignerInitialize:
			pass
		else:
			if super.designPanel != null:
				super.designPanel.SetErrorState(compilationErrors)
			
		
	
	override def Deselected():
		isFormDesignerVisible = false
		super.designPanel.Disable()
		if not failedDesignerInitialize:
			MergeFormChanges()
			textAreaControlProvider.TextEditorControl.Refresh()
		
		DeselectAllComponents()
	
	def NotifyAfterSave(successful as bool):
		if successful:
			designerResourceService as DesignerResourceService = host.GetService(typeof(System.ComponentModel.Design.IResourceService))
			if designerResourceService != null:
				designerResourceService.Save()
			
	
	def NotifyBeforeSave():
		MergeFormChanges()
		
	
	//boo bug?  compiler thinks these methods are not implemented.
	override def Dispose():
		super()
	override def RedrawContent():
		super()
	override def SwitchedTo():
		super()
	override WorkbenchWindow as IWorkbenchWindow:
		set:
			pass
	override TabPageText as string:
		get:
			return super.TabPageText
