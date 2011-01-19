namespace Boo.Lang
{
#if NO_SYSTEM_CORE
	public delegate TOut Func<TOut>();
	public delegate TOut Func<T, TOut>(T arg);
	public delegate void Action();
#endif
}