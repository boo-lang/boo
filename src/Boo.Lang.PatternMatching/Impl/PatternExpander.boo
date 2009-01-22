namespace Boo.Lang.PatternMatching.Impl

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

# import the pre compiled version of the match macro
import Boo.Lang.PatternMatching from Boo.Lang.PatternMatching

class PatternExpander:
	
	def Expand(matchValue as Expression, pattern as Expression) as Expression:
		match pattern:
			case MethodInvocationExpression():
				return ExpandObjectPattern(matchValue, pattern)
				
			case MemberReferenceExpression():
				return ExpandValuePattern(matchValue, pattern)
				
			case ReferenceExpression():
				return ExpandBindPattern(matchValue, pattern)
				
			case QuasiquoteExpression():
				return ExpandQuasiquotePattern(matchValue, pattern)
				
			case [| $l = $r |]:
				return ExpandCapturePattern(matchValue, pattern)
				
			case [| $l | $r |]:
				return ExpandEitherPattern(matchValue, pattern)
				
			case ArrayLiteralExpression():
				return ExpandFixedSizePattern(matchValue, pattern)
				
			otherwise:
				return ExpandValuePattern(matchValue, pattern)
		
	def ExpandEitherPattern(matchValue as Expression, node as BinaryExpression) as Expression:
		l = Expand(matchValue, node.Left)
		r = Expand(matchValue, node.Right)
		return [| $l or $r |]
		
	def ExpandBindPattern(matchValue as Expression, node as ReferenceExpression):
		return [| __eval__($node = $matchValue, true) |]
		
	def ExpandValuePattern(matchValue as Expression, node as Expression):
		return [| $matchValue == $node |]
		
	def ExpandCapturePattern(matchValue as Expression, node as BinaryExpression):
		return ExpandObjectPattern(matchValue, node.Left, node.Right)
		
	def ExpandObjectPattern(matchValue as Expression, node as MethodInvocationExpression) as Expression:
	
		if len(node.NamedArguments) == 0 and len(node.Arguments) == 0:
			return [| $matchValue isa $(TypeRefFrom(node)) |]
			 
		return ExpandObjectPattern(matchValue, NewTemp(node), node)
		
	def ExpandObjectPattern(matchValue as Expression, temp as ReferenceExpression, node as MethodInvocationExpression) as Expression:
		
		condition = [| ($matchValue isa $(TypeRefFrom(node))) and __eval__($temp = cast($(TypeRefFrom(node)), $matchValue), true) |]
		condition.LexicalInfo = node.LexicalInfo
		
		for member in node.Arguments:
			assert member isa ReferenceExpression, "Invalid argument '${member}' in pattern '${node}'."
			memberRef = MemberReferenceExpression(member.LexicalInfo, temp.CloneNode(), member.ToString())
			condition = [| $condition and __eval__($member = $memberRef, true) |]  
			
		for member in node.NamedArguments:
			namedArgCondition = ExpandMemberPattern(temp.CloneNode(), member)
			condition = [| $condition and $namedArgCondition |]
			
		return condition
	
	class QuasiquotePatternBuilder(DepthFirstVisitor):
		
		static final Ast = [| Boo.Lang.Compiler.Ast |]
		
		_parent as PatternExpander
		_pattern as Expression
		
		def constructor(parent as PatternExpander):
			_parent = parent
		
		def Build(node as QuasiquoteExpression):
			return Expand(node.Node)
			
		def Expand(node as Node):
			node.Accept(self)
			expansion = _pattern
			_pattern = null
			assert expansion is not null, "Unsupported pattern '${node}'"
			return expansion
			
		def Push(srcNode as Node, e as Expression):
			assert _pattern is null
			e.LexicalInfo = srcNode.LexicalInfo
			_pattern = e
			
		override def OnSpliceExpression(node as SpliceExpression):
			_pattern = node.Expression
			
		override def OnSpliceTypeReference(node as SpliceTypeReference):
			_pattern = node.Expression
			
		def ExpandFixedSize(items):
			a = [| (,) |]
			for item in items:
				a.Items.Add(Expand(item))
			return a
			
		override def OnOmittedExpression(node as OmittedExpression):
			_pattern = [| $Ast.OmittedExpression.Instance |]
			
		override def OnSlice(node as Slice):
			ctor = [| $Ast.Slice() |]
			ExpandProperty ctor, "Begin", node.Begin
			ExpandProperty ctor, "End", node.End
			ExpandProperty ctor, "Step", node.Step
			Push node, ctor
			
		def ExpandProperty(ctor as MethodInvocationExpression, name as string, value as Expression):
			if value is null: return
			ctor.NamedArguments.Add(ExpressionPair(First: ReferenceExpression(name), Second: Expand(value)))
			
		override def OnMacroStatement(node as MacroStatement):
			if len(node.Arguments) > 0:
				Push node, [| $Ast.MacroStatement(Name: $(node.Name), Arguments: $(ExpandFixedSize(node.Arguments))) |]
			else:
				Push node, [| $Ast.MacroStatement(Name: $(node.Name)) |]
			
		override def OnSlicingExpression(node as SlicingExpression):
			Push node, [| $Ast.SlicingExpression(Target: $(Expand(node.Target)), Indices: $(ExpandFixedSize(node.Indices))) |]
			
		override def OnTryCastExpression(node as TryCastExpression):
			Push node, [| $Ast.TryCastExpression(Target: $(Expand(node.Target)), Type: $(Expand(node.Type))) |]
			
		override def OnMethodInvocationExpression(node as MethodInvocationExpression):
			if len(node.Arguments) > 0:
				pattern = [| $Ast.MethodInvocationExpression(Target: $(Expand(node.Target)), Arguments: $(ExpandFixedSize(node.Arguments))) |]
			else:
				pattern = [| $Ast.MethodInvocationExpression(Target: $(Expand(node.Target))) |]
			Push node, pattern
			
		override def OnBoolLiteralExpression(node as BoolLiteralExpression):
			Push node, [| $Ast.BoolLiteralExpression(Value: $node) |]
			
		override def OnNullLiteralExpression(node as NullLiteralExpression):
			Push node, [| $Ast.NullLiteralExpression() |]
			
		override def OnUnaryExpression(node as UnaryExpression):
			Push node, [| $Ast.UnaryExpression(Operator: UnaryOperatorType.$(node.Operator.ToString()), Operand: $(Expand(node.Operand))) |]
			
		override def OnBinaryExpression(node as BinaryExpression):
			Push node, [| $Ast.BinaryExpression(Operator: BinaryOperatorType.$(node.Operator.ToString()), Left: $(Expand(node.Left)), Right: $(Expand(node.Right))) |]
		
		override def OnReferenceExpression(node as ReferenceExpression):
			Push node, [| $Ast.ReferenceExpression(Name: $(node.Name)) |]
			
		override def OnSuperLiteralExpression(node as SuperLiteralExpression):
			Push node, [| $Ast.SuperLiteralExpression() |]
			
	def ObjectPatternFor(node as QuasiquoteExpression):
		return QuasiquotePatternBuilder(self).Build(node)
		
	def ExpandQuasiquotePattern(matchValue as Expression, node as QuasiquoteExpression) as Expression:
		return ExpandObjectPattern(matchValue, ObjectPatternFor(node))
		
	def ExpandMemberPattern(matchValue as Expression, member as ExpressionPair):
		memberRef = MemberReferenceExpression(member.First.LexicalInfo, matchValue, member.First.ToString())	
		return Expand(memberRef, member.Second)
		
	def ExpandFixedSizePattern(matchValue as Expression, pattern as ArrayLiteralExpression):
		condition = [| $(len(pattern.Items)) == len($matchValue) |]
		i = 0
		for item in pattern.Items:
			itemValue = [| $matchValue[$i] |]
			itemPattern = Expand(itemValue, item)
			condition = [| $condition and $itemPattern |]
			++i
		return condition
		
	def TypeRefFrom(node as MethodInvocationExpression):
		return node.Target
		
internal def NewTemp(e as Expression):
	return ReferenceExpression(
			LexicalInfo: e.LexicalInfo,
			Name: "$match${CompilerContext.Current.AllocIndex()}")
