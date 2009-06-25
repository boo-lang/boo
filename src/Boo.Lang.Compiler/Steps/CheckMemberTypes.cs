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
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;

	public class CheckMemberTypes : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			if (Parameters.DisabledWarnings.Contains("BCW0024"))
				return;

			Visit(CompileUnit.Modules);
		}

		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}

		override public void LeaveField(Field node)
		{
			LeaveMember(node);
		}

		override public void LeaveProperty(Property node)
		{
			LeaveMember(node);
		}

		override public void LeaveEvent(Event node)
		{
			LeaveMember(node);
		}

		override public void OnMethod(Method node)
		{
			//do not visit body
			LeaveMethod(node);
		}

		override public void LeaveMethod(Method node)
		{
			LeaveMember(node);
		}

		override public void LeaveConstructor(Constructor node)
		{
			LeaveMember(node);
		}

		void LeaveMember(TypeMember node)
		{
			CheckExplicitTypeForVisibleMember(node);
		}

		void CheckExplicitTypeForVisibleMember(TypeMember node)
		{
			if (node.IsSynthetic || !node.IsVisible)
				return;

			switch (node.NodeType) //TODO: introduce INodeWithType?
			{
				case NodeType.Constructor:
					CheckExplicitParametersType(node);
					return;
				case NodeType.Method:
					Method method = (Method)node;
					if (null != method.ParentNode && method.ParentNode.NodeType == NodeType.Property)
						return; //ignore accessors
					CheckExplicitParametersType(node);
					if (null != method.ReturnType)
						return;
					if (method.Entity != null
						&& ((IMethod)method.Entity).ReturnType == TypeSystemServices.VoidType)
						return;
					break;
				case NodeType.Property:
					if (null != ((Property)node).Type)
						return;
					break;
				case NodeType.Event:
					if (null != ((Event)node).Type)
						return;
					break;
				default:
					return; //fields, nested types etc...
			}

			Warnings.Add(CompilerWarningFactory.VisibleMemberDoesNotDeclareTypeExplicitely(node));
		}

		void CheckExplicitParametersType(TypeMember node)
		{
			INodeWithParameters @params = node as INodeWithParameters;
			if (null == @params)
				return;

			foreach (ParameterDeclaration p in @params.Parameters)
			{
				if (null == p.Type)
					Warnings.Add(CompilerWarningFactory.VisibleMemberDoesNotDeclareTypeExplicitely(node, p.Name));
			}
		}
	}
}
