using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	public class ExpandComplexSlicingExpressions : MethodTrackingVisitorCompilerStep
	{
		public override void OnSlicingExpression(SlicingExpression node)
		{
			base.OnSlicingExpression(node);

			if (!node.IsComplexSlicing())
				return;

			if (node.IsTargetOfAssignment())
				return;

			CompleteOmittedExpressions(node);
			ExpandComplexSlicing(node);
		}

		void CompleteOmittedExpressions(SlicingExpression node)
		{
			foreach (var index in node.Indices.Where(slice => slice.Begin == OmittedExpression.Default))
				index.Begin = CodeBuilder.CreateIntegerLiteral(0);
		}

		private void ExpandComplexSlicing(SlicingExpression node)
		{
			var targetType = GetExpressionType(node.Target);
			if (IsString(targetType))
				BindComplexStringSlicing(node);
			else if (IsList(targetType))
				BindComplexListSlicing(node);
			else if (targetType.IsArray)
				BindComplexArraySlicing(node);
			else
				NotImplemented(node, "complex slicing for anything but lists, arrays and strings");
		}

		private bool IsString(IType targetType)
		{
			return TypeSystemServices.StringType == targetType;
		}

		private bool IsList(IType targetType)
		{
			return IsAssignableFrom(TypeSystemServices.ListType, targetType);
		}

		void BindComplexListSlicing(SlicingExpression node)
		{
			var slice = node.Indices[0];

			MethodInvocationExpression mie = null;
			if (slice.End == null || slice.End == OmittedExpression.Default)
			{
				mie = CodeBuilder.CreateMethodInvocation(node.Target, MethodCache.List_GetRange1);
				mie.Arguments.Add(slice.Begin);
			}
			else
			{
				mie = CodeBuilder.CreateMethodInvocation(node.Target, MethodCache.List_GetRange2);
				mie.Arguments.Add(slice.Begin);
				mie.Arguments.Add(slice.End);
			}
			node.ParentNode.Replace(node, mie);
		}

		void BindComplexArraySlicing(SlicingExpression node)
		{
			if (node.Indices.Count > 1)
			{
				MethodInvocationExpression mie = null;
				var compute_end = new ArrayLiteralExpression();
				var collapse = new ArrayLiteralExpression();
				var ranges = new ArrayLiteralExpression();
				for (int i = 0; i < node.Indices.Count; i++)
				{
					ranges.Items.Add(node.Indices[i].Begin);
					if (node.Indices[i].End == null )
					{
						var end = new BinaryExpression(BinaryOperatorType.Addition,
						                                            node.Indices[i].Begin,
						                                            new IntegerLiteralExpression(1));
						ranges.Items.Add(end);
						BindExpressionType(end, GetExpressionType(node.Indices[i].Begin));
						compute_end.Items.Add(new BoolLiteralExpression(false));
						collapse.Items.Add(new BoolLiteralExpression(true));
					}
					else if (node.Indices[i].End == OmittedExpression.Default)
					{
						var end = new IntegerLiteralExpression(0);
						ranges.Items.Add(end);
						BindExpressionType(end, GetExpressionType(node.Indices[i].Begin));
						compute_end.Items.Add(new BoolLiteralExpression(true));
						collapse.Items.Add(new BoolLiteralExpression(false));
					}
					else
					{
						ranges.Items.Add(node.Indices[i].End);
						compute_end.Items.Add(new BoolLiteralExpression(false));
						collapse.Items.Add(new BoolLiteralExpression(false));
					}
				}
				mie = CodeBuilder.CreateMethodInvocation(MethodCache.RuntimeServices_GetMultiDimensionalRange1, node.Target, ranges);
				mie.Arguments.Add(compute_end);
				mie.Arguments.Add(collapse);

				BindExpressionType(ranges, TypeSystemServices.Map(typeof(int[])));
				BindExpressionType(compute_end, TypeSystemServices.Map(typeof(bool[])));
				BindExpressionType(collapse, TypeSystemServices.Map(typeof(bool[])));
				node.ParentNode.Replace(node, mie);
			}
			else
			{
				Slice slice = node.Indices[0];
				MethodInvocationExpression mie = null;

				if (null == slice.End || slice.End == OmittedExpression.Default)
				{
					mie = CodeBuilder.CreateMethodInvocation(MethodCache.RuntimeServices_GetRange1, node.Target, slice.Begin);
				}
				else
				{
					mie = CodeBuilder.CreateMethodInvocation(MethodCache.RuntimeServices_GetRange2, node.Target, slice.Begin, slice.End);
				}
				node.ParentNode.Replace(node, mie);
			}
		}

		static bool NeedsNormalization(Expression index)
		{
			return index.NodeType != NodeType.IntegerLiteralExpression || ((IntegerLiteralExpression) index).Value < 0;
		}

		void BindComplexStringSlicing(SlicingExpression node)
		{
			var slice = node.Indices[0];

			MethodInvocationExpression mie = null;

			if (null == slice.End || slice.End == OmittedExpression.Default)
			{
				if (NeedsNormalization(slice.Begin))
				{
					mie = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
					mie.ExpressionType = TypeSystemServices.StringType;

					InternalLocal temp = DeclareTempLocal(TypeSystemServices.StringType);
					mie.Arguments.Add(
						CodeBuilder.CreateAssignment(
							CodeBuilder.CreateReference(temp),
							node.Target));

					mie.Arguments.Add(
						CodeBuilder.CreateMethodInvocation(
							CodeBuilder.CreateReference(temp),
							MethodCache.String_Substring_Int,
							CodeBuilder.CreateMethodInvocation(
								MethodCache.RuntimeServices_NormalizeStringIndex,
								CodeBuilder.CreateReference(temp),
								slice.Begin)));
				}
				else
				{
					mie = CodeBuilder.CreateMethodInvocation(node.Target, MethodCache.String_Substring_Int, slice.Begin);
				}
			}
			else
			{
				mie = CodeBuilder.CreateMethodInvocation(MethodCache.RuntimeServices_Mid, node.Target, slice.Begin, slice.End);
			}

			node.ParentNode.Replace(node, mie);
		}

		static bool IsAssignableFrom(IType expectedType, IType actualType)
		{
			return TypeCompatibilityRules.IsAssignableFrom(expectedType, actualType);
		}

		protected InternalLocal DeclareTempLocal(IType localType)
		{
			return CodeBuilder.DeclareTempLocal(CurrentMethod, localType);
		}

		protected RuntimeMethodCache MethodCache
		{
			get { return _methodCache.Instance; }
		}

		public override void Dispose()
		{
			_methodCache = new EnvironmentProvision<RuntimeMethodCache>();
			base.Dispose();
		}

		private EnvironmentProvision<RuntimeMethodCache> _methodCache;
	}
}