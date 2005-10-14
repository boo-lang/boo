${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public class DepthFirstTransformer : IAstVisitor
	{
		protected Node _resultingNode = null;	
<%
	for item as TypeMember in model.GetConcreteAstNodes():
			
		visitableFields = model.GetVisitableFields(item)
		resultingNodeType = model.GetResultingTransformerNode(item)
			
%>
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{	
<%
		if len(visitableFields):
			
%>			if (Enter${item.Name}(node))
			{
<%
			for field as Field in visitableFields:
				if model.IsCollectionField(field):

%>				Visit(node.${field.Name});
<%
				else:

%>				${field.Type} current${field.Name}Value = node.${field.Name};
				if (null != current${field.Name}Value)
				{			
					${field.Type} newValue = (${field.Type})VisitNode(current${field.Name}Value);
					if (!object.ReferenceEquals(newValue, current${field.Name}Value))
					{
						node.${field.Name} = newValue;
					}
				}
<%
				end
			end
%>
				Leave${item.Name}(node);
			}
<%
		end
%>		}
<%
		
		if len(visitableFields):
		
%>
		public virtual bool Enter${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
			return true;
		}
		
		public virtual void Leave${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
<%
		end
	end
%>		
		protected virtual void RemoveCurrentNode()
		{
			_resultingNode = null;
		}
		
		protected virtual void ReplaceCurrentNode(Node replacement)
		{
			_resultingNode = replacement;
		}
		
		protected virtual void OnNode(Node node)
		{
			node.Accept(this);
		}
		
		public virtual Node VisitNode(Node node)
		{
			if (null != node)
			{
				try
				{
					Node saved = _resultingNode;
					_resultingNode = node;
					OnNode(node);
					Node result = _resultingNode;
					_resultingNode = saved;
					return result;
				}
				catch (Boo.Lang.Compiler.CompilerError)
				{
					throw;
				}
				catch (Exception error)
				{
					OnError(node, error);
				}
			}
			return null;
		}
		
		protected virtual void OnError(Node node, Exception error)
		{
			throw Boo.Lang.Compiler.CompilerErrorFactory.InternalError(node, error);
		}
		
		public Node Visit(Node node)
		{
			return VisitNode(node);
		}
		
		public Expression Visit(Expression node)
		{
			return (Expression)VisitNode(node);
		}
		
		public Statement Visit(Statement node)
		{
			return (Statement)VisitNode(node);
		}
		
		public virtual bool Visit(NodeCollection collection)
		{
			if (null != collection)
			{
				int removed = 0;
				
				Node[] nodes = collection.ToArray();
				for (int i=0; i<nodes.Length; ++i)
				{					
					Node currentNode = nodes[i];
					Node resultingNode = VisitNode(currentNode);
					if (currentNode != resultingNode)
					{
						int actualIndex = i-removed;
						if (null == resultingNode)
						{
							++removed;
							collection.RemoveAt(actualIndex);
						}
						else
						{
							collection.ReplaceAt(actualIndex, resultingNode);
						}
					}
				}
				return true;
			}
			return false;
		}
	}
}
