// IDocumenter.cs - interface for all documenters
// Copyright (C) 2001  Kral Ferch, Jason Diamond
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
using System.Xml;

namespace NDoc.Core
{
	/// <summary>
	/// Custom event arguments' class used by DocBuildingEventHandler.
	/// </summary>
	public class ProgressArgs : EventArgs
	{
		private int progress;
		private string status;
		/// <summary>
		/// ProgressArgs default constructor.
		/// </summary>
		/// <param name="progress">Percentage value for a progress bar.</param>
		/// <param name="status">The label describing the current work beeing done.</param>
		public ProgressArgs(int progress, string status)
		{
			this.progress = progress;
			this.status = status;
		}
		/// <summary>
		/// Gets the percentage value.
		/// </summary>
		/// <value>A number between 0 and 100 corresponding to the percentage of work completed.</value>
		public int Progress
		{
			get {return progress;}
		}
		/// <summary>
		/// Gets the current work label.
		/// </summary>
		/// <value>A short description of the current work beeing done.</value>
		public string Status
		{
			get {return status;}
		}
	}

	/// <summary>
	/// Used by events raised by <see cref="IDocumenter"/> to notify doc building progress.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">An <c>ProgressArgs</c> that contains the event data.</param>
	public delegate void DocBuildingEventHandler(object sender, ProgressArgs e);

	/// <summary>Represents a class capable of generating documentation from a given assembly and /doc file.</summary>
	public interface IDocumenter
	{
		/// <summary>
		/// Raised to update the overall percent complete value and the current step name.
		/// </summary>
		event DocBuildingEventHandler DocBuildingStep;
		/// <summary>
		/// Raised to update the current step's percent complete value.
		/// </summary>
		event DocBuildingEventHandler DocBuildingProgress;

		/// <summary>Checks to make sure the documenter can perform a
		/// build.</summary>
		/// <param name="project">The project that would be built.</param>
		/// <param name="checkInputOnly">When true, don't check for output 
		/// file locking.</param>
		/// <remarks>This is for people who like to leave their CHMs open.</remarks>
		/// <returns>null if the documenter can build; otherwise a message
		/// describing why it can't build</returns>
		string CanBuild(Project project, bool checkInputOnly);

		/// <summary>
		/// Checks if the documentation output file(s) exist.
		/// </summary>
		/// <param name="project">The project that generated the documentation.</param>
		/// <returns>True if the documentation can be viewed.</returns>
		string CanBuild(Project project);

		/// <summary>Builds the documentation.</summary>
		/// <remarks>The compiler does not currently allow namespaces to documented.</remarks>
		void Build(Project project);

		/// <summary>
		/// Returns the documenter's main output file path.
		/// </summary>
		string MainOutputFile { get; }

		/// <summary>Spawns a new process to view the generated documentation.</summary>
		/// <exception cref="System.IO.FileNotFoundException">
		/// Thrown if the main output file does not exist.</exception>
		void View();

		/// <summary>Gets or sets the documenter's config object.</summary>
		/// <remarks>This can be put inside a PropertyGrid for editing by the user.</remarks>
		IDocumenterConfig Config { get; }
	}
}
