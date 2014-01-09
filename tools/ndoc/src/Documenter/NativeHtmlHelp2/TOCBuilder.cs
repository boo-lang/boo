using System;

using NDoc.Documenter.NativeHtmlHelp2.HxProject;
using NDoc.Documenter.NativeHtmlHelp2.Engine;

namespace NDoc.Documenter.NativeHtmlHelp2
{
	/// <summary>
	/// Orchestrates building the table of contents file base on HTMLFactory events
	/// </summary>
	public class TOCBuilder : IDisposable
	{
		private TOCFile _toc = null;

		private HtmlFactory _factory = null;

		/// <summary>
		/// Contruct a enw instance of the TOCBuilder class
		/// </summary>
		/// <param name="toc">The table of contents file to write to</param>
		/// <param name="factory">The HTMLFactory creating each file to be added</param>
		public TOCBuilder( TOCFile toc, HtmlFactory factory )
		{
			if ( toc == null )
				throw new NullReferenceException( "The TOCFile cannot be null" );

			if ( factory == null )
				throw new NullReferenceException( "The HtmlFactory cannot be null" );

			_toc = toc;
			_factory = factory;

			_toc.Open();

			// connect to factory events
			// this is so we can build the TOC as we go
			_factory.TopicStart += new TopicEventHandler(factory_TopicStart);
			_factory.TopicEnd += new EventHandler(factory_TopicEnd);
			_factory.AddFileToTopic += new TopicEventHandler(factory_AddFileToTopic);
		}

		private void factory_TopicStart(object sender, FileEventArgs args)
		{
			// this assumes that all content files are going in a directory named
			// "html" (relative to the location of the HxT
			_toc.OpenNode( string.Format( "/{0}/{1}", NativeHtmlHelp2Workspace.ContentLocationName, args.File ) );
		}

		private void factory_TopicEnd(object sender, EventArgs e)
		{
			_toc.CloseNode();
		}

		private void factory_AddFileToTopic(object sender, FileEventArgs args)
		{
			// this assumes that all content files are going in a directory named
			// "html" (relative to the location of the HxT
			_toc.InsertNode( string.Format( "/{0}/{1}", NativeHtmlHelp2Workspace.ContentLocationName, args.File ) );
		}

		/// <summary>
		/// Disposes the TOCBuilder instance
		/// </summary>
		public void Dispose()
		{
			if ( _factory != null )
			{
				_factory.TopicStart -= new TopicEventHandler(factory_TopicStart);
				_factory.TopicEnd -= new EventHandler(factory_TopicEnd);
				_factory.AddFileToTopic -= new TopicEventHandler(factory_AddFileToTopic);
			}

			if ( _toc != null )
				_toc.Close();
		}
	}
}
