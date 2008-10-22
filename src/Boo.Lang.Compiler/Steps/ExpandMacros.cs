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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Text;
	using System.Collections.Generic;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ExpandMacros : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		private StringBuilder _buffer = new StringBuilder();

		private int _expanded;

		public bool ExpandAll()
		{
			_expanded = 0;
			Run();
			return _expanded > 0;
		}
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));
			Visit(module.Globals);
			Visit(module.Members);
			LeaveNamespace();
		}
		
		override public void OnMacroStatement(MacroStatement node)
		{
			Visit(node.Block);
			Visit(node.Arguments);

			IType entity = ResolveMacroName(node) as IType;
			if (null != entity)
			{
				ProcessMacro(entity, node);
				return;
			}
			TreatMacroAsMethodInvocation(node);
		}

		private void ProcessMacro(IType macroType, MacroStatement node)
		{
			ExternalType type = macroType as ExternalType;
			if (null == type)
			{
				InternalClass klass = (InternalClass) macroType;
				ProcessInternalMacro(klass, node);
				return;
			}

			ProcessMacro(type.ActualType, node);
		}

		private void ProcessInternalMacro(InternalClass klass, MacroStatement node)
		{
			bool firstTry = ! MacroCompiler.AlreadyCompiled(klass.TypeDefinition);
			Type macroType = new MacroCompiler(Context).Compile(klass);
			if (null == macroType)
			{
				if (firstTry)
				{
					ProcessingError(CompilerErrorFactory.AstMacroMustBeExternal(node, klass.FullName));
				}
				else
				{
					RemoveCurrentNode();
				}
				return;
			}
			ProcessMacro(macroType, node);
		}

		private void ProcessingError(CompilerError error)
		{
			Errors.Add(error);
			RemoveCurrentNode();
		}

		private void ProcessMacro(Type actualType, MacroStatement node)
		{
            // HACK: workaround for mono
			if (-1 == Array.IndexOf(actualType.GetInterfaces(), typeof(IAstMacro)))
//			if (!typeof(IAstMacro).IsAssignableFrom(actualType))
			{
				ProcessingError(CompilerErrorFactory.InvalidMacro(node, actualType.FullName));
				return;
			}
			
			try
			{
				Statement replacement = ExpandMacro(actualType, node);
				if (null != node.Modifier)
				{
					replacement = NormalizeStatementModifiers.CreateModifiedStatement(node.Modifier, replacement);
				}
				ReplaceCurrentNode(replacement);

				++_expanded;
			}
			catch (Exception error)
			{
				ProcessingError(CompilerErrorFactory.MacroExpansionError(node, error));
			}
		}

		private void TreatMacroAsMethodInvocation(MacroStatement node)
		{
			MethodInvocationExpression invocation = new MethodInvocationExpression(
				node.LexicalInfo,
				new ReferenceExpression(node.LexicalInfo, node.Name));
			invocation.Arguments = node.Arguments;
			if (node.ContainsAnnotation("compound")
				|| !IsNullOrEmpty(node.Block))
			{
				invocation.Arguments.Add(new BlockExpression(node.Block));
			}

			ReplaceCurrentNode(new ExpressionStatement(node.LexicalInfo, invocation, node.Modifier));
		}
		
		private static bool IsNullOrEmpty(Block block)
		{
			return block == null || block.Statements.Count == 0;
		}

		private Statement ExpandMacro(Type macroType, MacroStatement node)
		{
			using (IAstMacro macro = (IAstMacro) Activator.CreateInstance(macroType))
			{
				macro.Initialize(_context);

				//use new-style BOO-1077 macro interface if available
				if (null != (macro as IAstEnumerableMacro))
					return ExpandEnumerableMacro((IAstEnumerableMacro) macro, node);

				return macro.Expand(node);
			}
		}

		private Statement ExpandEnumerableMacro(IAstEnumerableMacro macroType, MacroStatement node)
		{
			IEnumerable<Node> nodes = macroType.EnumerableExpand(node);
			if (null == nodes)
				return null;

			Block block = null;
			TypeDefinition enclosingType = null;

			foreach (Node n in nodes)
			{
				if (null == n) continue; //nop

				TypeMember member = n as TypeMember;
				if (null != member)
				{
					if (null == enclosingType)
						enclosingType = GetEnclosingTypeOrModule(node);
					enclosingType.Members.Add(member);
				}
				else
				{
					if (null == block)
						block = new Block();
					if (null != (n as Statement))
						block.Add((Statement) n);
					else
						block.Add((Expression) n);
				}
			}

			return block;
		}

		private static TypeDefinition GetEnclosingTypeOrModule(Node node)
		{
			while (null != node) {
				if (null != (node as TypeDefinition))
					return (TypeDefinition) node;
				node = node.ParentNode;
			}
			throw new ArgumentException("node");
		}

		private IEntity ResolveMacroName(MacroStatement node)
		{
			IEntity entity = NameResolutionService.ResolveQualifiedName(BuildMacroTypeName(node.Name));
			if (null != entity) return entity;
			return NameResolutionService.ResolveQualifiedName(node.Name);
		}

		private string BuildMacroTypeName(string name)
		{
			_buffer.Length = 0;
			if (!char.IsUpper(name[0]))
			{
				_buffer.Append(char.ToUpper(name[0]));
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

