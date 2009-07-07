#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


using System;

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class BindTypeMembers : BindMethods
	{
		Boo.Lang.List _parameters = new Boo.Lang.List();
		Boo.Lang.List _events = new Boo.Lang.List();
		IMethod _delegate_Combine;
		IMethod _delegate_Remove;

		const string PrivateMemberNeverUsed = "PrivateMemberNeverUsed";

		override public void OnMethod(Method node)
		{
			_parameters.Add(node);
			base.OnMethod(node);
		}
		
		void BindAllParameters()
		{
			Method entryPoint = ContextAnnotations.GetEntryPoint(Context);

			foreach (INodeWithParameters node in _parameters)
			{
				TypeMember member = (TypeMember)node;

				if (member.ContainsAnnotation(PrivateMemberNeverUsed))
					continue;

				NameResolutionService.Restore((INamespace)TypeSystemServices.GetEntity(member.DeclaringType));
				CodeBuilder.BindParameterDeclarations(member.IsStatic, node);
				if (!member.IsVisible && !member.IsSynthetic)
				{
					IExplicitMember explicitMember = member as IExplicitMember;
					if (null != explicitMember && null != explicitMember.ExplicitInfo)
						continue;
					if (member == entryPoint) //private Main is fine
						continue;
					member.Annotate(PrivateMemberNeverUsed, null);
				}
			}
		}
		
		override public void OnConstructor(Constructor node)
		{
			_parameters.Add(node);
			base.OnConstructor(node);
		}
		
		override public void OnField(Field node)
		{
			if (null == node.Entity)
			{
				node.Entity = new InternalField(node);
				if (!node.IsVisible && !node.IsSynthetic)
				{
					node.Annotate(PrivateMemberNeverUsed, null);
				}
			}
		}
		
		override public void OnProperty(Property node)
		{
			EnsureEntityFor(node);
			_parameters.Add(node);
			
			Visit(node.Getter);
			Visit(node.Setter);
			Visit(node.ExplicitInfo);
		}

		override public void OnEvent(Event node)
		{
			_events.Add(node);
		}

		void BindAllEvents()
		{
			foreach (Event node in _events)
			{
				BindEvent(node);
			}
		}

		void BindEvent(Event node)
		{
			EnsureEntityFor(node);

			IType type = GetType(node.Type);
			IType declaringType = GetType(node.DeclaringType);
			bool typeIsCallable = type is ICallableType;
			if (!typeIsCallable)
			{
				Errors.Add(
					CompilerErrorFactory.EventTypeIsNotCallable(node.Type,
					type.ToString()));
			}
			
			if (declaringType.IsInterface)
			{
				BindInterfaceEvent(node);
			}
			else
			{
				BindClassEvent(node, type, typeIsCallable);
			}
		}

		private void BindInterfaceEvent(Event node)
		{
			if (null == node.Add)
				node.Add = CreateInterfaceEventAddMethod(node);
			if (null == node.Remove)
				node.Remove = CreateInterfaceEventRemoveMethod(node);
		}

		private void BindClassEvent(Event node, IType type, bool typeIsCallable)
		{
			Field backingField = CodeBuilder.CreateField("$event$" + node.Name, type);
			backingField.IsSynthetic = true;
			backingField.Modifiers = TypeMemberModifiers.Private;
			if (node.IsTransient)
			{
				backingField.Modifiers |= TypeMemberModifiers.Transient;
			}
			if (node.IsStatic)
			{
				backingField.Modifiers |= TypeMemberModifiers.Static;
			}
			node.DeclaringType.Members.Add(backingField);
			
			((InternalEvent)node.Entity).BackingField = (InternalField)backingField.Entity;
		
			if (null == node.Add)
			{
				node.Add = CreateEventAddMethod(node, backingField);
			}
			else
			{
				Visit(node.Add);
			}
			if (null == node.Remove)
			{
				node.Remove = CreateEventRemoveMethod(node, backingField);
			}
			else
			{
				Visit(node.Remove);
			}
			if (null == node.Raise)
			{
				if (typeIsCallable)
					node.Raise = CreateEventRaiseMethod(node, backingField);
			}
			else
			{
				Visit(node.Raise);
			}
		}
		
		override public void Run()
		{
			base.Run();
			BindAllParameters();
			BindAllEvents();
		}
		
		override public void Dispose()
		{
			_parameters.Clear();
			_events.Clear();
			base.Dispose();
		}

		IMethod Delegate_Combine
		{
			get
			{
				InitializeDelegateMethods();
				return _delegate_Combine;
			}
		}

		IMethod Delegate_Remove
		{
			get
			{
				InitializeDelegateMethods();
				return _delegate_Remove;
			}
		}

		void InitializeDelegateMethods()
		{
			if (null != _delegate_Combine)
			{
				return;
			}
			Type delegateType = Types.Delegate;
			Type[] delegates = new Type[] { delegateType, delegateType };
			_delegate_Combine = TypeSystemServices.Map(delegateType.GetMethod("Combine", delegates));
			_delegate_Remove = TypeSystemServices.Map(delegateType.GetMethod("Remove", delegates));
		}

		Method CreateInterfaceEventMethod(Event node, string prefix)
		{
			Method method = CodeBuilder.CreateMethod(prefix + node.Name,
				TypeSystemServices.VoidType,
				TypeMemberModifiers.Public | TypeMemberModifiers.Virtual | TypeMemberModifiers.Abstract);
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(
				CodeBuilder.GetFirstParameterIndex(node),
				"handler",
				GetType(node.Type)));
			return method;
		}
		
		Method CreateInterfaceEventAddMethod(Event node)
		{
			return CreateInterfaceEventMethod(node, "add_"); 
		}

		Method CreateInterfaceEventRemoveMethod(Event node)
		{
			return CreateInterfaceEventMethod(node, "remove_");
		}

		Method CreateEventMethod(Event node, string prefix)
		{
			Method method = CodeBuilder.CreateMethod(prefix + node.Name,
				TypeSystemServices.VoidType,
				node.Modifiers);
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(
				CodeBuilder.GetFirstParameterIndex(node),
				"handler",
				GetType(node.Type)));
			return method;
		}
		
		Method CreateEventAddMethod(Event node, Field backingField)
		{
			Method m = CreateEventMethod(node, "add_");
			m.Body.Add(
				CodeBuilder.CreateAssignment(
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateMethodInvocation(
				Delegate_Combine,
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateReference(m.Parameters[0]))));
			return m;
		}
		
		Method CreateEventRemoveMethod(Event node, Field backingField)
		{
			Method m = CreateEventMethod(node, "remove_");
			m.Body.Add(
				CodeBuilder.CreateAssignment(
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateMethodInvocation(
				Delegate_Remove,
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateReference(m.Parameters[0]))));
			return m;
		}
		
		TypeMemberModifiers RemoveAccessiblityModifiers(TypeMemberModifiers modifiers)
		{
			TypeMemberModifiers mask = TypeMemberModifiers.Public |
				TypeMemberModifiers.Protected |
				TypeMemberModifiers.Private |
				TypeMemberModifiers.Internal;
			return modifiers & ~mask ;
		}
		
		Method CreateEventRaiseMethod(Event node, Field backingField)
		{
			TypeMemberModifiers modifiers = RemoveAccessiblityModifiers(node.Modifiers);
			if (node.IsPrivate)
			{
				modifiers |= TypeMemberModifiers.Private;
			}
			else
			{
				modifiers |= TypeMemberModifiers.Protected | TypeMemberModifiers.Internal;
			}
			
			Method method = CodeBuilder.CreateMethod("raise_" + node.Name,
				TypeSystemServices.VoidType,
				modifiers);

			ICallableType type = GetEntity(node.Type) as ICallableType;
			if (null != type)
			{
				int index = CodeBuilder.GetFirstParameterIndex(node);
				foreach (IParameter parameter in type.GetSignature().Parameters)
				{
					method.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(
						index,
						parameter.Name,
						parameter.Type,
						parameter.IsByRef));
					++index;
				}
			}

			//assign backingField to local to avoid potential race condition between null-check and Invoke
			InternalLocal local = CodeBuilder.DeclareTempLocal(method, GetType(backingField.Type));
			BinaryExpression assignment = new BinaryExpression(
				BinaryOperatorType.Assign,
				CodeBuilder.CreateReference(local),
				CodeBuilder.CreateReference(backingField));
			method.Body.Add(assignment);

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateReference(local),
				NameResolutionService.ResolveMethod(GetType(backingField.Type), "Invoke"));
			foreach (ParameterDeclaration parameter in method.Parameters)
			{
				mie.Arguments.Add(CodeBuilder.CreateReference(parameter));
			}
			
			IfStatement stmt = new IfStatement(node.LexicalInfo);
			stmt.Condition = CodeBuilder.CreateReference(local);
			stmt.TrueBlock = new Block();
			stmt.TrueBlock.Add(mie);
			method.Body.Add(stmt);

			return method;
		}
	}
}

