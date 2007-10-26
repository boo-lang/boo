namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IExtensionEnabled : IEntityWithParameters
	{
		bool IsExtension { get; }
	}
}