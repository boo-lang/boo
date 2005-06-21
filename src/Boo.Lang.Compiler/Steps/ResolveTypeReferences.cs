namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ResolveTypeReferences : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		public ResolveTypeReferences()
		{
		}

		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}

		override public void OnArrayTypeReference(ArrayTypeReference node)
		{
			NameResolutionService.ResolveArrayTypeReference(node);
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			NameResolutionService.ResolveSimpleTypeReference(node);
			if (node.Entity is InternalCallableType)
			{
				//EnsureRelatedNodeWasVisited(node.Entity);
			}
		}
		
		override public void LeaveCallableTypeReference(CallableTypeReference node)
		{
			IParameter[] parameters = new IParameter[node.Parameters.Count];
			for (int i=0; i<parameters.Length; ++i)
			{
				parameters[i] = new SimpleParameter("arg" + i, GetType(node.Parameters[i]));
			}
			
			IType returnType = null;
			if (null != node.ReturnType)
			{
				returnType = GetType(node.ReturnType);
			}
			else
			{
				returnType = TypeSystemServices.VoidType;
			}
			
			node.Entity = TypeSystemServices.GetConcreteCallableType(node, new CallableSignature(parameters, returnType));
		}

	}
}
