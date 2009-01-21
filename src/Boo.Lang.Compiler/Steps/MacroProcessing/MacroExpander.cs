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

using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	internal class MacroExpander : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		private int _expanded;

		private Set<Node> _visited = new Set<Node>();

		public bool ExpandAll()
		{
			Reset();
			Run();
			return _expanded > 0;
		}

		private void Reset()
		{
			_visited.Clear();
			_expanded = 0;
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

		public override bool EnterClassDefinition(ClassDefinition node)
		{
			if (WasVisited(node))
				return false;
			_visited.Add(node);
			return true;
		}
		
		override public void OnMacroStatement(MacroStatement node)
		{
			IType macroType = ResolveMacroName(node) as IType;
			if (null != macroType)
			{
				ExpandKnownMacro(node, macroType);
				if (_expansionDepth == 0)
					BubbleResultingTypeMemberStatementsUp();
				return;
			}
			ExpandUnknownMacro(node);
		}

		private void BubbleResultingTypeMemberStatementsUp()
		{
			CompileUnit.Accept(new TypeMemberStatementBubbler());
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

		private int _expansionDepth;

		private void EnsureVisited(TypeDefinition node)
		{
			if (WasVisited(node))
				return;
			node.Accept(this);
		}

		private bool WasVisited(TypeDefinition node)
		{
			return _visited.Contains(node);
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

			++_expanded;
			
			try
			{
				Statement expansion = ExpandMacro(actualType, node);
				ReplaceCurrentNode(ExpandMacroExpansion(node, expansion));
			}
			catch (Exception error)
			{
				ProcessingError(CompilerErrorFactory.MacroExpansionError(node, error));
			}
		}

		private Statement ExpandMacroExpansion(MacroStatement node, Statement expansion)
		{
			if (null == expansion)
				return null;

			Statement modifiedExpansion = ApplyMacroModifierToExpansion(node, expansion);
			modifiedExpansion.InitializeParent(node.ParentNode);
			return Visit(modifiedExpansion);
		}

		private Statement ApplyMacroModifierToExpansion(MacroStatement node, Statement expansion)
		{
			if (null == node.Modifier)
				return expansion;
			return NormalizeStatementModifiers.CreateModifiedStatement(node.Modifier, expansion);
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
			return block == null || block.IsEmpty;
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
			foreach (Node gn in generatedNodes)
			{
				//'yield' (ie. implicit 'yield null') means 'yield `macro`.Block'
				Node generatedNode = gn ?? node.Block;
				if (null == generatedNode)
					continue;

				TypeMember member = generatedNode as TypeMember;
				if (null != member)
				{
					if (_expansionDepth > 0)
						resultingBlock.Add(new TypeMemberStatement(member));
					else
						node.GetAncestor<TypeDefinition>().Members.Add(member);
					continue;
				}
				
				Block block = generatedNode as Block;
				if (null != block)
				{
					resultingBlock.Add(block);
					continue;
				}

				Statement statement = generatedNode as Statement;
				if (null != statement)
				{
					resultingBlock.Add(statement);
					continue;
				}

				Expression expression = generatedNode as Expression;
				if (null != expression)
				{
					resultingBlock.Add(expression);
					continue;
				}
				
				throw new CompilerError(node, "Unsupported expansion: " + generatedNode);
			}
			
			return resultingBlock.IsEmpty
				? null
				: Simplify(resultingBlock);
		}

		private Statement Simplify(Block resultingBlock)
		{
			return (resultingBlock.Statements.Count > 1 || resultingBlock.HasAnnotations)
			       	? resultingBlock
			       	: resultingBlock.Statements[0];
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