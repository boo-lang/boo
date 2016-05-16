// Documenter.cs - base XML documenter code
// Copyright (C) 2001  Kral Ferch, Jason Diamond
// Parts Copyright (C) 2004  Kevin Downs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace NDoc.Core
{
	/// <summary>Provides an abstract base class for documenters.</summary>
	/// <remarks>
	/// This is an <see langword="abstract"/> base class for NDoc Documenters.
	/// It provides default implementations of all the methods required by the <see cref="IDocumenter"/> interface. 
	/// It also provides some basic properties which are shared by all documenters. 
	/// </remarks>
	abstract public class BaseDocumenter : IDocumenter
	{
		IDocumenterConfig		config;

		/// <summary>Initializes a new instance of the <see cref="BaseDocumenter"/> class.</summary>
		/// <param name="config_">settings</param>
		protected BaseDocumenter( IDocumenterConfig config_ )
		{
			Debug.Assert( config_ != null );
			config = config_;
		}

		/// <summary>See <see cref="IDocumenter.Config">IDocumenter.Config</see>.</summary>
		public IDocumenterConfig Config
		{
			get { return config; }
		}

		/// <summary>See <see cref="IDocumenter.MainOutputFile">IDocumenter.MainOutputFile</see>.</summary>
		public abstract string MainOutputFile { get; }

		/// <summary>See <see cref="IDocumenter.View">IDocumenter.View</see>.</summary>
		public virtual void View()
		{
			if (File.Exists(this.MainOutputFile))
			{
				Process.Start(this.MainOutputFile);
			}
			else
			{
				throw new FileNotFoundException("Documentation not built.",
					this.MainOutputFile);
			}
		}

		/// <summary>See <see cref="IDocumenter.DocBuildingStep">IDocumenter.DocBuildingStep</see>.</summary>
		public event DocBuildingEventHandler DocBuildingStep;
		/// <summary>See <see cref="IDocumenter.DocBuildingProgress">IDocumenter.DocBuildingProgress</see>.</summary>
		public event DocBuildingEventHandler DocBuildingProgress;

		/// <summary>Raises the <see cref="DocBuildingStep"/> event.</summary>
		/// <param name="step">The overall percent complete value.</param>
		/// <param name="label">A description of the work currently beeing done.</param>
		protected void OnDocBuildingStep(int step, string label)
		{
			if (DocBuildingStep != null)
				DocBuildingStep(this, new ProgressArgs(step, label));
		}

		/// <summary>Raises the <see cref="DocBuildingProgress"/> event.</summary>
		/// <param name="progress">Percentage progress value</param>
		protected void OnDocBuildingProgress(int progress)
		{
			if (DocBuildingProgress != null)
				DocBuildingProgress(this, new ProgressArgs(progress, ""));
		}

		/// <summary>See <see cref="IDocumenter.CanBuild(Project)">IDocumenter.CanBuild</see>.</summary>
		public virtual string CanBuild(Project project)
		{
			return this.CanBuild(project, false);
		}

        /// <summary>See <see cref="IDocumenter.CanBuild(Project, bool)">CanBuild</see>.</summary>
		public virtual string CanBuild(Project project, bool checkInputOnly)
		{
			StringBuilder xfiles = new StringBuilder();
			foreach (AssemblySlashDoc asd in project.AssemblySlashDocs)
			{
				if (!File.Exists(asd.Assembly.Path))
				{
					xfiles.Append("\n" + asd.Assembly.Path);
				}
				if (asd.SlashDoc.Path.Length>0)
				{
					if (!File.Exists(asd.SlashDoc.Path))
					{
						xfiles.Append("\n" + asd.SlashDoc.Path);
					}
				}
			}

			if (xfiles.Length > 0)
			{
				return "One of more source files not found:\n" + xfiles.ToString();
			}
			else
			{
				return null;
			}
		}

		/// <summary>See <see cref="IDocumenter.Build">IDocumenter.Build</see>.</summary>
		abstract public void Build(Project project);
	}
}
