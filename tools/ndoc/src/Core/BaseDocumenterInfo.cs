using System;
using System.Diagnostics;

namespace NDoc.Core
{
	/// <summary>
	/// Holds meta data about a specific type of documenter
	/// </summary>
	public abstract class BaseDocumenterInfo : IDocumenterInfo, IComparable
	{
		private DocumenterDevelopmentStatus _developmentStatus = DocumenterDevelopmentStatus.Stable;
		private string _name;

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The documenter's name</param>
		protected BaseDocumenterInfo( string name ) : this( name, DocumenterDevelopmentStatus.Stable )
		{
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the documenter</param>
		/// <param name="status">The development status of the documenter</param>
		protected BaseDocumenterInfo( string name, DocumenterDevelopmentStatus status )
		{
			_name = name;
			_developmentStatus = status;
		}

		/// <summary>
		/// Creates an <see cref="IDocumenterConfig"/> object for this documenter type
		/// </summary>
		/// <param name="project">A project to associate the config with</param>
		/// <returns>A config object</returns>
		public IDocumenterConfig CreateConfig( Project project )
		{
			IDocumenterConfig config = CreateConfig();
			config.SetProject( project );

			return config;
		}

		/// <summary>
		/// Creates an <see cref="IDocumenterConfig"/> object for this documenter type
		/// </summary>
		/// <returns>A config object</returns>
		public abstract IDocumenterConfig CreateConfig();

		/// <summary>
		/// The documenter's name
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Specifies the development status (alpha, beta, stable) of a documenter.
		/// </summary>
		/// <remarks>
		/// As implemented in this class, this always returns <see cref="DocumenterDevelopmentStatus">Stable</see>.
		/// <note type="inheritinfo">Documenters should override this if they are not yet stable...</note>
		/// </remarks>
		public DocumenterDevelopmentStatus DevelopmentStatus
		{
			get
			{
				return _developmentStatus;
			}
		}

		/// <summary>Compares the currrent document to another documenter.</summary>
		public int CompareTo(object obj)
		{
			Debug.Assert( obj is IDocumenterInfo );
			return String.Compare(Name, ((IDocumenterInfo)obj).Name);
		}

		/// <summary>
		/// Override
		/// </summary>
		/// <returns>Formatted name of the documenter</returns>
		public override string ToString()
		{
			// build a development status string (alpha, beta, etc)
			string devStatus = string.Empty;
			if ( DevelopmentStatus != DocumenterDevelopmentStatus.Stable )
			{
				devStatus = DevelopmentStatus.ToString();
				// want it uncapitalized
				devStatus = string.Format( " ({0}{1})", Char.ToLower( devStatus[0] ), devStatus.Substring(1) );
			}

			return Name + devStatus;
		}
	}
}