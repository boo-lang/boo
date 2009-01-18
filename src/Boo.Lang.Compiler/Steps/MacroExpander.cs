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

using Boo.Lang.Compiler.Steps.Internal;

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Text;
	using System.Collections.Generic;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	
	internal class MacroExpander : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		private int _expanded;

		private bool _expandingInternalMacros;

		public bool ExpandingInternalMacros
		{
			get { return _expandingInternalMacros;  }
			set { _expandingInternalMacros = value;  }
		}

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
			try
			{
				Visit(module.Globals);
				Visit(module.Members);
			}
			finally
			{
				LeaveNamespace();
			}
		}
		
		override public void OnMacroStatement(MacroStatement node)
		{
			IType macroType = ResolveMacroName(node) as IType;
			if (null != macroType)
			{
				ExpandKnownMacro(node, macroType);
				return;
			}

			if (!ExpandingInternalMacros) // internal macros might appear later
				return;

			ExpandUnknownMacro(node);
		}

		private void ExpandKnownMacro(MacroStatement node, IType macroType)
		{
			EnsureVisitedRelatedNode(macroType);
			EnterNamespace(new NamespaceDelegator(CurrentNamespace, macroType));
			try
			{	
				ExpandChildrenOf(node);
			}
			finally
			{
				LeaveNamespace();
			}
			ProcessMacro(macroType, node);
		}

		private void EnsureVisitedRelatedNode(IType macroType)
		{
			AbstractInternalType internalType = macroType as AbstractInternalType;
			if (null == internalType)
				return;
			EnsureVisited(internalType.TypeDefinition);
		}

		private void ExpandUnknownMacro(MacroStatement node)
		{
			ExpandChildrenOf(node);
			TreatMacroAsMethodInvocation(node);
		}

		private void ExpandChildrenOf(MacroStatement node)
		{
			EnterExpansion();
			try
			{
				Visit(node.Block);
				Visit(node.Arguments);
			}
			finally
			{
				LeaveExpansion();
			}
		}

		private void LeaveExpansion()
		{
			--_expansionDepth;
		}

		private void EnterExpansion()
		{
			++_expansionDepth;
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
			if (!ExpandingInternalMacros)
				return;

			TypeDefinition macroDefinition = klass.TypeDefinition;
			bool firstTry = ! MacroCompiler.AlreadyCompiled(macroDefinition);
			Type macroType = new MacroCompiler(Context).Compile(macroDefinition);
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

		private static readonly object VisitedAnnotation = new object();

		private int _expansionDepth;

		private void EnsureVisited(TypeDefinition node)
		{
			if (node.ContainsAnnotation(VisitedAnnotation))
				return;
			node.Accept(this);
			node.Annotate(VisitedAnnotation);
		}
		
		private void ProcessingError(CompilerError error)
		{
			Errors.Add(error);
			RemoveCurrentNode();
		}

		private void ProcessMacro(Type actualType, MacroStatement node)
		{
            if (!typeof(IAstMacro).IsAssignableFrom(actualType))
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

				//use new-style BOO-1077 generator macro interface if available
				if (macro is IAstGeneratorMacro)
					return ExpandGeneratorMacro((IAstGeneratorMacro) macro, node);

				return macro.Expand(node);
			}
		}

		private Statement ExpandGeneratorMacro(IAstGeneratorMacro macroType, MacroStatement node)
		{
			IEnumerable<Node> generatedNodes = macroType.ExpandGenerator(node);
			if (null == generatedNodes)
				return null;

			Block resultingBlock = new Block();
			foreach (Node generatedNode in generatedNodes)
			{
				if (null == generatedNode) continue;

				TypeMember member = generatedNode as TypeMember;
				if (null != member)
				{
					if (_expansionDepth > 0)
						resultingBlock.Add(new TypeMemberStatement(member));
					else
						node.GetAncestor<TypeDefinition>().Members.Add(member);
					continue;
				}

				Statement statement = generatedNode as Statement;
				if (null != statement)
				{
					resultingBlock.Add((Statement) generatedNode);
					continue;
				}

				resultingBlock.Add((Expression) generatedNode);
			}

			return resultingBlock;
		}

		private static TypeDefinition GetEnclosingTypeOrModule(Node node)
		{
			TypeDefinition enclosingType = node.GetAncestor<TypeDefinition>();
			if (null != enclosingType)
				return enclosingType;

			throw new ArgumentException("node");
		}

		private IEntity ResolveMacroName(MacroStatement node)
		{
			string macroTypeName = BuildMacroTypeName(node.Name);
			IEntity entity = NameResolutionService.ResolveQualifiedName(macroTypeName);
			if (null != entity) return entity;
			return NameResolutionService.ResolveQualifiedName(node.Name);
		}

		private StringBuilder _buffer = new StringBuilder();

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

