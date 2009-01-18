namespace Boo.Lang.PatternMatching.Impl

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class PatternExpander:
	
	def expand(matchValue as Expression, pattern as Expression) as Expression:
		mie = pattern as MethodInvocationExpression
		if mie is not null:
			return expandObjectPattern(matchValue, mie)
			
		memberRef = pattern as MemberReferenceExpression
		if memberRef is not null:
			return expandValuePattern(matchValue, memberRef)
			
		reference = pattern as ReferenceExpression
		if reference is not null:
			return expandBindPattern(matchValue, reference)
			
		quasiquote = pattern as QuasiquoteExpression
		if quasiquote is not null:
			return expandQuasiquotePattern(matchValue, quasiquote)
			
		binary = pattern as BinaryExpression
		if binary is not null:
			return expandBinaryExpression(matchValue, binary)
			
		fixedSize = pattern as ArrayLiteralExpression
		if fixedSize is not null:
			return expandFixedSizePattern(matchValue, fixedSize)
			
		return expandValuePattern(matchValue, pattern)
		
	def expandBinaryExpression(matchValue as Expression, node as BinaryExpression):
		if isCapture(node):
			return expandCapturePattern(matchValue, node)
			
		if node.Operator == BinaryOperatorType.BitwiseOr:
			return expandEitherPattern(matchValue, node)
			
		assert false, "Unsupported pattern: ${node}"
		
	def expandEitherPattern(matchValue as Expression, node as BinaryExpression) as Expression:
		l = expand(matchValue, node.Left)
		r = expand(matchValue, node.Right)
		return [| $l or $r |]
		
	def isCapture(node as BinaryExpression):
		if node.Operator != BinaryOperatorType.Assign: return false
		return node.Left isa ReferenceExpression and node.Right isa MethodInvocationExpression
		
	def expandBindPattern(matchValue as Expression, node as ReferenceExpression):
		return [| __eval__($node = $matchValue, true) |]
		
	def expandValuePattern(matchValue as Expression, node as Expression):
		return [| $matchValue == $node |]
		
	def expandCapturePattern(matchValue as Expression, node as BinaryExpression):
		return expandObjectPattern(matchValue, node.Left, node.Right)
		
	def expandObjectPattern(matchValue as Expression, node as MethodInvocationExpression) as Expression:
	
		if len(node.NamedArguments) == 0 and len(node.Arguments) == 0:
			return [| $matchValue isa $(typeRef(node)) |]
			 
		return expandObjectPattern(matchValue, newTemp(node), node)
		
	def expandObjectPattern(matchValue as Expression, temp as ReferenceExpression, node as MethodInvocationExpression) as Expression:
		
		condition = [| ($matchValue isa $(typeRef(node))) and __eval__($temp = cast($(typeRef(node)), $matchValue), true) |]
		condition.LexicalInfo = node.LexicalInfo
		
		for member in node.Arguments:
			assert member isa ReferenceExpression, "Invalid argument '${member}' in pattern '${node}'."
			memberRef = MemberReferenceExpression(member.LexicalInfo, temp.CloneNode(), member.ToString())
			condition = [| $condition and __eval__($member = $memberRef, true) |]  
			
		for member in node.NamedArguments:
			namedArgCondition = expandMemberPattern(temp.CloneNode(), member)
			condition = [| $condition and $namedArgCondition |]
			
		return condition
	
	class QuasiquotePatternBuilder(DepthFirstVisitor):
		
		_parent as PatternExpander
		_pattern as Expression
		
		def constructor(parent as PatternExpander):
			_parent = parent
		
		def build(node as QuasiquoteExpression):
			return expand(node.Node)
			
		def expand(node as Node):
			node.Accept(self)
			expansion = _pattern
			_pattern = null
			assert expansion is not null, "Unsupported pattern '${node}'"
			return expansion
			
		def push(srcNode as Node, e as Expression):
			assert _pattern is null
			e.LexicalInfo = srcNode.LexicalInfo
			_pattern = e
			
		override def OnSpliceExpression(node as SpliceExpression):
			_pattern = node.Expression
			
		override def OnSpliceTypeReference(node as SpliceTypeReference):
			_pattern = node.Expression
			
		def expandFixedSize(items):
			a = [| (,) |]
			for item in items:
				a.Items.Add(expand(item))
			return a
			
		override def OnOmittedExpression(node as OmittedExpression):
			_pattern = [| OmittedExpression.Instance |]
			
		override def OnSlice(node as Slice):
			ctor = [| Slice() |]
			expandProperty ctor, "Begin", node.Begin
			expandProperty ctor, "End", node.End
			expandProperty ctor, "Step", node.Step
			push node, ctor
			
		def expandProperty(ctor as MethodInvocationExpression, name as string, value as Expression):
			if value is null: return
			ctor.NamedArguments.Add(ExpressionPair(First: ReferenceExpression(name), Second: expand(value)))
			
		override def OnSlicingExpression(node as SlicingExpression):
			push node, [| SlicingExpression(Target: $(expand(node.Target)), Indices: $(expandFixedSize(node.Indices))) |]
			
		override def OnTryCastExpression(node as TryCastExpression):
			push node, [| TryCastExpression(Target: $(expand(node.Target)), Type: $(expand(node.Type))) |]
			
		override def OnMethodInvocationExpression(node as MethodInvocationExpression):
			if len(node.Arguments) > 0:
				pattern = [| MethodInvocationExpression(Target: $(expand(node.Target)), Arguments: $(expandFixedSize(node.Arguments))) |]
			else:
				pattern = [| MethodInvocationExpression(Target: $(expand(node.Target))) |]
			push node, pattern
			
		override def OnBoolLiteralExpression(node as BoolLiteralExpression):
			push node, [| BoolLiteralExpression(Value: $node) |]
			
		override def OnNullLiteralExpression(node as NullLiteralExpression):
			push node, [| NullLiteralExpression() |]
			
		override def OnUnaryExpression(node as UnaryExpression):
			push node, [| UnaryExpression(Operator: UnaryOperatorType.$(node.Operator.ToString()), Operand: $(expand(node.Operand))) |]
			
		override def OnBinaryExpression(node as BinaryExpression):
			push node, [| BinaryExpression(Operator: BinaryOperatorType.$(node.Operator.ToString()), Left: $(expand(node.Left)), Right: $(expand(node.Right))) |]
		
		override def OnReferenceExpression(node as ReferenceExpression):
			push node, [| ReferenceExpression(Name: $(node.Name)) |]
			
		override def OnSuperLiteralExpression(node as SuperLiteralExpression):
			push node, [| SuperLiteralExpression() |]
			
	def objectPatternFor(node as QuasiquoteExpression):
		return QuasiquotePatternBuilder(self).build(node)
		
	def expandQuasiquotePattern(matchValue as Expression, node as QuasiquoteExpression) as Expression:
		return expandObjectPattern(matchValue, objectPatternFor(node))
		
	def expandMemberPattern(matchValue as Expression, member as ExpressionPair):
		memberRef = MemberReferenceExpression(member.First.LexicalInfo, matchValue, member.First.ToString())	
		return expand(memberRef, member.Second)
		
	def expandFixedSizePattern(matchValue as Expression, pattern as ArrayLiteralExpression):
		condition = [| $(len(pattern.Items)) == len($matchValue) |]
		i = 0
		for item in pattern.Items:
			itemValue = [| $matchValue[$i] |]
			itemPattern = expand(itemValue, item)
			condition = [| $condition and $itemPattern |]
			++i
		return condition
		
	def typeRef(node as MethodInvocationExpression):
		return node.Target
		
def newTemp(e as Expression):
	return ReferenceExpression(
			LexicalInfo: e.LexicalInfo,
			Name: "$match$${CompilerContext.Current.AllocIndex()}")
