using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public interface IAccessibilityChecker
	{
		bool IsAccessible(IAccessibleMember member);
	}
	
	public class AccessibilityChecker : IAccessibilityChecker
	{
		public static readonly IAccessibilityChecker Global = new GlobalAccessibilityChecker();
		
		private readonly TypeDefinition _scope;

		public AccessibilityChecker(TypeDefinition scope)
		{
			_scope = scope;
		}

		public bool IsAccessible(IAccessibleMember member)
		{
			if (member.IsPublic) return true;

			IType declaringType = member.DeclaringType;
			if (declaringType == CurrentType()) return true;
			if (member.IsInternal && member is IInternalEntity) return true;
			if (member.IsProtected && CurrentType().IsSubclassOf(declaringType)) return true;

			return IsDeclaredInside(declaringType);
		}

		private IType CurrentType()
		{
			return (IType)_scope.Entity;
		}

		private bool IsDeclaredInside(IType candidate)
		{
			IInternalEntity entity = candidate as IInternalEntity;
			if (null == entity) return false;

			TypeDefinition type = _scope.DeclaringType;
			while (type != null)
			{
				if (type == entity.Node) return true;
				type = type.DeclaringType;
			}
			return false;
		}
		
		public class GlobalAccessibilityChecker : IAccessibilityChecker
		{	
			public bool IsAccessible(IAccessibleMember member)
			{
				if (member.IsPublic) return true;
				return member.IsInternal && member is IInternalEntity;
			}
		}
	}
}
