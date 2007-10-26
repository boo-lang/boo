namespace Boo.Lang.Compiler.TypeSystem
{
	public interface ILocalEntity : ITypedEntity
	{
		bool IsPrivateScope
		{
			get;
		}
		
		/// <summary>
		/// Is this variable shared among closures?
		/// </summary>
		bool IsShared
		{
			get;
			set;
		}
		
		/// <summary>
		/// Is this variable ever used in the body of the method?
		/// </summary>
		bool IsUsed
		{
			get;
			set;
		}
	}
}