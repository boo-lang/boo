namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessClosures : AbstractTransformerCompilerStep
	{
		Method _currentMethod;
		
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}
		
		override public void OnMethod(Method node)
		{
			_currentMethod = node;
			Visit(node.Body);
		}
		
		override public void OnCallableBlockExpression(CallableBlockExpression node)
		{
			InternalMethod closureEntity = (InternalMethod)GetEntity(node);
			ReplaceCurrentNode(CreateClosureReference(closureEntity));
		}
		
		Expression CreateClosureReference(InternalMethod closure)
		{
			using (ForeignReferenceCollector collector = new ForeignReferenceCollector())
			{
				collector.ForeignMethod = _currentMethod;
				collector.Initialize(_context);
				collector.Visit(closure.Method.Body);
				
				if (collector.ContainsForeignLocalReferences)
				{	
					return CreateClosureClass(collector, closure);					
				}
			}
			return CodeBuilder.CreateMemberReference(closure);
		}
		
		Expression CreateClosureClass(ForeignReferenceCollector collector, InternalMethod closure)
		{
			Method method = closure.Method;
			TypeDefinition parent = method.DeclaringType;
			parent.Members.Remove(method);
			
			BooClassBuilder builder = collector.CreateSkeletonClass(method.Name);					
			builder.ClassDefinition.Members.Add(method);			
			method.Name = "Invoke";			
			parent.Members.Add(builder.ClassDefinition);	
			
			if (method.IsStatic)
			{	
				// need to adjust paremeter indexes (parameter 0 is now self)
				foreach (ParameterDeclaration parameter in method.Parameters)
				{
					((InternalParameter)parameter.Entity).Index += 1;
				}
			}
			
			method.Modifiers = TypeMemberModifiers.Public;
			
			collector.AdjustReferences();
			return CodeBuilder.CreateMemberReference(
					collector.CreateConstructorInvocationWithReferencedEntities(
							builder.Entity),
					closure);
		}
	}
}
