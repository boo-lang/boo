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
	using System;
	using System.Text;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ExpandMacros : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		StringBuilder _buffer = new StringBuilder();
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{			
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));
			Visit(module.Members);
			Visit(module.Globals);			
			LeaveNamespace();
		}
		
		override public void OnMacroStatement(MacroStatement node)
		{
			Visit(node.Block);
			Visit(node.Arguments);
			
			Node replacement = null;
			
			IEntity tag = NameResolutionService.ResolveQualifiedName(node.Name);
			if (null == tag)
			{
				tag = NameResolutionService.ResolveQualifiedName(BuildMacroTypeName(node.Name));
			}
			
			if (null == tag)
			{
				Errors.Add(CompilerErrorFactory.UnknownMacro(node, node.Name));
			}
			else
			{
				if (EntityType.Type != tag.EntityType)
				{
					Errors.Add(CompilerErrorFactory.InvalidMacro(node, node.Name));
				}
				else
				{
					IType macroType = (IType)tag;
					ExternalType type = macroType as ExternalType;
					if (null == type)
					{
						Errors.Add(CompilerErrorFactory.AstMacroMustBeExternal(node, macroType.FullName));
					}
					else
					{
						object macroInstance = Activator.CreateInstance(type.ActualType);
						if (!(macroInstance is IAstMacro))
						{
							Errors.Add(CompilerErrorFactory.InvalidMacro(node, macroType.FullName));
						}
						else
						{							
							try
							{
								using (IAstMacro macro = ((IAstMacro)macroInstance))
								{
									macro.Initialize(_context);
									replacement = macro.Expand(node);
								}
							}
							catch (Exception error)
							{
								Errors.Add(CompilerErrorFactory.MacroExpansionError(node, error));
							}
						}
					}
				}
			}
			ReplaceCurrentNode(replacement);
		}
		
		string BuildMacroTypeName(string name)
		{
			_buffer.Length = 0;
			if (!Char.IsUpper(name[0]))
			{
				_buffer.Append(Char.ToUpper(name[0]));
				_buffer.Append(name.Substring(1));
				_buffer.Append("Macro");
			}
			else
			{
				_buffer.Append(name);
				_buffer.Append("Macro");
			}
			return _buffer.ToString();
		}
	}
}
