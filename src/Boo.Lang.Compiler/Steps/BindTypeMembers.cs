#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
