#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Ast
{
	/// <summary>
	/// Delimits a block of code where the original lexical info for code literals should be preserved.
	/// 
	/// preservingLexicalInfo:
	///     print [| foo |].LexicalInfo.FileName # prints current file name
	/// print [| bar |].LexicalInfo.FileName # prints nothing
	/// 
	/// Useful for stepping into macro generated code.
	/// </summary>
	public class PreservingLexicalInfoMacro : AbstractAstMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			if (macro.Arguments.Count > 0)
				throw new ArgumentException("preservingLexicalInfo doesn't take any arguments");

			macro.Body.Annotate(QuasiquoteExpander.PreserveLexicalInfoAnnotation);
			return macro.Body;
		}
	}

	public class QuasiquoteExpander : DepthFirstTransformer
	{
		public static object PreserveLexicalInfoAnnotation = new object();
		private bool _preserveLexicalInfo;

		public override void OnBlock(Block node)
		{
			var previousPreserveLexicalInfo = _preserveLexicalInfo;
			_preserveLexicalInfo = previousPreserveLexicalInfo || node.ContainsAnnotation(PreserveLexicalInfoAnnotation);
			base.OnBlock(node);
			_preserveLexicalInfo = previousPreserveLexicalInfo;
		}

		override public void OnQuasiquoteExpression(QuasiquoteExpression node)
		{
			var serializer = new CodeSerializer(_preserveLexicalInfo);
			ReplaceCurrentNode(serializer.Serialize(node));
		}
	}

	public partial class CodeSerializer : FastDepthFirstVisitor
	{
		public static string LiftName(string value)
		{
			return value;
		}
		
		public static string LiftName(ReferenceExpression node)
		{
			return node.Name;
		}

		private readonly bool _preserveLexicalInfo;
		private readonly Stack<Expression> _stack = new Stack<Expression>();
		private int _quasiquoteDepth;

		public CodeSerializer() : this(false)
		{	
		}

		public CodeSerializer(bool preserveLexicalInfo)
		{
			_preserveLexicalInfo = preserveLexicalInfo;
		}

		public Expression Serialize(QuasiquoteExpression node)
		{
			_quasiquoteDepth = 0;
			Expression e = Serialize(node.Node);
			if (_stack.Count != 0)
				throw new InvalidOperationException();
			return e;
		}

		public Expression Serialize(Node node)
		{
			if (null == node) return new NullLiteralExpression();
			node.Accept(this);
			return _stack.Pop();
		}

		public Expression Serialize(LexicalInfo location)
		{
			if (_preserveLexicalInfo && location.IsValid)
				return SerializeLexicalInfo(location);
			return CreateReference(location, location.GetType().FullName + ".Empty");
		}

		private Expression SerializeLexicalInfo(LexicalInfo location)
		{
			var ctorInvocation = CreateInvocation(location, location.GetType().FullName);
			ctorInvocation.Arguments.Add(Serialize(location.FileName));
			ctorInvocation.Arguments.Add(Serialize(location.Line));
			ctorInvocation.Arguments.Add(Serialize(location.Column));
			return ctorInvocation;
		}

		public Expression CreateReference(Node sourceNode, string qname)
		{
			return CreateReference(sourceNode.LexicalInfo, qname);
		}

		private Expression CreateReference(LexicalInfo lexicalInfo, string qname)
		{
			return AstUtil.CreateReferenceExpression(lexicalInfo, qname);
		}

		public Expression CreateReference(string qname)
		{
			return AstUtil.CreateReferenceExpression(qname);
		}

		public bool ShouldSerialize<T>(NodeCollection<T> c) where T: Node
		{
			return c.Count > 0;
		}

		public bool ShouldSerialize(object value)
		{
			return value != null;
		}

		public Expression Serialize(string value)
		{
			return new StringLiteralExpression(value);
		}

		public Expression Serialize(bool value)
		{
			return new BoolLiteralExpression(value);
		}

		public Expression Serialize(long value)
		{
			return new IntegerLiteralExpression(value);
		}

		public Expression Serialize(double value)
		{
			return new DoubleLiteralExpression(value);
		}

		public Expression Serialize(TimeSpan value)
		{
			return new TimeSpanLiteralExpression(value);
		}

		private Expression SerializeEnum(string enumType, long value)
		{	
			return new CastExpression(
				Serialize(value),
				new SimpleTypeReference("Boo.Lang.Compiler.Ast." + enumType));
		}
		
		protected Expression SerializeCollection(Node sourceNode, string typeName, StatementCollection nodes)
		{
			return SerializeCollectionWith((item) => LiftStatement(Serialize(item)), sourceNode, typeName, nodes);
		}

		protected Expression SerializeCollection(Node sourceNode, string typeName, ParameterDeclarationCollection nodes)
		{
			var result = SerializeCollectionWith(Serialize, sourceNode, typeName, nodes);
			result.Arguments.Insert(0, new BoolLiteralExpression(nodes.HasParamArray));
			return result;
		}

		protected Expression SerializeCollection(Node sourceNode, string typeName, IEnumerable nodes)
		{
			return SerializeCollectionWith(Serialize, sourceNode, typeName, nodes);
		}

		private MethodInvocationExpression SerializeCollectionWith(Func<Node, Expression> serialize, Node sourceNode, string typeName, IEnumerable nodes)
		{
			var result = CreateFromArrayInvocation(sourceNode, typeName);
			foreach (Node node in nodes)
				result.Arguments.Add(serialize(node));
			return result;
		}

		private MethodInvocationExpression CreateFromArrayInvocation(Node sourceNode, string typeName)
		{
			return CreateInvocation(sourceNode, typeName + ".FromArray");
		}

		public override void OnExpressionStatement(ExpressionStatement node)
		{
			Visit(node.Expression);
			if (null != node.Modifier)
			{
				Visit(node.Modifier);
				MethodInvocationExpression ctor = CreateInvocation(node, "Boo.Lang.Compiler.Ast.ExpressionStatement");
				ctor.NamedArguments.Add(Pair("Modifier", Pop()));
				ctor.NamedArguments.Add(Pair("Expression", Pop()));
				Push(ctor);
			}
		}

		public override void OnOmittedExpression(OmittedExpression node)
		{
			Push(CreateReference(node, "Boo.Lang.Compiler.Ast.OmittedExpression.Default"));
		}

		override public void OnQuasiquoteExpression(Boo.Lang.Compiler.Ast.QuasiquoteExpression node)
		{
			++_quasiquoteDepth;

			var mie = new MethodInvocationExpression(
					node.LexicalInfo,
					CreateReference(node, "Boo.Lang.Compiler.Ast.QuasiquoteExpression"));
			if (ShouldSerialize(node.Node))
			{
				mie.NamedArguments.Add(
					new ExpressionPair(
						CreateReference(node, "Node"),
						Serialize(node.Node)));
			}
			Push(mie);

			--_quasiquoteDepth;
		}
		
		public override void OnSpliceMemberReferenceExpression(SpliceMemberReferenceExpression node)
		{
			if (InsideSerializedQuasiquote())
			{
				SerializeSpliceMemberReferenceExpression(node);
				return;
			}
			var ctor = CreateInvocation(node, "Boo.Lang.Compiler.Ast.MemberReferenceExpression");
			ctor.Arguments.Add(Serialize(node.Target));
			ctor.Arguments.Add(LiftMemberName(node.NameExpression));
			Push(ctor);
		}

		private bool InsideSerializedQuasiquote()
		{
			return _quasiquoteDepth > 0;
		}

		public override void OnSpliceTypeDefinitionBody(SpliceTypeDefinitionBody node)
		{
			if (InsideSerializedQuasiquote())
			{
				SerializeSpliceTypeDefinitionBody(node);
				return;
			}
			if (node.ParentNode is EnumDefinition)
				Push(LiftEnumMember(node.Expression));
			else
				Push(LiftTypeMember(node.Expression));
		}

		public override void OnSpliceTypeMember(SpliceTypeMember node)
		{
			if (InsideSerializedQuasiquote())
			{
				SerializeSpliceTypeMember(node);
				return;
			}
			var ctor = (MethodInvocationExpression)Serialize(node.TypeMember);
			SpliceName(ctor, node.NameExpression);
			Push(ctor);
		}

		private void SpliceName(MethodInvocationExpression ctor, Expression nameExpression)
		{
			ctor.NamedArguments.Add(Pair("Name", LiftMemberName(nameExpression)));
		}
		
		private ExpressionPair Pair(string name, Expression e)
		{
			return new ExpressionPair(
					new ReferenceExpression(e.LexicalInfo, name),
					e);
		}

		public override void OnSpliceParameterDeclaration(SpliceParameterDeclaration node)
		{
			if (InsideSerializedQuasiquote())
			{
				SerializeSpliceParameterDeclaration(node);
				return;
			}
			MethodInvocationExpression ctor = (MethodInvocationExpression)Serialize(node.ParameterDeclaration);
			SpliceName(ctor, node.NameExpression);
			Push(ctor);
		}
		
		public override void OnSpliceTypeReference(SpliceTypeReference node)
		{
			if (InsideSerializedQuasiquote())
			{
				SerializeSpliceTypeReference(node);
				return;
			}
			Push(LiftTypeReference(node.Expression));
		}

		public override void OnSpliceExpression(SpliceExpression node)
		{
			if (InsideSerializedQuasiquote())
			{
				SerializeSpliceExpression(node);
				return;
			}
			
			if (IsStatementExpression(node))
			{
				Push(LiftStatement(node.Expression));
				return;
			}

			Push(LiftExpression(node.Expression));
		}
		
		private MethodInvocationExpression LiftMemberName(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.CodeSerializer.LiftName", node);
		}

		private MethodInvocationExpression LiftStatement(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.Statement.Lift", node);
		}

		private MethodInvocationExpression LiftExpression(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.Expression.Lift", node);
		}
		
		private MethodInvocationExpression LiftTypeReference(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.TypeReference.Lift", node);
		}

		private MethodInvocationExpression LiftTypeMember(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.TypeMember.Lift", node);
		}

		private MethodInvocationExpression LiftEnumMember(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.EnumMember.Lift", node);
		}

		private MethodInvocationExpression Lift(string methodName, Expression node)
		{
			MethodInvocationExpression lift = CreateInvocation(node, methodName);
			lift.Arguments.Add(new QuasiquoteExpander().Visit(node));
			return lift;
		}

		private MethodInvocationExpression CreateInvocation(Node sourceNode, string reference)
		{
			return CreateInvocation(sourceNode.LexicalInfo, reference);
		}

		private MethodInvocationExpression CreateInvocation(LexicalInfo lexicalInfo, string reference)
		{
			return new MethodInvocationExpression(lexicalInfo, CreateReference(lexicalInfo, reference));
		}

		private static bool IsStatementExpression(SpliceExpression node)
		{	
			if (node.ParentNode == null) return false;
			return node.ParentNode.NodeType == NodeType.ExpressionStatement;
		}

		private void Push(Expression node)
		{
			_stack.Push(node);
		}

		private Expression Pop()
		{
			return _stack.Pop();
		}
	}
}
