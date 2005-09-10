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

namespace BooBinding.CodeCompletion

import System
import System.Collections
import ICSharpCode.Core.Services
import SharpDevelop.Internal.Parser
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast as AST
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps

class Using(AbstractUsing):
	pass

class Visitor(AbstractVisitorCompilerStep):
	[Getter(Cu)]
	_cu as CompilationUnit = CompilationUnit()
	
	_currentClass as Stack = Stack()
	_firstModule = true
	
	override def Run():
		print "RUN"
		try:
			Visit(CompileUnit)
		except e:
			print e.ToString()
			//msg as IMessageService = ServiceManager.Services.GetService(typeof(IMessageService))
			//msg.ShowError(e)
	
	private def GetModifier(m as AST.TypeMember) as ModifierEnum:
		r = ModifierEnum.None
		r = r | ModifierEnum.Public    if m.IsPublic
		r = r | ModifierEnum.Protected if m.IsProtected
		r = r | ModifierEnum.Private   if m.IsPrivate
		r = r | ModifierEnum.Internal  if m.IsInternal
		
		r = r | ModifierEnum.Static   if m.IsStatic
		r = r | ModifierEnum.Virtual  if m.IsModifierSet(AST.TypeMemberModifiers.Virtual)
		r = r | ModifierEnum.Abstract if m.IsModifierSet(AST.TypeMemberModifiers.Abstract)
		r = r | ModifierEnum.Override if m.IsModifierSet(AST.TypeMemberModifiers.Override)
		
		r = r | ModifierEnum.Final if m.IsFinal
		return r
	
	[Property(LineLength)]
	_lineLength as (int)
	
	private def GetLineEnd(line as int) as int:
		return 0 if _lineLength == null or line < 1 or line > _lineLength.Length
		return _lineLength[line - 1] + 1
	
	private def GetRegion(m as AST.Node):
		l = m.LexicalInfo
		return null if (l.Line < 0)
		return DefaultRegion(l.Line, 0 /*l.Column*/, l.Line, GetLineEnd(l.Line))
	
	private def GetClientRegion(m as AST.Node) as DefaultRegion:
		l = m.LexicalInfo
		return null if l.Line < 0
		l2 as AST.SourceLocation = null
		if m isa AST.Method:
			l2 = cast(AST.Method, m).Body.EndSourceLocation
		elif m isa AST.Property:
			p as AST.Property = m
			if p.Getter != null and p.Getter.Body != null:
				l2 = cast(AST.Property, m).Getter.Body.EndSourceLocation
				if p.Setter != null and p.Setter.Body != null:
					l3 = cast(AST.Property, m).Setter.Body.EndSourceLocation
					l2 = l3 if l3.Line > l2.Line
			elif p.Setter != null and p.Setter.Body != null:
				l2 = cast(AST.Property, m).Setter.Body.EndSourceLocation
		else:
			l2 = m.EndSourceLocation
		return null if l2 == null or l2.Line < 0 or l.Line == l2.Line
		// TODO: use l.Column / l2.Column when the tab-bug has been fixed
		return DefaultRegion(l.Line, GetLineEnd(l.Line), l2.Line, GetLineEnd(l2.Line))
	
	override def OnImport(p as AST.Import):
		u = Using()
		if p.Alias == null:
			u.Usings.Add(p.Namespace)
		else:
			u.Aliases[p.Alias.Name] = p.Namespace
		_cu.Usings.Add(u)
	
	override def OnCallableDefinition(node as AST.CallableDefinition):
		print "OnCallableDefinition: ${node.FullName}"
		region = GetRegion(node)
		modifier = GetModifier(node)
		c = Class(_cu, ClassType.Delegate, modifier, region)
		c.BaseTypes.Add('System.Delegate')
		c.FullyQualifiedName = node.FullName
		if _currentClass.Count > 0:
			cast(Class, _currentClass.Peek()).InnerClasses.Add(c)
		else:
			_cu.Classes.Add(c)
		invokeMethod = Method('Invoke', ReturnType(node.ReturnType), modifier, region, region)
		invokeMethod.Parameters = GetParameters(node.Parameters)
		c.Methods.Add(invokeMethod)
	
	override def EnterClassDefinition(node as AST.ClassDefinition):
		EnterTypeDefinition(node, ClassType.Class)
		return super(node)
	
	override def EnterInterfaceDefinition(node as AST.InterfaceDefinition):
		EnterTypeDefinition(node, ClassType.Interface)
		return super(node)
	
	override def EnterEnumDefinition(node as AST.EnumDefinition):
		EnterTypeDefinition(node, ClassType.Enum)
		return super(node)
	
	override def EnterModule(node as AST.Module):
		EnterTypeDefinition(node, ClassType.Class) unless _firstModule
		_firstModule = false
		return super(node)
	
	private def EnterTypeDefinition(node as AST.TypeDefinition, classType as ClassType):
		try:
			print "Enter ${node.GetType().Name} (${node.FullName})"
			region = GetClientRegion(node)
			modifier = GetModifier(node)
			c = Class(_cu, classType, modifier, region)
			c.FullyQualifiedName = node.FullName
			c.Documentation = node.Documentation
			if _currentClass.Count > 0:
				cast(Class, _currentClass.Peek()).InnerClasses.Add(c)
			else:
				_cu.Classes.Add(c)
			if node.BaseTypes != null:
				for r as AST.SimpleTypeReference in node.BaseTypes:
					c.BaseTypes.Add(r.Name)
			_currentClass.Push(c)
		except ex:
			print ex.ToString()
			raise
	
	override def LeaveClassDefinition(node as AST.ClassDefinition):
		LeaveTypeDefinition(node)
		super(node)
	
	override def LeaveInterfaceDefinition(node as AST.InterfaceDefinition):
		LeaveTypeDefinition(node)
		super(node)
	
	override def LeaveEnumDefinition(node as AST.EnumDefinition):
		LeaveTypeDefinition(node)
		super(node)
	
	override def LeaveModule(node as AST.Module):
		LeaveTypeDefinition(node) unless _currentClass.Count == 0
		super(node)
	
	private def LeaveTypeDefinition(node as AST.TypeDefinition):
		c as Class = _currentClass.Pop()
		print "Leave ${node.GetType().Name} ${node.FullName} (Class = ${c.FullyQualifiedName})"
		c.UpdateModifier()
	
	override def OnMethod(node as AST.Method):
		try:
			print "Method: ${node.FullName}"
			method = Method(node.Name, ReturnType.CreateReturnType(node), GetModifier(node), GetRegion(node), GetClientRegion(node))
			method.Parameters = GetParameters(node.Parameters)
			method.Node = node
			method.Documentation = node.Documentation
			cast(Class, _currentClass.Peek()).Methods.Add(method)
		except ex:
			print ex.ToString()
			raise
	
	private def GetParameters(params as AST.ParameterDeclarationCollection):
		parameters = ParameterCollection()
		return parameters if params == null
		for par as AST.ParameterDeclaration in params:
			parameters.Add(Parameter(par.Name, ReturnType(par.Type)))
		return parameters
	
	override def OnConstructor(node as AST.Constructor):
		return if node.Body.Statements.Count == 0
		ctor = Constructor(GetModifier(node), GetRegion(node), GetClientRegion(node))
		ctor.Parameters = GetParameters(node.Parameters)
		ctor.Node = node
		ctor.Documentation = node.Documentation
		cast(Class, _currentClass.Peek()).Methods.Add(ctor)
		
	override def OnEnumMember(node as AST.EnumMember):
		try:
			c as Class = _currentClass.Peek()
			field = Field(ReturnType(c), node.Name, GetModifier(node), GetRegion(node))
			field.Documentation = node.Documentation
			field.SetModifiers(ModifierEnum.Const | ModifierEnum.SpecialName)
			c.Fields.Add(field)
		except x:
			print x
			raise
	
	override def OnField(node as AST.Field):
		try:
			print "Field ${node.Name}"
			c as Class = _currentClass.Peek()
			field = Field(ReturnType.CreateReturnType(node), node.Name, GetModifier(node), GetRegion(node))
			field.Documentation = node.Documentation
			c.Fields.Add(field)
		except ex:
			print ex.ToString()
			raise
			
	override def OnEvent(node as AST.Event):
		try:
			print "event ${node.Name}"
			c as Class = _currentClass.Peek()
			region = GetRegion(node)
			e = Event(node.Name, ReturnType.CreateReturnType(node), GetModifier(node), region, region)
			e.Documentation = node.Documentation
			c.Events.Add(e)
		except ex:
			print ex.ToString()
			raise
	
	override def OnProperty(node as AST.Property):
		try:
			print "Property ${node.Name}"
			property = Property(node.Name, ReturnType.CreateReturnType(node), GetModifier(node), GetRegion(node), GetClientRegion(node))
			property.Documentation = node.Documentation
			property.Node = node
			cast(Class, _currentClass.Peek()).Properties.Add(property)
		except ex:
			print ex.ToString()
			raise
	
	/*
	// TODO: Detect indexer method and add it as Indexer
	override def Visit(indexerDeclaration as AST.IndexerDeclaration, data as object) as object:
		region as DefaultRegion = GetRegion(indexerDeclaration.StartLocation, indexerDeclaration.EndLocation)
		bodyRegion as DefaultRegion = GetRegion(indexerDeclaration.BodyStart, indexerDeclaration.BodyEnd)
		parameters as ParameterCollection = ParameterCollection()
		i as Indexer = Indexer(ReturnType(indexerDeclaration.TypeReference), parameters, indexerDeclaration.Modifier, region, bodyRegion)
		if indexerDeclaration.Parameters != null:
			for par as AST.ParameterDeclarationExpression in indexerDeclaration.Parameters:
				parType as ReturnType = ReturnType(par.TypeReference)
				p as Parameter = Parameter(par.ParameterName, parType)
				parameters.Add(p)
			
		
		c as Class = _currentClass.Peek()
		c.Indexer.Add(i)
		return null
	*/
	


