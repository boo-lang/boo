using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
	public class GotoOnTopLevelContinue : DepthFirstTransformer
	{
		LabelStatement _label;

		int _usage;

		public GotoOnTopLevelContinue(LabelStatement label)
		{
			_label = label;
		}
		
		public int UsageCount
		{
			get { return _usage;  }
		}

		public override void OnContinueStatement(ContinueStatement node)
		{
			ReplaceCurrentNode(NewGoto(node));
			++_usage;
		}

		public override void OnWhileStatement(WhileStatement node)
		{	
		}

		public override void OnForStatement(ForStatement node)
		{
		}

		public override void OnCallableBlockExpression(CallableBlockExpression node)
		{	
		}

		public GotoStatement NewGoto(Node sourceNode)
		{
			ReferenceExpression reference = new ReferenceExpression(sourceNode.LexicalInfo, _label.Name);
			reference.Entity = _label.Entity;
			return new GotoStatement(sourceNode.LexicalInfo, reference);
		}
	}
}
