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
		Boo.Lang.List _parameters = new Boo.Lang.List();
		
		public BindTypeMembers()
		{
		}
		
		override public void OnMethod(Method node)
		{
			if (null == node.Entity)
			{
				node.Entity = new InternalMethod(TypeSystemServices, node);
				_parameters.Add(node);
			}
		}
		
		void BindAllParameters()
		{
			foreach (INodeWithParameters node in _parameters)
			{
				TypeMember member = (TypeMember)node;
				NameResolutionService.Restore((INamespace)TypeSystemServices.GetEntity(member.DeclaringType));
				BindParameters(member, node.Parameters);
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
				parameter.Entity = new InternalParameter(parameter, i + delta);
			}
		}
		
		override public void OnConstructor(Constructor node)
		{
			if (null == node.Entity)
			{
				node.Entity = new InternalConstructor(TypeSystemServices, node);
				_parameters.Add(node);
			}
		}
		
		override public void OnField(Field node)
		{
			if (null == node.Entity)
			{
				node.Entity = new InternalField(TypeSystemServices, node);
			}
		}
		
		override public void OnProperty(Property node)
		{
			if (null == node.Entity)
			{				
				node.Entity = new InternalProperty(TypeSystemServices, node);
				_parameters.Add(node);
			}
			
			Visit(node.Getter);
			Visit(node.Setter);
		}	
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members);
		}
		
		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}
		
		override public void Run()
		{			
			NameResolutionService.Reset();
			Visit(CompileUnit.Modules);
			BindAllParameters();
		}
		
		override public void Dispose()
		{
			base.Dispose();
			_parameters.Clear();
		}
	}
}
