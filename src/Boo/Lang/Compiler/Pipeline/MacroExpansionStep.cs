#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Pipeline
{
	using System;
	using System.Text;
	using Boo.Lang;
	using Boo.Lang.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Bindings;
	
	public class MacroExpansionStep : AbstractNamespaceSensitiveCompilerStep
	{
		StringBuilder _buffer = new StringBuilder();
		
		public override void Run()
		{
			Switch(CompileUnit);
		}
		
		public override void OnModule(Module module, ref Module resultingModule)
		{			
			PushNamespace(ImportResolutionStep.GetModuleNamespace(module));
			Switch(module.Members);
			Switch(module.Globals);			
			PopNamespace();
		}
		
		public override void OnMacroStatement(MacroStatement node, ref Statement resultingNode)
		{
			Switch(node.Block);
			Switch(node.Arguments);
			
			resultingNode = null;
			
			IBinding binding = ResolveQualifiedName(node, node.Name);
			if (null == binding)
			{
				binding = ResolveQualifiedName(node, BuildMacroTypeName(node.Name));
			}
			
			if (null == binding)
			{
				Errors.Add(CompilerErrorFactory.UnknownMacro(node, node.Name));
			}
			else
			{
				if (BindingType.TypeReference != binding.BindingType)
				{
					Errors.Add(CompilerErrorFactory.InvalidMacro(node, node.Name));
				}
				else
				{
					ITypeBinding macroType = ((TypeReferenceBinding)binding).BoundType;
					ExternalTypeBinding type = macroType as ExternalTypeBinding;
					if (null == type)
					{
						Errors.Add(CompilerErrorFactory.AstMacroMustBeExternal(node, macroType.FullName));
					}
					else
					{
						object macroInstance = Activator.CreateInstance(type.Type);
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
									resultingNode = macro.Expand(node);
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
