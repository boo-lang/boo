using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NDoc.Gui
{
	/// <summary>
	/// Manages the wait cursor
	/// </summary>
	public class WaitCursor : IDisposable
	{		
		private Control m_parent;
		private Cursor m_startCursor;

		/// <summary>
		/// Constructing this class casues the cursor to be set to the wait cursor
		/// and resets it at disposal
		/// </summary>
		/// <param name="parent">The control whose cursor to set</param>
		public WaitCursor( Control parent ) : this( parent, Cursors.WaitCursor )
		{
		}

		/// <summary>
		/// Constructing this class casues the cursor to be set to the specified cursor
		/// and resets it at disposal
		/// </summary>
		/// <param name="parent">The control whose cursor to set</param>
		/// <param name="cursor">The cursor</param>
		public WaitCursor( Control parent, Cursor cursor )
		{
			Debug.Assert( parent != null );
			m_parent = parent;
			m_startCursor = m_parent.Cursor;
			m_parent.Cursor = cursor;
		}

		/// <summary>
		/// Restores the cursor to the default
		/// </summary>
		public void Dispose()
		{
			if ( m_parent.IsDisposed == false )
				m_parent.Cursor = m_startCursor;
		}
	}
}
			