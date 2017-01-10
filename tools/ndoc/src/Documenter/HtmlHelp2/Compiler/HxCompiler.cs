using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using MSHelpCompiler;

namespace NDoc.Documenter.HtmlHelp2.Compiler
{
	/// <summary>
	/// Summary description for HxCompiler.
	/// </summary>
	public class HxCompiler
	{
		/// <summary>
		/// Creates new instnace of an HxCompiler
		/// </summary>
		public HxCompiler()
		{
		}

		/// <summary>
		/// Compiles the Html Help 2 project
		/// </summary>
		/// <param name="projectRoot">The root locaion of the project inputs</param>
		/// <param name="helpName">The name of the project</param>
		/// <param name="langID">The language ID of the final help file</param>
		public void Compile( DirectoryInfo projectRoot, string helpName, short langID )
		{
			string ProjectPathAndName = Path.Combine( projectRoot.FullName, helpName );

			using( CompilerStatus status = new CompilerStatus( ProjectPathAndName + ".HxComp.log" ) )
			{ 
				int cookie = -1;
				HxCompClass compiler = new HxCompClass();

				compiler.Initialize();
				compiler.LangId = langID;

				cookie = compiler.AdviseCompilerMessageCallback( status );

				compiler.Compile( ProjectPathAndName + ".HxC", projectRoot.FullName, helpName + ".HxS", 0 );
			
				if ( cookie != -1 )
					compiler.UnadviseCompilerMessageCallback( cookie );

				if ( status.CompileFailed )
					throw new Exception( status.ErrorMessage );
			}

		}

		#region Nested CompilerStatus class
		/// <summary>
		/// Captures compilsation status from the HXComp process
		/// </summary>
		private class CompilerStatus : IHxCompError, IDisposable
		{
			private bool _compileFailed = false;

			private string _errorMsg = String.Empty;

			private StreamWriter _logFile = null;

			/// <summary>
			/// Creates a new instance of CompilerStatus
			/// </summary>
			public CompilerStatus( string logName ) 
			{
				_logFile = new StreamWriter( File.Create( logName ) );
			}

			~CompilerStatus()
			{
				Dispose();
			}

			public void Dispose()
			{
				_logFile.Close();
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Indicates whether a fatal compilation error has occured
			/// </summary>
			public bool CompileFailed
			{
				get
				{
					return _compileFailed;
				}
			}

			/// <summary>
			/// If the compile failed this holds whatever error message was return by HxComp
			/// </summary>
			public string ErrorMessage
			{
				get
				{
					return _errorMsg;
				}
			}

			#region IHxCompError Interface
			HxCompStatus IHxCompError.QueryStatus()
			{
				//_compileFailed is only ever true if we get a fatal error message in report error
				if ( _compileFailed )
					return HxCompStatus.HxCompStatus_Cancel;
				else
					return HxCompStatus.HxCompStatus_Continue;
			}

			void IHxCompError.ReportError( String TaskItemString, String Filename, Int32 nLineNum, Int32 nCharNum, HxCompErrorSeverity Severity, String DescriptionString )
			{
				StringBuilder msg = new StringBuilder( "HxComp error\n" );

				msg.AppendFormat( "\tTaskItemString={0}\n", TaskItemString );
				msg.AppendFormat( "\tFilename={0}\n", Filename );
				msg.AppendFormat( "\tLineNum={0}\n", nLineNum );
				msg.AppendFormat( "\tCharNum={0}\n", nCharNum );
				msg.AppendFormat( "\tSeverity={0}\n", Severity );
				msg.AppendFormat( "\tDescriptionString={0}\n", DescriptionString );

				String sMsg = msg.ToString();
				Trace.WriteLine( sMsg );
				_logFile.WriteLine( sMsg );

				if ( Severity == HxCompErrorSeverity.HxCompErrorSeverity_Fatal )
				{
					_compileFailed = true;
					_errorMsg = sMsg;
				}

			}

			void IHxCompError.ReportMessage( HxCompErrorSeverity Severity , String DescriptionString )
			{
				Trace.WriteLine( String.Format( "HxComp [{0}]: {1}", Severity, DescriptionString ) );
				_logFile.WriteLine( String.Format( "{0}: {1}", Severity, DescriptionString ) );

				//If enough non-fatal errors accumulate (more than half of the topics
				//HxComp will abort, reporting the error here not in ReportError
				if ( Severity == HxCompErrorSeverity.HxCompErrorSeverity_Fatal )
				{
					_compileFailed = true;
					_errorMsg = DescriptionString;
				}

			}
			#endregion

		}
		#endregion
	}
}
