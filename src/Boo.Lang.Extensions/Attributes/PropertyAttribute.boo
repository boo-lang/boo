#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


namespace Boo.Lang.Extensions

import System
import Boo.Lang.Compiler.Ast

public class PropertyAttribute(Boo.Lang.Compiler.AbstractAstAttribute):

	protected _propertyName as ReferenceExpression

	protected _propertyType as TypeReference

	protected _setPreCondition as Expression
	
	protected _protected as BoolLiteralExpression

	protected _protectedSetter as BoolLiteralExpression

	protected _observable as BoolLiteralExpression
	
	protected _attributes as ListLiteralExpression
	
	public def constructor(propertyName as ReferenceExpression):
		self(propertyName, null)

	public def constructor(propertyNameAndType as TryCastExpression):
		re = propertyNameAndType.Target as ReferenceExpression
		if not re:
			raise ArgumentException('Left-side must be a ReferenceExpression (ie. name of the property)', 'propertyNameAndType')
		_propertyType = propertyNameAndType.Type
		self(re, null)

	public def constructor(propertyName as ReferenceExpression, setPreCondition as Expression):
		if propertyName is null:
			raise ArgumentNullException('propertyName')
		_propertyName = propertyName
		_setPreCondition = setPreCondition

	virtual HasSetter as bool:
		get:
			return true

	Protected:
		get:
			return _protected
		set:
			_protected = value

	ProtectedSetter:
		get:
			return _protectedSetter
		set:
			if not HasSetter:
				raise ArgumentException("ProtectedSetter", "ProtectedSetter can not be set on a getter.")
			_protectedSetter = value

	protected IsProtected:
		get:
			if _protected is null:
				return false
			return _protected.Value

	protected IsProtectedSetter:
		get:
			if _protectedSetter is null:
				return false
			return _protectedSetter.Value

	Observable:
		get:
			return _observable
		set:
			_observable = value
	
	protected IsObservable:
		get:
			if _observable is null:
				return false
			return _observable.Value
	
	protected ChangedEventName:
		get:
			return (_propertyName.Name + 'Changed')
	
	Attributes:
		get:
			return _attributes
		set:
			_attributes = value
	
	public override def Apply(node as Node):
		f = (node as Field)
		if f is null:
			InvalidNodeForAttribute('Field')
			return 
		
		p = Property()
		if f.IsStatic:
			p.Modifiers |= TypeMemberModifiers.Static
		if IsProtected:
			p.Modifiers |= TypeMemberModifiers.Protected
		else:
			p.Modifiers |= TypeMemberModifiers.Public
		p.Name = _propertyName.Name
		if not _propertyType:
			p.Type = f.Type
		else:
			p.Type = _propertyType
		p.Getter = CreateGetter(f)
		p.Setter = CreateSetter(f)
		p.LexicalInfo = LexicalInfo
		
		if (Attributes is not null) and (Attributes.Items.Count > 0):
			for item as Expression in Attributes.Items:
				p.Attributes.Add(ConvertExpressionToAttribute(item))
		
		f.DeclaringType.Members.Add(p)
		
		if IsObservable:
			f.DeclaringType.Members.Add(CreateChangedEvent(f))
	
	public static def ConvertExpressionToAttribute(item as Expression) as Boo.Lang.Compiler.Ast.Attribute:
		att as Boo.Lang.Compiler.Ast.Attribute = Boo.Lang.Compiler.Ast.Attribute(item.LexicalInfo)
		if item isa MethodInvocationExpression:
			m = cast(MethodInvocationExpression, item)
			att.Name = m.Target.ToString()
			att.Arguments = m.Arguments
			att.NamedArguments = m.NamedArguments
		else:
			att.Name = item.ToString()
		return att

	
	protected virtual def CreateGetter(f as Field) as Method:
		// get:
		//		return <f.Name>
		getter = Method()
		getter.Name = 'get'
		getter.Body.Statements.Add(ReturnStatement(super.LexicalInfo, ReferenceExpression(f.Name), null))
		return getter

	
	protected virtual def CreateSetter(f as Field) as Method:
		setter = Method()
		setter.Name = 'set'

		if IsProtectedSetter:
			setter.Visibility = TypeMemberModifiers.Protected

		if _setPreCondition is not null:
			setter.Body.Add(RaiseStatement(_setPreCondition.LexicalInfo, AstUtil.CreateMethodInvocationExpression(AstUtil.CreateReferenceExpression('System.ArgumentException'), StringLiteralExpression((('precondition \'' + _setPreCondition.ToString()) + '\' failed:'))), StatementModifier(StatementModifierType.Unless, _setPreCondition)))
		setter.Body.Add(BinaryExpression(super.LexicalInfo, BinaryOperatorType.Assign, MemberReferenceExpression(CreateRefTarget(f), f.Name), ReferenceExpression('value')))
		
		if IsObservable:
			mie = MethodInvocationExpression(ReferenceExpression(ChangedEventName))
			mie.Arguments.Add(SelfLiteralExpression())
			mie.Arguments.Add(MemberReferenceExpression(MemberReferenceExpression(ReferenceExpression('System'), 'EventArgs'), 'Empty'))
			setter.Body.Add(mie)
		return setter

	
	private def CreateRefTarget(f as Field) as Expression:
		if f.IsStatic:
			return ReferenceExpression(LexicalInfo, f.DeclaringType.Name)
		return SelfLiteralExpression(LexicalInfo)

	protected def CreateChangedEvent(f as Field) as Event:
		e = Event(_observable.LexicalInfo)
		e.Name = ChangedEventName
		e.Type = CodeBuilder.CreateTypeReference(f.LexicalInfo, typeof(System.EventHandler))
		if IsProtected or IsProtectedSetter:
			e.Visibility = TypeMemberModifiers.Protected
		else:
			e.Visibility = TypeMemberModifiers.Public
		return e

