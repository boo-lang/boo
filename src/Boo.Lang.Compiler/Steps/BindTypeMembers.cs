#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class BindTypeMembers : AbstractVisitorCompilerStep
	{
		public BindTypeMembers()
		{
		}
		
		override public void OnMethod(Method node)
		{
			if (null == node.Tag)
			{
				node.Tag = new InternalMethod(TypeSystemServices, node);
				BindParameters(node, node.Parameters);
			}
		}
		
		void BindParameters(TypeMember member, ParameterDeclarationCollection parameters)
		{			
			// arg0 is the this pointer when member is not static			
			int delta = member.IsStatic ? 0 : 1;
			
			for (int i=0; i<parameters.Count; ++i)
			{
				ParameterDeclaration parameter = parameters[i];
				if (null == parameter.Type)
				{
					parameter.Type = CreateTypeReference(TypeSystemServices.ObjectType);
				}
				else
				{
					NameResolutionService.ResolveTypeReference(parameter.Type);
				}
				parameter.Tag = new InternalParameter(parameter, i + delta);
			}
		}
		
		override public void OnConstructor(Constructor node)
		{
			if (null == node.Tag)
			{
				node.Tag = new InternalConstructor(TypeSystemServices, node);
				BindParameters(node, node.Parameters);
			}
		}
		
		override public void OnField(Field node)
		{
			if (null == node.Tag)
			{
				node.Tag = new InternalField(TypeSystemServices, node);
			}
		}
		
		override public void OnProperty(Property node)
		{
			if (null == node.Tag)
			{				
				node.Tag = new InternalProperty(TypeSystemServices, node);
				BindParameters(node, node.Parameters);
			}
			
			Accept(node.Getter);
			Accept(node.Setter);
		}	
		
		override public void Run()
		{
			NameResolutionService.Reset();
			Accept(CompileUnit.Modules);
		}
		
		override public void OnModule(Module module)
		{
			NameResolutionService.EnterNamespace((INamespace)module.Tag);
			Accept(module.Members);
			NameResolutionService.LeaveNamespace();
		}
	}
}
