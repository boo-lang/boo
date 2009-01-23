using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using System;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	class NodeGeneratorExpander
	{
		private readonly MacroStatement _node;
		private readonly bool _addTypeMembersToEnclosingTypeDefinition;

		public NodeGeneratorExpander(MacroStatement node, bool addTypeMembersToEnclosingTypeDefinition)
		{
			_node = node;
			_addTypeMembersToEnclosingTypeDefinition = addTypeMembersToEnclosingTypeDefinition;
		}

		public Statement Expand(IEnumerable<Node> generator)
		{
			Block resultingBlock = new Block();
			foreach (Node node in generator)
			{
				//'yield' (ie. implicit 'yield null') means 'yield `macro`.Body'
				Node generatedNode = node ?? _node.Body;
				if (null == generatedNode)
					continue;

				TypeMember member = generatedNode as TypeMember;
				if (null != member)
				{
					if (_addTypeMembersToEnclosingTypeDefinition)
						_node.GetAncestor<TypeDefinition>().Members.Add(member);
					else
						resultingBlock.Add(new TypeMemberStatement(member));
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

				Import import = generatedNode as Import;
				if (null != import)
				{
					ExpandImport(import);
					continue;
				}

				throw new CompilerError(_node, "Unsupported expansion: " + generatedNode.ToCodeString());
			}

			return resultingBlock.IsEmpty
					? null
					: resultingBlock.Simplify();
		}

		private void ExpandImport(Import import)
		{
			ImportCollection imports = _node.GetAncestor<Module>().Imports;
			if (imports.Contains(import.Matches))
				return;

			imports.Add(import);
			BindImport(import);
		}

		private void BindImport(Import import)
		{
			CompilerContext context = CompilerContext.Current;
			INamespace previous = context.NameResolutionService.CurrentNamespace;
			try
			{
				context.NameResolutionService.Reset();

				BindNamespaces namespaceBinder = new BindNamespaces();
				namespaceBinder.Initialize(context);
				import.Accept(namespaceBinder);
			}
			catch (Exception x)
			{
				throw new CompilerError(_node, "Error expanding " + import.ToCodeString(), x);
			}
			finally
			{
				context.NameResolutionService.EnterNamespace(previous);
			}
		}
	}
}
