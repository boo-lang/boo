${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
	
	public class DepthFirstVisitor : IAstVisitor
	{
		public virtual bool Visit(Node node)
		{			
			if (null != node)
			{
				try
				{
					node.Accept(this);
					return true;
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
			return false;
		}
		
		protected virtual void OnError(Node node, Exception error)
		{
			throw Boo.Lang.Compiler.CompilerErrorFactory.InternalError(node, error);
		}
		
		public virtual void Visit(Node[] array, NodeType nodeType)
		{
			foreach (Node node in array)
			{
				if (node.NodeType == nodeType)
				{
					Visit(node);
				}
			}
		}
		
		public virtual bool Visit(NodeCollection collection, NodeType nodeType)
		{
			if (null != collection)
			{
				Visit(collection.ToArray(), nodeType);
				return true;
			}
			return false;
		}
		
		public virtual void Visit(Node[] array)
		{
			foreach (Node node in array)
			{
				Visit(node);
			}
		}
		
		public virtual bool Visit(NodeCollection collection)
		{
			if (null != collection)
			{
				Visit(collection.ToArray());
				return true;
			}
			return false;
		}
<%

for item in model.GetConcreteAstNodes():
	
	fields = model.GetVisitableFields(item)	
	if len(fields):
	
%>
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{				
			if (Enter${item.Name}(node))
			{
<%
		for field in fields:
%>				Visit(node.${field.Name});
<%
		end
%>				Leave${item.Name}(node);
			}
		}
			
		public virtual bool Enter${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
			return true;
		}
		
		public virtual void Leave${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
<%
	else:
%>
		public virtual void On${item.Name}(Boo.Lang.Compiler.Ast.${item.Name} node)
		{
		}
<%
	end
end
%>				
	}
}

