
#if MONOTOUCH
namespace System.Diagnostics
{
	using System.IO;
	using System.Collections.Generic;
	
	public enum TraceLevel { Off, Error, Warning, Info, Verbose }
	
	public class TraceSwitch
	{
		public TraceSwitch(string displayName, string description) {}
		public bool TraceInfo { get { return Level >= TraceLevel.Info; } }
		public bool TraceVerbose { get { return Level >= TraceLevel.Verbose; } }
		public bool TraceWarning { get { return Level >= TraceLevel.Warning; } }
		public bool TraceError { get { return Level >= TraceLevel.Error; } }
		public TraceLevel Level { get; set; }
	}
	
	public static class Trace
	{
		public static void WriteLine(object message)
		{
			if (_listeners == null) return;
			foreach (var listener in _listeners) listener.WriteLine(message);
		}
		
		public static int IndentLevel { get; set; }
		
		public static TraceListenerCollection Listeners
		{
			get { return _listeners ?? (_listeners = new TraceListenerCollection()); }
		}
		
		private static TraceListenerCollection _listeners;
	}
	
	public class TraceListenerCollection : List<TraceListener>
	{
	}
	
	public abstract class TraceListener
	{
		public abstract void WriteLine(object message);
	}
	
	public class TextWriterTraceListener : TraceListener
	{
		readonly TextWriter _writer;
		
		public TextWriterTraceListener(TextWriter writer)
		{
			_writer = writer;
		}
		
		override public void WriteLine(object message)
		{
			_writer.WriteLine(message);
		}
	}
}
#endif
