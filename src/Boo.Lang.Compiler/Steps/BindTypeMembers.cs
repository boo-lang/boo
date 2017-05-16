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
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	
	public class BindTypeMembers : BindMethods
	{
        private readonly List _parameters = new List();
        private readonly List _events = new List();
        private IMethod _delegate_Combine;
		private IMethod _delegate_Remove;

		private const string PrivateMemberNeverUsed = "PrivateMemberNeverUsed";

		public override void OnMethod(Method node)
		{
			_parameters.Add(node);
			base.OnMethod(node);
		}

        private void BindAllParameters()
		{
			Method entryPoint = ContextAnnotations.GetEntryPoint(Context);

			foreach (INodeWithParameters node in _parameters)
			{
				var member = (TypeMember)node;

				if (member.ContainsAnnotation(PrivateMemberNeverUsed))
					continue;

				NameResolutionService.EnterNamespace((INamespace)TypeSystemServices.GetEntity(member.DeclaringType));
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
		
		public override void OnConstructor(Constructor node)
		{
			_parameters.Add(node);
			base.OnConstructor(node);
		}
		
		public override void OnField(Field node)
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
		
		public override void OnProperty(Property node)
		{
			EnsureEntityFor(node);
			_parameters.Add(node);
			
			Visit(node.Getter);
			Visit(node.Setter);
			Visit(node.ExplicitInfo);
		}

		public override void OnEvent(Event node)
		{
			_events.Add(node);
		}

        private void BindAllEvents()
		{
			foreach (Event node in _events)
			{
				BindEvent(node);
			}
		}

        private void BindEvent(Event node)
		{
			EnsureEntityFor(node);

			var type = GetType(node.Type);
			var declaringType = GetType(node.DeclaringType);
			var typeIsCallable = type is ICallableType;
			if (!typeIsCallable)
			{
				Errors.Add(
					CompilerErrorFactory.EventTypeIsNotCallable(node.Type, type));
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
			var backingField = CodeBuilder.CreateField("$event$" + node.Name, type);
			backingField.IsSynthetic = true;
			backingField.Modifiers = TypeMemberModifiers.Private;
			if (node.HasTransientModifier)
				backingField.Modifiers |= TypeMemberModifiers.Transient;
			if (node.IsStatic)
				backingField.Modifiers |= TypeMemberModifiers.Static;
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
		
		public override void Run()
		{
			base.Run();
			BindAll();
		}

		public override TypeMember Reify(TypeMember node)
		{
			base.Reify(node);
			BindAll();
			return node;
		}

		private void BindAll()
		{
			BindAllParameters();
			BindAllEvents();
			Reset();
		}

		private void Reset()
		{
			_parameters.Clear();
			_events.Clear();
		}

        private IMethod Delegate_Combine
		{
			get
			{
				InitializeDelegateMethods();
				return _delegate_Combine;
			}
		}

        private IMethod Delegate_Remove
		{
			get
			{
				InitializeDelegateMethods();
				return _delegate_Remove;
			}
		}

        private void InitializeDelegateMethods()
		{
			if (null != _delegate_Combine)
			{
				return;
			}
			_delegate_Combine = TypeSystemServices.Map(Methods.Of<Delegate, Delegate, Delegate>(Delegate.Combine));
			_delegate_Remove = TypeSystemServices.Map(Methods.Of<Delegate, Delegate, Delegate>(Delegate.Remove));
		}

        private Method CreateInterfaceEventMethod(Event node, string prefix)
		{
			var method = CodeBuilder.CreateMethod(prefix + node.Name,
				TypeSystemServices.VoidType,
				TypeMemberModifiers.Public | TypeMemberModifiers.Virtual | TypeMemberModifiers.Abstract);
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(
				CodeBuilder.GetFirstParameterIndex(node),
				"handler",
				GetType(node.Type)));
			return method;
		}

        private Method CreateInterfaceEventAddMethod(Event node)
		{
			return CreateInterfaceEventMethod(node, "add_"); 
		}

        private Method CreateInterfaceEventRemoveMethod(Event node)
		{
			return CreateInterfaceEventMethod(node, "remove_");
		}

		private Method CreateEventMethod(Event node, string prefix)
		{
			var method = CodeBuilder.CreateMethod(prefix + node.Name,
				TypeSystemServices.VoidType,
				node.Modifiers);
			method.Parameters.Add(
				CodeBuilder.CreateParameterDeclaration(
				CodeBuilder.GetFirstParameterIndex(node),
				"handler",
				GetType(node.Type)));
			return method;
		}
		
		private Method CreateEventAddMethod(Event node, Field backingField)
		{
			var m = CreateEventMethod(node, "add_");
			m.Body.Add(
				CodeBuilder.CreateAssignment(
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateMethodInvocation(
				Delegate_Combine,
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateReference(m.Parameters[0]))));
			return m;
		}
		
		private Method CreateEventRemoveMethod(Event node, Field backingField)
		{
			var m = CreateEventMethod(node, "remove_");
			m.Body.Add(
				CodeBuilder.CreateAssignment(
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateMethodInvocation(
				Delegate_Remove,
				CodeBuilder.CreateReference(backingField),
				CodeBuilder.CreateReference(m.Parameters[0]))));
			return m;
		}
		
		private static TypeMemberModifiers RemoveAccessiblityModifiers(TypeMemberModifiers modifiers)
		{
		    const TypeMemberModifiers mask = TypeMemberModifiers.Public |
		                                     TypeMemberModifiers.Protected |
		                                     TypeMemberModifiers.Private |
		                                     TypeMemberModifiers.Internal;
		    return modifiers & ~mask ;
		}
		
		private Method CreateEventRaiseMethod(Event node, Field backingField)
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

			var returnType = ((ICallableType)node.Type.Entity).GetSignature().ReturnType;

			var method = CodeBuilder.CreateMethod("raise_" + node.Name,
				returnType,
				modifiers);

			var type = GetEntity(node.Type) as ICallableType;
			if (null != type)
			{
				var index = CodeBuilder.GetFirstParameterIndex(node);
				foreach (var parameter in type.GetSignature().Parameters)
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
			var local = CodeBuilder.DeclareTempLocal(method, GetType(backingField.Type));
			var assignment = new BinaryExpression(
				BinaryOperatorType.Assign,
				CodeBuilder.CreateReference(local),
				CodeBuilder.CreateReference(backingField));
			method.Body.Add(assignment);

			var mie = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateReference(local),
				NameResolutionService.ResolveMethod(GetType(backingField.Type), "Invoke"));
			foreach (var parameter in method.Parameters)
			{
				mie.Arguments.Add(CodeBuilder.CreateReference(parameter));
			}

		    var stmt = new IfStatement(node.LexicalInfo)
		    {
		        Condition = CodeBuilder.CreateReference(local),
		        TrueBlock = new Block()
		    };
			if (returnType == TypeSystemServices.VoidType)
				stmt.TrueBlock.Add(mie);
			else stmt.TrueBlock.Add(new ReturnStatement(mie));
			method.Body.Add(stmt);

			return method;
		}
	}
}

