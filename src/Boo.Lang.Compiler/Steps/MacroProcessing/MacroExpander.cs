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
using System.Linq;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	public sealed class MacroExpander : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		private int _expanded;

		private Set<Node> _visited = new Set<Node>();

		private Queue<Statement> _pendingExpansions = new Queue<Statement>();

		private DynamicVariable<bool> _ignoringUnknownMacros = new DynamicVariable<bool>(false);

		public bool ExpandAll()
		{
			Reset();
			Run();
			BubbleUpPendingTypeMembers();
			return _expanded > 0;
		}

		private void Reset()
		{
			_pendingExpansions.Clear();
			_visited.Clear();
			_expanded = 0;
		}

		override public void Run()
		{
			ExpandModuleGlobalsIgnoringUnknownMacros();
			ExpandModules();
		}

		private void ExpandModules()
		{
			foreach (Module module in CompileUnit.Modules)
				ExpandOnModuleNamespace(module, VisitModule);
		}

		private void ExpandModuleGlobalsIgnoringUnknownMacros()
		{
			foreach (Module module in CompileUnit.Modules)
				ExpandModuleGlobalsIgnoringUnknownMacros(module);
		}

		private void ExpandModuleGlobalsIgnoringUnknownMacros(Module current)
		{
			_ignoringUnknownMacros.With(true, ()=> ExpandOnModuleNamespace(current, VisitGlobalsAllowingCancellation));
		}

		private void VisitGlobalsAllowingCancellation(Module module)
		{
			var globals = module.Globals.Statements;
			foreach (var stmt in globals.ToArray())
			{
				Node resultingNode;
				if (VisitAllowingCancellation(stmt, out resultingNode) && resultingNode != stmt)
					globals.Replace(stmt, (Statement) resultingNode);
				BubbleUpPendingTypeMembers();
			}
		}

		private void VisitModule(Module module)
		{
			Visit(module.Globals);
			Visit(module.Members);
		}
		
		void ExpandOnModuleNamespace(Module module, Action<Module> action)
		{
			EnterModuleNamespace(module);
			try
			{
				action(module);
			}
			finally
			{
				LeaveNamespace();
			}
		}

		private void EnterModuleNamespace(Module module)
		{
			EnterNamespace(InternalModule.ScopeFor(module));
		}

		public override bool EnterClassDefinition(ClassDefinition node)
		{
			if (WasVisited(node))
				return false;
			_visited.Add(node);
			return true;
		}

		bool _referenced;

		internal void EnsureCompilerAssemblyReference(CompilerContext context)
		{
			if (_referenced)
				return;

			if (null != context.References.Find("Boo.Lang.Compiler"))
			{
				_referenced = true;
				return;
			}

			context.References.Add(typeof(CompilerContext).Assembly);
			_referenced = true;
		}

		override public void OnMacroStatement(MacroStatement node)
		{
			EnsureCompilerAssemblyReference(Context);

			var macroType = ResolveMacroName(node) as IType;
			if (null != macroType)
			{
				ExpandKnownMacro(node, macroType);
				return;
			}

			if (_ignoringUnknownMacros.Value)
				Cancel();

			ExpandUnknownMacro(node);
		}

		private bool IsTopLevelExpansion()
		{
			return _expansionDepth == 0;
		}

		private void BubbleUpPendingTypeMembers()
		{
			while (_pendingExpansions.Count > 0)
				TypeMemberStatementBubbler.BubbleTypeMemberStatementsUp(_pendingExpansions.Dequeue());
		}

		private void ExpandKnownMacro(MacroStatement node, IType macroType)
		{
			ExpandChildrenOfMacroOnMacroNamespace(node, macroType);
			ProcessMacro(macroType, node);
		}

		private void ExpandChildrenOfMacroOnMacroNamespace(MacroStatement node, IType macroType)
		{
			EnterMacroNamespace(macroType);
			try
			{	
				ExpandChildrenOf(node);
			}
			finally
			{
				LeaveNamespace();
			}
		}

		private void EnterMacroNamespace(IType macroType)
		{
			EnsureNestedMacrosCanBeSeenAsMembers(macroType);

			EnterNamespace(new NamespaceDelegator(CurrentNamespace, macroType));
		}

		private static void EnsureNestedMacrosCanBeSeenAsMembers(IType macroType)
		{
			var internalMacroType = macroType as InternalClass;
			if (null != internalMacroType)
				TypeMemberStatementBubbler.BubbleTypeMemberStatementsUp(internalMacroType.TypeDefinition);
		}

		private void ExpandUnknownMacro(MacroStatement node)
		{
			ExpandChildrenOf(node);
			if (IsTypeMemberMacro(node))
				UnknownTypeMemberMacro(node);
			else
				TreatMacroAsMethodInvocation(node);
		}

		private static bool IsTypeMemberMacro(MacroStatement node)
		{
			return node.ParentNode.NodeType == NodeType.StatementTypeMember;
		}

		private void UnknownTypeMemberMacro(MacroStatement node)
		{
			var error = LooksLikeOldStyleFieldDeclaration(node)
				? CompilerErrorFactory.UnknownClassMacroWithFieldHint(node, node.Name)
				: CompilerErrorFactory.UnknownMacro(node, node.Name);
			ProcessingError(error);
		}

		private static bool LooksLikeOldStyleFieldDeclaration(MacroStatement node)
		{
			return node.Arguments.Count == 0 && node.Body.IsEmpty;
		}

		private void ExpandChildrenOf(MacroStatement node)
		{
			EnterExpansion();
			try
			{
				Visit(node.Body);
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
			var externalType = macroType as ExternalType;
			if (externalType == null)
			{
				InternalClass internalType = (InternalClass) macroType;
				ProcessInternalMacro(internalType, node);
				return;
			}

			ProcessMacro(externalType.ActualType, node);
		}

		private void ProcessInternalMacro(InternalClass klass, MacroStatement node)
		{
			TypeDefinition macroDefinition = klass.TypeDefinition;
			if (MacroDefinitionContainsMacroApplication(macroDefinition, node))
			{
				ProcessingError(CompilerErrorFactory.InvalidMacro(node, klass));
				return;
			}

			var macroCompiler = My<MacroCompiler>.Instance;
			bool firstTry = !macroCompiler.AlreadyCompiled(macroDefinition);
			Type macroType = macroCompiler.Compile(macroDefinition);
			if (macroType == null)
			{
				if (firstTry)
					ProcessingError(CompilerErrorFactory.AstMacroMustBeExternal(node, klass));
				else
					RemoveCurrentNode();
				return;
			}
			ProcessMacro(macroType, node);
		}

		private static bool MacroDefinitionContainsMacroApplication(TypeDefinition definition, MacroStatement statement)
		{
			return statement.GetAncestors<TypeDefinition>().Any(ancestor => ancestor == definition);
		}

		private int _expansionDepth;

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
				ProcessingError(CompilerErrorFactory.InvalidMacro(node, Map(actualType)));
				return;
			}

			++_expanded;
			
			try
			{
				var macroExpansion = ExpandMacro(actualType, node);
				var completeExpansion = ExpandMacroExpansion(node, macroExpansion);
				ReplaceCurrentNode(completeExpansion);
				if (completeExpansion != null && IsTopLevelExpansion())
					_pendingExpansions.Enqueue(completeExpansion);
			}
			catch (LongJumpException)
			{
				throw;
			}
			catch (Exception error)
			{
				ProcessingError(CompilerErrorFactory.MacroExpansionError(node, error));
			}
		}

		private IType Map(Type actualType)
		{
			return TypeSystemServices.Map(actualType);
		}

		private Statement ExpandMacroExpansion(MacroStatement node, Statement expansion)
		{
			if (null == expansion)
				return null;

			Statement modifiedExpansion = ApplyMacroModifierToExpansion(node, expansion);
			modifiedExpansion.InitializeParent(node.ParentNode);
			return Visit(modifiedExpansion);
		}

		private static Statement ApplyMacroModifierToExpansion(MacroStatement node, Statement expansion)
		{
			if (node.Modifier == null)
				return expansion;
			return NormalizeStatementModifiers.CreateModifiedStatement(node.Modifier, expansion);
		}

		private void TreatMacroAsMethodInvocation(MacroStatement node)
		{
			var invocation = new MethodInvocationExpression(node.LexicalInfo, new ReferenceExpression(node.LexicalInfo, node.Name))
			                 	{ Arguments = node.Arguments };
			if (node.ContainsAnnotation("compound") || !IsNullOrEmpty(node.Body))
				invocation.Arguments.Add(new BlockExpression(node.Body));
			ReplaceCurrentNode(new ExpressionStatement(node.LexicalInfo, invocation, node.Modifier));
		}
		
		private static bool IsNullOrEmpty(Block block)
		{
			return block == null || block.IsEmpty;
		}

		private Statement ExpandMacro(Type macroType, MacroStatement node)
		{
			var macro = (IAstMacro) Activator.CreateInstance(macroType);
			macro.Initialize(Context);

			//use new-style BOO-1077 generator macro interface if available
			var gm = macro as IAstGeneratorMacro;
			if (gm != null)
				return ExpandGeneratorMacro(gm, node);

			return macro.Expand(node);
		}

		private static Statement ExpandGeneratorMacro(IAstGeneratorMacro macroType, MacroStatement node)
		{
			IEnumerable<Node> generatedNodes = macroType.ExpandGenerator(node);
			if (null == generatedNodes)
				return null;

			return new NodeGeneratorExpander(node).Expand(generatedNodes);
		}

		private IEntity ResolveMacroName(MacroStatement node)
		{
			var macroTypeName = BuildMacroTypeName(node.Name);
			var entity = ResolvePreferringInternalMacros(macroTypeName)
				?? ResolvePreferringInternalMacros(node.Name);

			if (entity is IType)
				return entity;
			if (entity == null)
				return null;

			//we got something interesting, check if it is/has an extension method
			//that resolves a nested macro extension
			return ResolveMacroExtensionType(node, entity as IMethod)
				?? ResolveMacroExtensionType(node, entity as Ambiguous)
				?? entity; //return as-is
		}

		private IEntity ResolvePreferringInternalMacros(string macroTypeName)
		{
			IEntity resolved = NameResolutionService.ResolveQualifiedName(macroTypeName);
			Ambiguous ambiguous = resolved as Ambiguous;
			if (null != ambiguous && ambiguous.AllEntitiesAre(EntityType.Type))
				return Entities.PreferInternalEntitiesOverExternalOnes(ambiguous);
			return resolved;
		}

		IEntity ResolveMacroExtensionType(MacroStatement node, Ambiguous extensions)
		{
			if (null == extensions)
				return null;
			foreach (var entity in extensions.Entities)
			{
				var extensionType = ResolveMacroExtensionType(node, entity as IMethod);
				if (null != extensionType)
					return extensionType;
			}
			return null;
		}

		IEntity ResolveMacroExtensionType(MacroStatement node, IMethod extension)
		{
			if (null == extension)
				return null;
			IType extendedMacroType = GetExtendedMacroType(extension);
			if (null == extendedMacroType)
				return null;

			//ok now check if extension is correctly nested under parent
			foreach (MacroStatement parent in node.GetAncestors<MacroStatement>())
				if (ResolveMacroName(parent) == extendedMacroType)
					return GetExtensionMacroType(extension);

			return null;
		}

		IType GetExtendedMacroType(IMethod method)
		{
			InternalMethod internalMethod = method as InternalMethod;
			if (null != internalMethod)
			{
				Method extension = internalMethod.Method;
				if (!extension.Attributes.Contains(Types.CompilerGeneratedAttribute.FullName))
					return null;
				SimpleTypeReference sref = extension.Parameters[0].Type as SimpleTypeReference;
				if (null != sref && extension.Parameters.Count == 2)
				{
					IType type = NameResolutionService.ResolveQualifiedName(sref.Name) as IType;
					if (type != null && type.Name.EndsWith("Macro")) //no entity yet
						return type;
				}
			}
			else if (method is ExternalMethod && method.IsExtension)
			{
				var parameters = method.GetParameters();
				if (parameters.Length == 2 && TypeSystemServices.IsMacro(parameters[0].Type))
					return parameters[0].Type;
			}
			return null;
		}

		IType GetExtensionMacroType(IMethod method)
		{
			InternalMethod internalMethod = method as InternalMethod;
			if (null != internalMethod)
			{
				Method extension = internalMethod.Method;
				SimpleTypeReference sref = extension.ReturnType as SimpleTypeReference;
				if (null != sref)
				{
					IType type = NameResolutionService.ResolveQualifiedName(sref.Name) as IType;
					if (type != null && type.Name.EndsWith("Macro"))//no entity yet
						return type;
				}
			}
			else if (method is ExternalMethod)
				return method.ReturnType;

			return null;
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
