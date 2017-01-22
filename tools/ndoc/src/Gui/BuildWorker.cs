using System;
using System.Diagnostics;
using System.Threading;

using NDoc.Core;

namespace NDoc.Gui
{
	/// <summary>
	/// Class that manages the build thread and status notification
	/// </summary>
	public class BuildWorker
	{
		private IDocumenter m_documenter;
		private Project     m_project;

		private Thread m_workerThread;
		private IBuildStatus m_buildStatus;

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="status">A status sink for build notifications</param>
		public BuildWorker( IBuildStatus status )
		{
			Debug.Assert( status != null );

			m_buildStatus = status;
		}

		/// <summary>
		/// Return true if the build thread is active
		/// </summary>
		public bool IsBuilding
		{
			get
			{
				lock ( this )
					return m_workerThread != null && m_workerThread.IsAlive;
			}
		}

		/// <summary>
		/// Builds the project using the specified <see cref="IDocumenter"/>
		/// </summary>
		/// <param name="documenter">The <see cref="IDocumenter"/> to use</param>
		/// <param name="project">The <see cref="Project"/> to build</param>
		public void Build( IDocumenter documenter, Project project )
		{
			Debug.Assert( documenter != null );
			Debug.Assert( project != null );

			Debug.Assert( m_workerThread == null );

			m_documenter = documenter;
			m_project = project;

			m_workerThread = new Thread( new ThreadStart( ThreadProc ) );
			m_workerThread.Name = "Build";
			m_workerThread.IsBackground = true;
			m_workerThread.Priority = ThreadPriority.Normal;

			m_workerThread.Start();
		}

		/// <summary>
		/// Cancels the build
		/// </summary>
		public void Cancel()
		{
			if ( IsBuilding )
			{
				m_workerThread.Abort();
				m_workerThread.Join( 1000 );
				m_workerThread = null;
			}
		}

		private void ThreadProc()
		{
			GC.Collect();
			Debug.WriteLine("Memory before build: " + GC.GetTotalMemory(false).ToString());

			try
			{
				m_documenter.DocBuildingStep += new DocBuildingEventHandler(m_documenter_DocBuildingStep);
				// Build the documentation.
				m_documenter.Build(m_project);
			}
			catch ( Exception ex )
			{
				if ( App.GetInnermostException( ex ) is ThreadAbortException )
					m_buildStatus.BuildCancelled();
				else
					m_buildStatus.BuildException( ex );
			}
			finally
			{
				m_documenter.DocBuildingStep -= new DocBuildingEventHandler(m_documenter_DocBuildingStep);

				m_buildStatus.BuildComplete();

				m_project = null;
				m_documenter = null;
				lock( this )
					m_workerThread = null;

				GC.Collect();
				Debug.WriteLine("Memory after build: " + GC.GetTotalMemory(false).ToString());
			}
		}

		private void m_documenter_DocBuildingStep(object sender, ProgressArgs e)
		{
			m_buildStatus.ReportProgress( e );
		}
	}
}
