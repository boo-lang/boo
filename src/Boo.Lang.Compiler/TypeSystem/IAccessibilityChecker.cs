using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public interface IAccessibilityChecker
	{
		bool IsAccessible(IAccessibleMember member);
	}
}