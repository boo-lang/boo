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

using System;
using System.Collections;
using System.Reflection;
using List=Boo.Lang.List;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{		
	public class BindNamespaces : AbstractCompilerStep
	{			
		override public void Run()
		{
			NameResolutionService.Reset();
			
			foreach (Boo.Lang.Compiler.Ast.Module module in CompileUnit.Modules)
			{
				foreach (Import import in module.Imports)
				{
					IEntity tag = NameResolutionService.ResolveQualifiedName(import.Namespace);					
					if (null == tag)
					{
						tag = TypeSystemServices.ErrorEntity;
						Errors.Add(CompilerErrorFactory.InvalidNamespace(import));
					}
					else
					{
						if (null != import.AssemblyReference)
						{	
							NamespaceEntity nsInfo = tag as NamespaceEntity;
							if (null == nsInfo)
							{
								Errors.Add(CompilerErrorFactory.NotImplemented(import, "assembly qualified type references"));
							}
							else
							{								
								tag = new AssemblyQualifiedNamespaceEntity(GetBoundAssembly(import.AssemblyReference), nsInfo);
							}
						}
						if (null != import.Alias)
						{
							tag = new AliasedNamespace(import.Alias.Name, tag);
							import.Alias.Entity = tag;
						}
					}
					
					_context.TraceInfo("{1}: import reference '{0}' bound to {2}.", import, import.LexicalInfo, tag.Name);
					import.Entity = tag;
				}
			}			
		}
		
		Assembly GetBoundAssembly(ReferenceExpression reference)
		{
			return ((AssemblyReference)TypeSystemServices.GetEntity(reference)).Assembly;
		}
	}
}
