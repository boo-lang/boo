// MainForm.cs - main GUI interface to NDoc
// Copyright (C) 2001  Kral Ferch, Keith Hill
//
// Modified by: Keith Hill on Sep 28, 2001.
//   Tweaked the layout quite a bit. Uses new HeaderGroupBox from Matthew Adams
//   from DOTNET list.  Added to menu, added a toolbar and status bar.  Changed
//   the way docs are built on separate thread so that you can cancel from the
//   toolbar and so that the updates use the statusbar to indicate progress.
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
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

using NDoc.Core;
using NDoc.Core.PropertyGridUI;
using VS = NDoc.VisualStudio;

namespace NDoc.Gui
{
	/// <summary>The main application form.</summary>
	/// <remarks>The main application form contains a listview that holds
	/// assembly and /doc file pairs. You can add, edit, or delete a row
	/// in the listview. You can document multiple assemblies at one time.
	/// <para>NDoc provides for dynamic recognition of available
	/// documenters.  It locates any available assemblies that are capable
	/// of creating documentation by searching the directory for any
	/// assemblies that contain a class that derives from
	/// <see cref="IDocumenter"/> which is defined in the NDoc.Core
	/// namespace.</para>
	/// <para>Currently there are 3 documenters supplied with NDoc:
	/// <list type="bullet">
	/// <item><term>Msdn</term><description>Compiled HTML Help like the
	/// .NET Framework SDK.</description></item>
	/// <item><term>JavaDoc</term><description>JavaDoc-like html
	/// documentation.</description></item>
	/// <item><term>Xml</term><description>An XML file containing the
	/// full documentation.</description></item>
	/// </list>
	/// </para>
	/// <para>NDoc allows you to save documentation projects. NDoc project
	/// files have the .ndoc extension.</para>
	/// <para>The bottom part of the main application form contains
	/// a property grid.  You can edit the properties of the selected
	/// documenter via this property grid.</para>
	/// </remarks>
	public class MainForm : System.Windows.Forms.Form, IBuildStatus
	{
		private const string UNTITLED_PROJECT_NAME = "(Untitled)";

		#region Fields
		#region Required Designer Fields
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuFileNewItem;
		private System.Windows.Forms.ColumnHeader slashDocHeader;
		private System.Windows.Forms.ColumnHeader assemblyHeader;
		private System.Windows.Forms.MenuItem menuFileRecentProjectsItem;
		private System.Windows.Forms.MenuItem menuSpacerItem3;
		private System.Windows.Forms.MenuItem menuSpacerItem2;
		private System.Windows.Forms.MenuItem menuSpacerItem1;
		private System.Windows.Forms.MenuItem menuFileExitItem;
		private System.Windows.Forms.MenuItem menuFileSaveAsItem;
		private System.Windows.Forms.MenuItem menuFileOpenItem;
		private System.Windows.Forms.MenuItem menuFileSaveItem;
		private System.Windows.Forms.MenuItem menuFileItem;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ToolBarButton openToolBarButton;
		private System.Windows.Forms.ImageList toolBarImageList;
		private System.Windows.Forms.ToolBarButton newToolBarButton;
		private System.Windows.Forms.ToolBarButton saveToolBarButton;
		private System.Windows.Forms.ToolBarButton separatorToolBarButton;
		private System.Windows.Forms.ToolBarButton buildToolBarButton;
		private System.Windows.Forms.ToolBarButton viewToolBarButton;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusBarTextPanel;
		private System.Windows.Forms.MenuItem menuDocItem;
		private System.Windows.Forms.MenuItem menuDocBuildItem;
		private System.Windows.Forms.MenuItem menuDocViewItem;
		private GroupBox assembliesHeaderGroupBox;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.ToolBarButton cancelToolBarButton;
		#endregion // Required Designer Fields

		private BuildWorker m_buildWorker;
		private Project project;
		private string projectFilename;
		private System.Windows.Forms.ToolBarButton solutionToolBarButton;
		private System.Windows.Forms.MenuItem menuFileOpenSolution;
		private System.Windows.Forms.MenuItem menuHelpItem;
		private System.Windows.Forms.MenuItem menuAboutItem;
		private System.Windows.Forms.MenuItem menuSpacerItem4;
		private System.Windows.Forms.MenuItem menuSpacerItem6;
		private System.Windows.Forms.MenuItem menuCancelBuildItem;
		private System.Windows.Forms.MenuItem menuViewLicense;
		private System.Windows.Forms.Splitter splitter1;
		private GroupBox documenterHeaderGroupBox;
		private System.Windows.Forms.Label labelDocumenters;
		private System.Windows.Forms.ComboBox comboBoxDocumenters;
		private RuntimePropertyGrid propertyGrid;
		private NDoc.Gui.TraceWindowControl traceWindow1;
		private System.Windows.Forms.MenuItem menuView;
		private System.Windows.Forms.MenuItem menuViewBuildProgress;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuViewOptions;
		private System.Windows.Forms.MenuItem menuViewStatusBar;
		private NDocOptions options;
		private System.Windows.Forms.MenuItem menuHelpContents;
		private System.Windows.Forms.MenuItem menuHelpIndex;
		private System.Windows.Forms.MenuItem menuNDocOnline;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuViewDescriptions;

		private string startingProjectFilename;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuFileImportNamespaces;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuFileExportNamespaces;
		private AssemblyListControl assemblyListControl;
		private System.Windows.Forms.MenuItem detailsMenuItem5;
		private System.Windows.Forms.MenuItem listMenuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private StringCollection recentProjectFilenames = new StringCollection();
		#endregion // Fields

		#region Constructors / Dispose
		/// <summary>Initializes the main application form, locates
		/// available documenters, and sets up the menus.</summary>
		/// <remarks>NDoc project files have a .ndoc extension which
		/// could be a registered file type in the system.  If a .ndoc
		/// project file is double-clicked from explorer then the NDoc
		/// application is called and passed the project file as a command line
		/// argument.  This project filename will get passed into this
		/// constructor.  If no project filename is passed in then the
		/// constructor selects the most recently used project file (from
		/// the MRU list that's stored in the NDoc configuration file) and
		/// initializes the main application form using the information
		/// in that project file.</remarks>
		/// <param name="startingProjectFilename">A project filename passed
		/// in as an argument to the NDoc application.</param>
		public MainForm(string startingProjectFilename)
		{
			this.SetStyle( ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// manually add image resources.
			// This avoids problems with VS.NET designer versioning. 

			Assembly assembly= Assembly.GetExecutingAssembly();

			ImageList.ImageCollection imlcol = this.toolBarImageList.Images;
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.New.ico")));
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.OpenSolution.ico")));
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.OpenFile.ico")));
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.Save.ico")));
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.Build.ico")));
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.Cancel.ico")));
			imlcol.Add(new Icon(assembly.GetManifestResourceStream("NDoc.Gui.graphics.View.ico")));

			this.startingProjectFilename = startingProjectFilename;
		}

		/// <summary>
		/// See <see cref="UserControl.OnLoad"/>
		/// </summary>
		/// <param name="e">event arguments</param>
		protected override void OnLoad(EventArgs e)
		{
			project = new Project();
			project.Modified += new ProjectModifiedEventHandler(OnProjectModified);
			project.ActiveConfigChanged += new EventHandler(project_ActiveConfigChanged);
			project.AssemblySlashDocs.Cleared += new EventHandler(AssemblySlashDocs_Cleared);
			project.AssemblySlashDocs.ItemAdded += new AssemblySlashDocEventHandler(AssemblySlashDocs_ItemRemovedAdded);
			project.AssemblySlashDocs.ItemRemoved += new AssemblySlashDocEventHandler(AssemblySlashDocs_ItemRemovedAdded);

			assemblyListControl.AssemblySlashDocs = project.AssemblySlashDocs;

			foreach ( IDocumenterInfo documenter in InstalledDocumenters.Documenters )
				comboBoxDocumenters.Items.Add( documenter );

			options = new NDocOptions();
			ReadConfig();

			Clear();

			// If a project document wasn't passed in on the command line
			// then try loading up the most recently used project file.
			if ( startingProjectFilename == null )
			{
				if ( this.options.LoadLastProjectOnStart )
				{
					while ( recentProjectFilenames.Count > 0 )
					{
						if ( File.Exists( recentProjectFilenames[0] ) )
						{
							FileOpen( recentProjectFilenames[0] );
							break;
						}
						else
						{
							//the project file was not found, remove it from the MRU
							recentProjectFilenames.RemoveAt(0);
						}
					}
				}
			}
			else
			{
				//load project passed on the command line
				if ( File.Exists( startingProjectFilename ) )
				{
					FileOpen( startingProjectFilename );
				}
				else
				{
					MessageBox.Show(
						this, 
						"The NDoc project file '" + startingProjectFilename + "' does not exist.",
						"Error loading NDoc project file",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
					);
				}
			}

			EnableAssemblyItems();
			MakeMRUMenu();

			SetWindowTitle();
		
			this.traceWindow1.TraceText = string.Format( "[NDoc version {0}]\n", Assembly.GetExecutingAssembly().GetName().Version );

			m_buildWorker = new BuildWorker( this );

			base.OnLoad (e);
		}

		#endregion // Constructors / Dispose

		#region InitializeComponent
		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.menuDocBuildItem = new System.Windows.Forms.MenuItem();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.menuFileExitItem = new System.Windows.Forms.MenuItem();
			this.newToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.toolBarImageList = new System.Windows.Forms.ImageList(this.components);
			this.menuFileSaveItem = new System.Windows.Forms.MenuItem();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuFileItem = new System.Windows.Forms.MenuItem();
			this.menuFileNewItem = new System.Windows.Forms.MenuItem();
			this.menuFileOpenSolution = new System.Windows.Forms.MenuItem();
			this.menuFileOpenItem = new System.Windows.Forms.MenuItem();
			this.menuSpacerItem1 = new System.Windows.Forms.MenuItem();
			this.menuFileSaveAsItem = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuFileImportNamespaces = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuFileExportNamespaces = new System.Windows.Forms.MenuItem();
			this.menuSpacerItem2 = new System.Windows.Forms.MenuItem();
			this.menuFileRecentProjectsItem = new System.Windows.Forms.MenuItem();
			this.menuSpacerItem3 = new System.Windows.Forms.MenuItem();
			this.menuDocItem = new System.Windows.Forms.MenuItem();
			this.menuDocViewItem = new System.Windows.Forms.MenuItem();
			this.menuSpacerItem6 = new System.Windows.Forms.MenuItem();
			this.menuCancelBuildItem = new System.Windows.Forms.MenuItem();
			this.menuView = new System.Windows.Forms.MenuItem();
			this.detailsMenuItem5 = new System.Windows.Forms.MenuItem();
			this.listMenuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuViewBuildProgress = new System.Windows.Forms.MenuItem();
			this.menuViewStatusBar = new System.Windows.Forms.MenuItem();
			this.menuViewDescriptions = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuViewOptions = new System.Windows.Forms.MenuItem();
			this.menuHelpItem = new System.Windows.Forms.MenuItem();
			this.menuHelpContents = new System.Windows.Forms.MenuItem();
			this.menuHelpIndex = new System.Windows.Forms.MenuItem();
			this.menuSpacerItem4 = new System.Windows.Forms.MenuItem();
			this.menuViewLicense = new System.Windows.Forms.MenuItem();
			this.menuNDocOnline = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuAboutItem = new System.Windows.Forms.MenuItem();
			this.slashDocHeader = new System.Windows.Forms.ColumnHeader();
			this.cancelToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.viewToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarTextPanel = new System.Windows.Forms.StatusBarPanel();
			this.assemblyHeader = new System.Windows.Forms.ColumnHeader();
			this.openToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.separatorToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.solutionToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.saveToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.assembliesHeaderGroupBox = new System.Windows.Forms.GroupBox();
			this.assemblyListControl = new NDoc.Gui.AssemblyListControl();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.buildToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.traceWindow1 = new NDoc.Gui.TraceWindowControl();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.documenterHeaderGroupBox = new System.Windows.Forms.GroupBox();
			this.labelDocumenters = new System.Windows.Forms.Label();
			this.comboBoxDocumenters = new System.Windows.Forms.ComboBox();
			this.propertyGrid = new NDoc.Core.PropertyGridUI.RuntimePropertyGrid();
			((System.ComponentModel.ISupportInitialize)(this.statusBarTextPanel)).BeginInit();
			this.assembliesHeaderGroupBox.SuspendLayout();
			this.documenterHeaderGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuDocBuildItem
			// 
			this.menuDocBuildItem.Index = 0;
			this.menuDocBuildItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftB;
			this.menuDocBuildItem.Text = "&Build";
			this.menuDocBuildItem.Click += new System.EventHandler(this.menuDocBuildItem_Click);
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(358, 593);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(144, 15);
			this.progressBar.TabIndex = 24;
			this.progressBar.Visible = false;
			// 
			// menuFileExitItem
			// 
			this.menuFileExitItem.Index = 12;
			this.menuFileExitItem.Text = "&Exit";
			this.menuFileExitItem.Click += new System.EventHandler(this.menuFileExitItem_Click);
			// 
			// newToolBarButton
			// 
			this.newToolBarButton.ImageIndex = 0;
			this.newToolBarButton.ToolTipText = "New";
			// 
			// toolBarImageList
			// 
			this.toolBarImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.toolBarImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// menuFileSaveItem
			// 
			this.menuFileSaveItem.Index = 4;
			this.menuFileSaveItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.menuFileSaveItem.Text = "&Save";
			this.menuFileSaveItem.Click += new System.EventHandler(this.menuFileSaveItem_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFileItem,
																					  this.menuDocItem,
																					  this.menuView,
																					  this.menuHelpItem});
			// 
			// menuFileItem
			// 
			this.menuFileItem.Index = 0;
			this.menuFileItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuFileNewItem,
																						 this.menuFileOpenSolution,
																						 this.menuFileOpenItem,
																						 this.menuSpacerItem1,
																						 this.menuFileSaveItem,
																						 this.menuFileSaveAsItem,
																						 this.menuItem4,
																						 this.menuItem2,
																						 this.menuItem6,
																						 this.menuSpacerItem2,
																						 this.menuFileRecentProjectsItem,
																						 this.menuSpacerItem3,
																						 this.menuFileExitItem});
			this.menuFileItem.Text = "&Project";
			// 
			// menuFileNewItem
			// 
			this.menuFileNewItem.Index = 0;
			this.menuFileNewItem.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.menuFileNewItem.Text = "&New";
			this.menuFileNewItem.Click += new System.EventHandler(this.menuFileNewItem_Click);
			// 
			// menuFileOpenSolution
			// 
			this.menuFileOpenSolution.Index = 1;
			this.menuFileOpenSolution.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftN;
			this.menuFileOpenSolution.Text = "New from &Visual Studio Solution...";
			this.menuFileOpenSolution.Click += new System.EventHandler(this.menuFileOpenSolution_Click);
			// 
			// menuFileOpenItem
			// 
			this.menuFileOpenItem.Index = 2;
			this.menuFileOpenItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.menuFileOpenItem.Text = "&Open...";
			this.menuFileOpenItem.Click += new System.EventHandler(this.menuFileOpenItem_Click);
			// 
			// menuSpacerItem1
			// 
			this.menuSpacerItem1.Index = 3;
			this.menuSpacerItem1.Text = "-";
			// 
			// menuFileSaveAsItem
			// 
			this.menuFileSaveAsItem.Index = 5;
			this.menuFileSaveAsItem.Text = "Save &As...";
			this.menuFileSaveAsItem.Click += new System.EventHandler(this.menuFileSaveAsItem_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 6;
			this.menuItem4.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 7;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFileImportNamespaces});
			this.menuItem2.Text = "Import";
			// 
			// menuFileImportNamespaces
			// 
			this.menuFileImportNamespaces.Index = 0;
			this.menuFileImportNamespaces.Text = "Namespace Summaries";
			this.menuFileImportNamespaces.Click += new System.EventHandler(this.menuFileImportNamespaces_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 8;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFileExportNamespaces});
			this.menuItem6.Text = "Export";
			// 
			// menuFileExportNamespaces
			// 
			this.menuFileExportNamespaces.Index = 0;
			this.menuFileExportNamespaces.Text = "Namespace Summaries";
			this.menuFileExportNamespaces.Click += new System.EventHandler(this.menuFileExportNamespaces_Click);
			// 
			// menuSpacerItem2
			// 
			this.menuSpacerItem2.Index = 9;
			this.menuSpacerItem2.Text = "-";
			// 
			// menuFileRecentProjectsItem
			// 
			this.menuFileRecentProjectsItem.Index = 10;
			this.menuFileRecentProjectsItem.Text = "&Recent Projects";
			// 
			// menuSpacerItem3
			// 
			this.menuSpacerItem3.Index = 11;
			this.menuSpacerItem3.Text = "-";
			// 
			// menuDocItem
			// 
			this.menuDocItem.Index = 1;
			this.menuDocItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuDocBuildItem,
																						this.menuDocViewItem,
																						this.menuSpacerItem6,
																						this.menuCancelBuildItem});
			this.menuDocItem.Text = "&Documentation";
			// 
			// menuDocViewItem
			// 
			this.menuDocViewItem.Index = 1;
			this.menuDocViewItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftV;
			this.menuDocViewItem.Text = "&View";
			this.menuDocViewItem.Click += new System.EventHandler(this.menuDocViewItem_Click);
			// 
			// menuSpacerItem6
			// 
			this.menuSpacerItem6.Index = 2;
			this.menuSpacerItem6.Text = "-";
			// 
			// menuCancelBuildItem
			// 
			this.menuCancelBuildItem.Enabled = false;
			this.menuCancelBuildItem.Index = 3;
			this.menuCancelBuildItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftC;
			this.menuCancelBuildItem.Text = "&Cancel Build";
			this.menuCancelBuildItem.Click += new System.EventHandler(this.menuCancelBuildItem_Click);
			// 
			// menuView
			// 
			this.menuView.Index = 2;
			this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.detailsMenuItem5,
																					 this.listMenuItem7,
																					 this.menuItem8,
																					 this.menuViewBuildProgress,
																					 this.menuViewStatusBar,
																					 this.menuViewDescriptions,
																					 this.menuItem1,
																					 this.menuViewOptions});
			this.menuView.Text = "View";
			// 
			// detailsMenuItem5
			// 
			this.detailsMenuItem5.Index = 0;
			this.detailsMenuItem5.Text = "Details";
			this.detailsMenuItem5.Click += new System.EventHandler(this.detailsMenuItem5_Click);
			// 
			// listMenuItem7
			// 
			this.listMenuItem7.Index = 1;
			this.listMenuItem7.Text = "List";
			this.listMenuItem7.Click += new System.EventHandler(this.listMenuItem7_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 2;
			this.menuItem8.Text = "-";
			// 
			// menuViewBuildProgress
			// 
			this.menuViewBuildProgress.Checked = true;
			this.menuViewBuildProgress.Index = 3;
			this.menuViewBuildProgress.Text = "Build Window";
			this.menuViewBuildProgress.Click += new System.EventHandler(this.menuViewBuildProgress_Click);
			// 
			// menuViewStatusBar
			// 
			this.menuViewStatusBar.Checked = true;
			this.menuViewStatusBar.Index = 4;
			this.menuViewStatusBar.Text = "Status Bar";
			this.menuViewStatusBar.Click += new System.EventHandler(this.menuViewStatusBar_Click);
			// 
			// menuViewDescriptions
			// 
			this.menuViewDescriptions.Checked = true;
			this.menuViewDescriptions.Index = 5;
			this.menuViewDescriptions.Text = "Descriptions";
			this.menuViewDescriptions.Click += new System.EventHandler(this.menuViewDescriptions_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 6;
			this.menuItem1.Text = "-";
			// 
			// menuViewOptions
			// 
			this.menuViewOptions.Index = 7;
			this.menuViewOptions.Text = "Options...";
			this.menuViewOptions.Click += new System.EventHandler(this.menuViewOptions_Click);
			// 
			// menuHelpItem
			// 
			this.menuHelpItem.Index = 3;
			this.menuHelpItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuHelpContents,
																						 this.menuHelpIndex,
																						 this.menuSpacerItem4,
																						 this.menuViewLicense,
																						 this.menuNDocOnline,
																						 this.menuItem3,
																						 this.menuAboutItem});
			this.menuHelpItem.Text = "&Help";
			// 
			// menuHelpContents
			// 
			this.menuHelpContents.Index = 0;
			this.menuHelpContents.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.menuHelpContents.Text = "Contents...";
			this.menuHelpContents.Click += new System.EventHandler(this.menuHelpContents_Click);
			// 
			// menuHelpIndex
			// 
			this.menuHelpIndex.Index = 1;
			this.menuHelpIndex.Text = "Index...";
			this.menuHelpIndex.Click += new System.EventHandler(this.menuHelpIndex_Click);
			// 
			// menuSpacerItem4
			// 
			this.menuSpacerItem4.Index = 2;
			this.menuSpacerItem4.Text = "-";
			// 
			// menuViewLicense
			// 
			this.menuViewLicense.Index = 3;
			this.menuViewLicense.Text = "View License";
			this.menuViewLicense.Click += new System.EventHandler(this.menuViewLicense_Click);
			// 
			// menuNDocOnline
			// 
			this.menuNDocOnline.Index = 4;
			this.menuNDocOnline.Text = "NDoc Online";
			this.menuNDocOnline.Click += new System.EventHandler(this.menuNDocOnline_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 5;
			this.menuItem3.Text = "-";
			// 
			// menuAboutItem
			// 
			this.menuAboutItem.Index = 6;
			this.menuAboutItem.Text = "&About NDoc...";
			this.menuAboutItem.Click += new System.EventHandler(this.menuAboutItem_Click);
			// 
			// slashDocHeader
			// 
			this.slashDocHeader.Text = "/doc Filename";
			this.slashDocHeader.Width = 200;
			// 
			// cancelToolBarButton
			// 
			this.cancelToolBarButton.Enabled = false;
			this.cancelToolBarButton.ImageIndex = 5;
			this.cancelToolBarButton.ToolTipText = "Cancel";
			// 
			// viewToolBarButton
			// 
			this.viewToolBarButton.ImageIndex = 6;
			this.viewToolBarButton.ToolTipText = "View Documentation (Ctrl+Shift+V)";
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 590);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 this.statusBarTextPanel});
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(520, 20);
			this.statusBar.TabIndex = 21;
			this.statusBar.VisibleChanged += new System.EventHandler(this.statusBar_VisibleChanged);
			// 
			// statusBarTextPanel
			// 
			this.statusBarTextPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarTextPanel.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.statusBarTextPanel.Text = "Ready";
			this.statusBarTextPanel.Width = 504;
			// 
			// assemblyHeader
			// 
			this.assemblyHeader.Text = "Assembly Filename";
			this.assemblyHeader.Width = 200;
			// 
			// openToolBarButton
			// 
			this.openToolBarButton.ImageIndex = 2;
			this.openToolBarButton.ToolTipText = "Open ";
			// 
			// separatorToolBarButton
			// 
			this.separatorToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// solutionToolBarButton
			// 
			this.solutionToolBarButton.ImageIndex = 1;
			this.solutionToolBarButton.ToolTipText = "New from Visual Studio Solution";
			// 
			// saveToolBarButton
			// 
			this.saveToolBarButton.ImageIndex = 3;
			this.saveToolBarButton.ToolTipText = "Save";
			// 
			// assembliesHeaderGroupBox
			// 
			this.assembliesHeaderGroupBox.BackColor = System.Drawing.SystemColors.Control;
			this.assembliesHeaderGroupBox.Controls.Add(this.assemblyListControl);
			this.assembliesHeaderGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.assembliesHeaderGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.assembliesHeaderGroupBox.Location = new System.Drawing.Point(0, 28);
			this.assembliesHeaderGroupBox.Name = "assembliesHeaderGroupBox";
			this.assembliesHeaderGroupBox.Size = new System.Drawing.Size(520, 164);
			this.assembliesHeaderGroupBox.TabIndex = 22;
			this.assembliesHeaderGroupBox.TabStop = false;
			this.assembliesHeaderGroupBox.Text = "Select Assemblies to Document";
			// 
			// assemblyListControl
			// 
			this.assemblyListControl.AssemblySlashDocs = null;
			this.assemblyListControl.DetailsView = true;
			this.assemblyListControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assemblyListControl.Location = new System.Drawing.Point(3, 16);
			this.assemblyListControl.Name = "assemblyListControl";
			this.assemblyListControl.Size = new System.Drawing.Size(514, 145);
			this.assemblyListControl.TabIndex = 0;
			this.assemblyListControl.TabStop = false;
			this.assemblyListControl.EditNamespaces += new System.EventHandler(this.assemblyListControl_EditNamespaces);
			this.assemblyListControl.DetailsViewChanged += new System.EventHandler(this.assemblyListControl_DetailsViewChanged);
			// 
			// toolBar
			// 
			this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.newToolBarButton,
																					   this.solutionToolBarButton,
																					   this.openToolBarButton,
																					   this.saveToolBarButton,
																					   this.separatorToolBarButton,
																					   this.buildToolBarButton,
																					   this.cancelToolBarButton,
																					   this.viewToolBarButton});
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.toolBarImageList;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(520, 28);
			this.toolBar.TabIndex = 20;
			this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			this.toolBar.Wrappable = false;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarButton_Click);
			// 
			// buildToolBarButton
			// 
			this.buildToolBarButton.ImageIndex = 4;
			this.buildToolBarButton.ToolTipText = "Build Documentation (Ctrl+Shift+B)";
			// 
			// traceWindow1
			// 
			this.traceWindow1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.traceWindow1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.traceWindow1.Location = new System.Drawing.Point(0, 462);
			this.traceWindow1.Name = "traceWindow1";
			this.traceWindow1.Size = new System.Drawing.Size(520, 128);
			this.traceWindow1.TabIndex = 25;
			this.traceWindow1.TabStop = false;
			this.traceWindow1.TraceText = "";
			this.traceWindow1.VisibleChanged += new System.EventHandler(this.traceWindow1_VisibleChanged);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 459);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(520, 3);
			this.splitter1.TabIndex = 26;
			this.splitter1.TabStop = false;
			// 
			// documenterHeaderGroupBox
			// 
			this.documenterHeaderGroupBox.BackColor = System.Drawing.SystemColors.Control;
			this.documenterHeaderGroupBox.Controls.Add(this.labelDocumenters);
			this.documenterHeaderGroupBox.Controls.Add(this.comboBoxDocumenters);
			this.documenterHeaderGroupBox.Controls.Add(this.propertyGrid);
			this.documenterHeaderGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.documenterHeaderGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.documenterHeaderGroupBox.Location = new System.Drawing.Point(0, 192);
			this.documenterHeaderGroupBox.Name = "documenterHeaderGroupBox";
			this.documenterHeaderGroupBox.Size = new System.Drawing.Size(520, 267);
			this.documenterHeaderGroupBox.TabIndex = 27;
			this.documenterHeaderGroupBox.TabStop = false;
			this.documenterHeaderGroupBox.Text = "Select and Configure Documenter";
			// 
			// labelDocumenters
			// 
			this.labelDocumenters.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelDocumenters.Location = new System.Drawing.Point(16, 26);
			this.labelDocumenters.Name = "labelDocumenters";
			this.labelDocumenters.Size = new System.Drawing.Size(112, 21);
			this.labelDocumenters.TabIndex = 10;
			this.labelDocumenters.Text = "Documentation Type:";
			this.labelDocumenters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboBoxDocumenters
			// 
			this.comboBoxDocumenters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDocumenters.DropDownWidth = 160;
			this.comboBoxDocumenters.Location = new System.Drawing.Point(128, 24);
			this.comboBoxDocumenters.Name = "comboBoxDocumenters";
			this.comboBoxDocumenters.Size = new System.Drawing.Size(160, 21);
			this.comboBoxDocumenters.TabIndex = 9;
			this.comboBoxDocumenters.SelectedIndexChanged += new System.EventHandler(this.comboBoxDocumenters_SelectedIndexChanged);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(8, 56);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(504, 204);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.Text = "PropertyGrid";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 610);
			this.Controls.Add(this.documenterHeaderGroupBox);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.traceWindow1);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.assembliesHeaderGroupBox);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.toolBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.MinimumSize = new System.Drawing.Size(504, 460);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "NDoc";
			((System.ComponentModel.ISupportInitialize)(this.statusBarTextPanel)).EndInit();
			this.assembliesHeaderGroupBox.ResumeLayout(false);
			this.documenterHeaderGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion // InitializeComponent

		#region Methods
		private void OnProjectModified(object sender, EventArgs e)
		{
			SetWindowTitle();
		}

		private void SetWindowTitle()
		{
			string projectName;

			if (projectFilename != null)
			{
				if (projectFilename == UNTITLED_PROJECT_NAME)
				{
					projectName = projectFilename;
				}
				else
				{
					projectName = Path.GetFileName( projectFilename );
					projectName = projectName.Substring( 0, projectName.LastIndexOf('.') );
				}

				this.Text = "NDoc - " + projectName + ( project.IsDirty ? "*" : "" );
			}
		}

		/// <summary>
		/// Enables/disables the Save and SaveAs menu items.
		/// </summary>
		/// <param name="bEnable"><b>true</b> for enabling the menu items, <b>false</b> for disabling.</param>
		private void EnableMenuItems(bool bEnable)
		{
			menuFileSaveItem.Enabled = bEnable;
			menuFileSaveAsItem.Enabled = bEnable;
			saveToolBarButton.Enabled = bEnable;
		}

		/// <summary>
		/// Enable/disable the buttons in the GUI based on whether there any assemblies to document.
		/// </summary>
		private void EnableAssemblyItems()
		{
			bool bEnable = project.AssemblySlashDocs.Count > 0;

			menuDocBuildItem.Enabled = bEnable;
			menuDocViewItem.Enabled = bEnable;
			buildToolBarButton.Enabled = bEnable;
			viewToolBarButton.Enabled = bEnable;
		}

		/// <summary>
		/// Clears and recreates the most recently used files (MRU) menu.
		/// </summary>
		private void MakeMRUMenu()
		{
			menuFileRecentProjectsItem.Enabled = false;

			if ( recentProjectFilenames.Count > 0 )
			{
				int count = 1;

				menuFileRecentProjectsItem.MenuItems.Clear();
				menuFileRecentProjectsItem.Enabled = true;

				foreach ( string project in recentProjectFilenames )
				{
					MenuItem  menuItem = new MenuItem ();

					menuItem.Text = string.Format( "&{0} {1}", count, project );
					menuItem.Click += new System.EventHandler (this.menuMRUItem_Click);
					menuFileRecentProjectsItem.MenuItems.Add(menuItem);

					count++;

					if ( count > this.options.MRUSize )
						break;
				}
			}
		}

		/// <summary>
		/// Updates the MRU menu to reflect the project that was just opened.
		/// </summary>
		private void UpdateMRUList()
		{
			try
			{
				recentProjectFilenames.Remove(projectFilename);
			}
			catch( Exception )
			{
				// Remove throws an exception if the item isn't in the list.
				// But that's ok for us so do nothing.
			}

			recentProjectFilenames.Insert( 0, projectFilename );
			MakeMRUMenu();
			EnableAssemblyItems();
		}

		private static Point GetOnScreenLocation( Point pt )
		{
			// look for a screen that contains this point
			// if one is found the point is ok so return it
			foreach ( Screen screen in Screen.AllScreens )
			{
				if ( screen.Bounds.Contains( pt ) )
					return pt;
			}

			// otherwise return the upper left point of the primary screen
			return new Point( Screen.PrimaryScreen.WorkingArea.X, Screen.PrimaryScreen.WorkingArea.Y );
		}

		/// <summary>Reads in the NDoc configuration file from the
		/// application directory.</summary>
		/// <remarks>The config file stores the most recently used (MRU)
		/// list of project files.  It also stores which documenter was
		/// being used last.</remarks>
		private void ReadConfig()
		{
			Settings settings = new Settings( Settings.UserSettingsFile );

			this.Location = GetOnScreenLocation( (Point)settings.GetSetting( "gui", "location", new Point( Screen.PrimaryScreen.WorkingArea.Top, Screen.PrimaryScreen.WorkingArea.Left ) ) );

			Screen screen = Screen.FromControl( this );
			this.Size = (Size)settings.GetSetting( "gui", "size", new Size( screen.WorkingArea.Width / 3, screen.WorkingArea.Height - 20 ) );
			
			// size the window to the working area if it is larger (can happen when resolution changes)
			if ( this.Height > screen.WorkingArea.Height )
				this.Height = screen.WorkingArea.Height;

			if ( this.Width > screen.WorkingArea.Width )
				this.Width = screen.WorkingArea.Width;

			if ( settings.GetSetting( "gui", "maximized", false ) )
				this.WindowState = FormWindowState.Maximized;			

			this.traceWindow1.Visible = settings.GetSetting( "gui", "viewTrace", true );
			this.traceWindow1.Height = settings.GetSetting( "gui", "traceWindowHeight", this.traceWindow1.Height );
			this.statusBar.Visible = settings.GetSetting( "gui", "statusBar", true );
			this.ShowDescriptions = settings.GetSetting( "gui", "showDescriptions", true );
			this.assemblyListControl.DetailsView = settings.GetSetting( "gui", "detailedAssemblyView", false );

			IList list = recentProjectFilenames;
			settings.GetSettingList( "gui", "mru", typeof( string ), ref list );		
	
			string documenterName = settings.GetSetting( "gui", "documenter", "MSDN" );

			this.options.LoadLastProjectOnStart = settings.GetSetting( "gui", "loadLastProjectOnStart", true );
			this.options.ShowProgressOnBuild = settings.GetSetting( "gui", "showProgressOnBuild", false );
			this.options.MRUSize = settings.GetSetting( "gui", "mruSize", 8 );

			int index = 0;

			foreach ( IDocumenterInfo documenter in comboBoxDocumenters.Items )
			{
				if ( documenter.Name == documenterName )
				{
					comboBoxDocumenters.SelectedIndex = index;
					break;
				}

				++index;
			}
		}

		/// <summary>Writes out the NDoc configuration file to the
		/// application directory.</summary>
		/// <remarks>The config file stores the most recently used (MRU)
		/// list of project files.  It also stores which documenter was
		/// being used last.</remarks>
		private void WriteConfig()
		{			
			using( Settings settings = new Settings( Settings.UserSettingsFile ) )
			{
				if ( this.WindowState == FormWindowState.Maximized )
				{
					settings.SetSetting( "gui", "maximized", true );
				}
				else if ( this.WindowState == FormWindowState.Normal )
				{
					settings.SetSetting( "gui", "maximized", false );
					settings.SetSetting( "gui", "location", this.Location );
					settings.SetSetting( "gui", "size", this.Size );
				}
				settings.SetSetting( "gui", "viewTrace", this.traceWindow1.Visible );
				settings.SetSetting( "gui", "traceWindowHeight", this.traceWindow1.Height );
				settings.SetSetting( "gui", "statusBar", this.statusBar.Visible );
				settings.SetSetting( "gui", "showDescriptions", this.ShowDescriptions );
				settings.SetSetting( "gui", "detailedAssemblyView", this.assemblyListControl.DetailsView );

				if ( project.ActiveDocumenter != null )
					settings.SetSetting( "gui", "documenter", project.ActiveDocumenter.Name );

				// Trim our MRU list down to max amount before writing the config.
				while (recentProjectFilenames.Count > this.options.MRUSize)
					recentProjectFilenames.RemoveAt(this.options.MRUSize);

				settings.SetSettingList( "gui", "mru", "project", recentProjectFilenames );			
			}
		}

		private void FileOpen(string fileName)
		{
			bool  bFailed = true;

			try
			{
				using ( new WaitCursor( this ) )
				{
					Directory.SetCurrentDirectory( Path.GetDirectoryName(fileName) );

					try
					{
						project.Read(fileName);
					}
					catch (DocumenterPropertyFormatException e)
					{
						WarningForm.ShowWarning( "Invalid Properties in Project File.", e.Message  + "Documenter defaults will be used....", this );
					}

					projectFilename = fileName;
					SetWindowTitle();

					RefreshPropertyGrid();

					UpdateMRUList();

					EnableMenuItems(true);

					project.IsDirty = false;

					bFailed = false;
				}
			}
			catch (DocumenterException docEx)
			{
				ErrorForm.ShowError( "Unable to read in project file", docEx, this );
			}
			catch (Exception ex)
			{
				ErrorForm.ShowError( "An error occured while trying to read in project file:\n" + fileName + ".", ex, this );
			}

			if ( bFailed )
			{
				recentProjectFilenames.Remove(fileName);
				MakeMRUMenu();
				Clear();
			}
		}

		private void FileSave(string fileName)
		{
			try
			{
				project.Write( fileName );
				using ( Settings settings = new Settings( Settings.UserSettingsFile ) )
					settings.SetSetting( "gui", "lastSaveDirectory", Path.GetDirectoryName( fileName ) );

				SetWindowTitle();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.InnerException.Message, "Save", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				FileSaveAs();
			}
		}

		private void FileSaveAs()
		{
			using ( SaveFileDialog saveFileDlg = new SaveFileDialog() )
			{
				if (projectFilename == UNTITLED_PROJECT_NAME)
				{
					Settings settings = new Settings( Settings.UserSettingsFile );
					saveFileDlg.InitialDirectory = settings.GetSetting( "gui", "lastSaveDirectory", App.RuntimeLocation );
					saveFileDlg.FileName = @".\Untitled.ndoc";
				}
				else
				{
					saveFileDlg.InitialDirectory = Path.GetDirectoryName(projectFilename);
					saveFileDlg.FileName = Path.GetFileName(projectFilename);
				}

				saveFileDlg.Filter = "NDoc Project files (*.ndoc)|*.ndoc|All files (*.*)|*.*" ;

				if( saveFileDlg.ShowDialog() == DialogResult.OK )
				{
					FileSave( saveFileDlg.FileName );

					projectFilename = saveFileDlg.FileName;
					SetWindowTitle();
					UpdateMRUList();
					EnableMenuItems(true);
					propertyGrid.Refresh();
				}
			}
		}

		private void Clear()
		{
			projectFilename = UNTITLED_PROJECT_NAME;
			project.Clear();

			RefreshPropertyGrid();

			EnableAssemblyItems();

			EnableMenuItems(false);
			SetWindowTitle();
		}

		private void RefreshPropertyGrid()
		{
			if ( ( comboBoxDocumenters.SelectedIndex == -1 ) && ( comboBoxDocumenters.Items.Count > 0 ) )
				comboBoxDocumenters.SelectedIndex = 0;

			SelectedDocumenterChanged();
		}
		#endregion // Methods

		#region Event Handlers
		/// <summary>
		/// Resets NDoc to an empty project by calling <see cref="Clear"/>.
		/// </summary>
		/// <param name="sender">The File->New menu item (not used).</param>
		/// <param name="e">Event arguments (not used).</param>
		/// <seealso cref="Clear"/>
		private void menuFileNewItem_Click (object sender, System.EventArgs e)
		{
			if ( QueryContinueDiscardProject() )
				Clear();
		}

		private void menuFileOpenSolution_Click (object sender, System.EventArgs e)
		{
			if ( QueryContinueDiscardProject() )
			{
				using ( OpenFileDialog openFileDlg = new OpenFileDialog() )
				{
					openFileDlg.InitialDirectory = Directory.GetCurrentDirectory();
					openFileDlg.Filter = "Visual Studio Solution files (*.sln)|*.sln|All files (*.*)|*.*" ;

					if( openFileDlg.ShowDialog() == DialogResult.OK )
					{
						VS.Solution sol = new VS.Solution( openFileDlg.FileName );

						string warningMessages = String.Empty;

						if ( sol.ProjectCount != 0 )
						{
							using( SolutionForm sf = new SolutionForm() )
							{
								sf.Text = "Solution " + sol.Name;

								sf.ConfigList.Items.Clear();

								foreach (string configkey in sol.GetConfigurations())
									sf.ConfigList.Items.Add(configkey);

								sf.ShowDialog(this);
								if (sf.ConfigList.SelectedIndex < 0)
									return;

								//clear current ndoc project settings
								Clear();

								warningMessages = LoadFromSolution( sol, (string)sf.ConfigList.SelectedItem );

								EnableMenuItems(true);
								EnableAssemblyItems();

								projectFilename =  Path.Combine(sol.Directory, sol.Name + ".ndoc");
							}
						}
						else
						{
							warningMessages = "There are no projects in this solution that NDoc can import.\n\n";
							warningMessages += "Either the solution is blank, or the projects contained within\n";
							warningMessages += "the solution are not of a type NDoc can import.\n"; 
						}

						if ( warningMessages.Length > 0 )
							WarningForm.ShowWarning( "VS Solution Import Warnings", warningMessages, this );
					}
				}
			}
		}

		private string LoadFromSolution( VS.Solution sol, string solconfig )
		{
			using ( new WaitCursor( this ) )
			{
				string warningMessages = "";
				foreach (VS.Project p in sol.GetProjects())
				{
					string projconfig = sol.GetProjectConfigName( solconfig, p.ID.ToString() );

					if ( projconfig == null )
					{
						warningMessages += String.Format("VS Project {0} could not be imported.\n- There are no settings in the project file for configuration '{1}'\n\n",p.Name,solconfig);
						continue;
					}

					string apath = p.GetRelativeOutputPathForConfiguration(projconfig);
					string xpath = p.GetRelativePathToDocumentationFile(projconfig);
					string spath = sol.Directory;

					AssemblySlashDoc assemblySlashDoc = new AssemblySlashDoc();
					assemblySlashDoc.Assembly.Path = Path.Combine( spath, apath );

					if ( !File.Exists( assemblySlashDoc.Assembly.Path ) )
						warningMessages += String.Format("VS Project '{0}' has been imported, but the specified assembly does not exist.\n- You will not be able to build documentation for this project until its assembly has been successfully compiled...\n\n",p.Name);

					if ( xpath != null && xpath.Length > 0  )
					{
						assemblySlashDoc.SlashDoc.Path = Path.Combine( spath, xpath );

						if ( !File.Exists( assemblySlashDoc.SlashDoc.Path ) )
							warningMessages += String.Format("VS Project '{0}' has been imported, but the XML documentation file specified in the project cannot be found.\n- This can occur if the project is set to do 'incremental' compiles.\n- NDoc output will be very limited until the VS project is rebuilt with XML documntation...\n",p.Name);						
					}
					else
					{
						warningMessages += String.Format("VS Project '{0}' has been imported, but the project is not set to produce XML documentation.\n- NDoc output for this assembly will be very limited...\n\n",p.Name);
					}

					project.AssemblySlashDocs.Add( assemblySlashDoc );
				}

				return warningMessages;
			}
		}

		private void menuFileOpenItem_Click (object sender, System.EventArgs e)
		{
			if ( QueryContinueDiscardProject() )
			{
				using( OpenFileDialog openFileDlg = new OpenFileDialog() )
				{
					openFileDlg.InitialDirectory = Directory.GetCurrentDirectory();
					openFileDlg.Filter = "NDoc Project files (*.ndoc)|*.ndoc|All files (*.*)|*.*" ;

					if( openFileDlg.ShowDialog() == DialogResult.OK )
						FileOpen(openFileDlg.FileName);
				}
			}
		}

		private bool QueryContinueDiscardProject()
		{
			bool continueDiscard = true;

			if ( project.IsDirty )
			{
				switch ( PromptToSave() )
				{
					case DialogResult.Yes:
						SaveOrSaveAs();
						break;

					case DialogResult.No:
						break;

					case DialogResult.Cancel:
						continueDiscard = false;
						break;
				}
			}

			return continueDiscard;
		}

		private void menuFileSaveItem_Click (object sender, System.EventArgs e)
		{
			SaveOrSaveAs();
		}

		private void SaveOrSaveAs()
		{
			if ( projectFilename == UNTITLED_PROJECT_NAME )
				FileSaveAs();

			else
				FileSave( projectFilename );
		}

		private void menuFileSaveAsItem_Click (object sender, System.EventArgs e)
		{
			FileSaveAs();
		}

		private void menuMRUItem_Click (object sender, System.EventArgs e)
		{
			string fileName = ((MenuItem)sender).Text.Substring(3);

			if ( QueryContinueDiscardProject() )
			{
				if ( File.Exists( fileName ) )
				{
					FileOpen( fileName );
				}
				else
				{
					MessageBox.Show( this, "Project file doesn't exist.", "Open", MessageBoxButtons.OK, MessageBoxIcon.Information );
					try
					{
						recentProjectFilenames.Remove(fileName);
					}
					catch
					{
						// Remove throws an exception if the item isn't in the list.
						// But that's ok for us so do nothing.
					}
					MakeMRUMenu();
				}
			}
		}

		private void menuFileExitItem_Click (object sender, System.EventArgs e)
		{
			Close();
		}

		private void menuDocBuildItem_Click(object sender, System.EventArgs e)
		{
			Debug.Assert( m_buildWorker != null && m_buildWorker.IsBuilding == false );
			Debug.Assert( project.ActiveConfig != null );

			IDocumenter documenter = project.ActiveConfig.CreateDocumenter();

			string message = documenter.CanBuild( project );

			if ( message == null )
			{
				this.Cursor = Cursors.AppStarting;

				//make sure the current directory is the project directory
				if ( projectFilename != UNTITLED_PROJECT_NAME )
					Directory.SetCurrentDirectory( Path.GetDirectoryName( projectFilename ) );

				if ( Directory.Exists( Path.GetDirectoryName( documenter.MainOutputFile ) ) == false )
					Directory.CreateDirectory( Path.GetDirectoryName( documenter.MainOutputFile ) );

				string logPath = Path.Combine( Path.GetDirectoryName( documenter.MainOutputFile ), "ndoc.log" );

				ConfigureUIForBuild( true );

				Trace.Listeners.Add( new TextWriterTraceListener( new StreamWriter( logPath, false, new System.Text.UTF8Encoding( false ) ),"ndoc" ) );

				UpdateProgress( "Building documentation...", 0 );

				m_buildWorker.Build( documenter, project );	
			}
			else
			{
				MessageBox.Show( this, message, "NDoc", MessageBoxButtons.OK, MessageBoxIcon.Stop );
			}
		}

		private void menuCancelBuildItem_Click(object sender, System.EventArgs e)
		{
			statusBarTextPanel.Text = "Cancelling build ...";
			m_buildWorker.Cancel();
		}

		private void ConfigureUIForBuild(bool starting)
		{
			foreach (ToolBarButton button in toolBar.Buttons)
			{
				if ( button == cancelToolBarButton )
					button.Enabled = starting;

				else
					button.Enabled = !starting;
			}

			foreach (MenuItem subMenuItem in menuFileItem.MenuItems)
				subMenuItem.Enabled = !starting;

			menuDocBuildItem.Enabled = !starting;
			menuDocViewItem.Enabled = !starting;
			menuCancelBuildItem.Enabled = starting;
			menuAboutItem.Enabled = !starting;
			menuViewBuildProgress.Enabled = !starting;
			menuViewOptions.Enabled = !starting;

			assembliesHeaderGroupBox.Enabled = !starting;
			documenterHeaderGroupBox.Enabled = !starting;
            
			if ( starting )
			{
				this.traceWindow1.Clear();

				if ( this.options.ShowProgressOnBuild && !this.traceWindow1.Visible )
					this.traceWindow1.Visible = true;

				if ( this.traceWindow1.Visible )
					this.traceWindow1.Connect();
			}
			
			progressBar.Visible = starting;
		}

		private delegate void UpdateProgressDelegate(string text, int percent);
		private void UpdateProgress(string text, int percent)
		{
			if ( text != null )
			{
				statusBarTextPanel.Text = text;
				Trace.WriteLine( text );
			}
			progressBar.Value = Math.Min( Math.Max( percent, 0 ), 100 );
			statusBar.Update();
		}

		private void menuDocViewItem_Click(object sender, System.EventArgs e)
		{
			Debug.Assert( project.ActiveConfig != null );
			IDocumenter documenter = project.ActiveConfig.CreateDocumenter();

			//make sure the current directory is the project directory
			if ( projectFilename != UNTITLED_PROJECT_NAME )
				Directory.SetCurrentDirectory( Path.GetDirectoryName( projectFilename ) );

			try
			{
				documenter.View();
			}
			catch (FileNotFoundException)
			{
				string msg = "The documentation has not been built yet.\nWould you like to build it now?";
				DialogResult result = MessageBox.Show( this, msg, "NDoc", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );

				if ( result == DialogResult.Yes )
					menuDocBuildItem_Click(sender, e);
			}
			catch (DocumenterException ex)
			{
				MessageBox.Show( this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void menuAboutItem_Click(object sender, System.EventArgs e)
		{
			using ( AboutForm aboutForm = new AboutForm() )
			{
				aboutForm.StartPosition = FormStartPosition.CenterParent;
				aboutForm.ShowDialog(this);
			}
		}

		private void toolBarButton_Click(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == cancelToolBarButton)
				menuCancelBuildItem_Click(sender, EventArgs.Empty);

			else if (e.Button == newToolBarButton)
				menuFileNewItem_Click(sender, EventArgs.Empty);

			else if (e.Button == solutionToolBarButton)
				menuFileOpenSolution_Click(sender, EventArgs.Empty);
			
			else if (e.Button == openToolBarButton)
				menuFileOpenItem_Click(sender, EventArgs.Empty);
			
			else if (e.Button == saveToolBarButton)
				menuFileSaveItem_Click(sender, EventArgs.Empty);

			else if (e.Button == buildToolBarButton)
				menuDocBuildItem_Click(sender, EventArgs.Empty);

			else if (e.Button == viewToolBarButton)
				menuDocViewItem_Click(sender, EventArgs.Empty);
		}

		private void comboBoxDocumenters_SelectedIndexChanged (object sender, System.EventArgs e)
		{
			SelectedDocumenterChanged();
		}

		private void SelectedDocumenterChanged()
		{
			project.ActiveDocumenter = comboBoxDocumenters.SelectedItem as IDocumenterInfo;
		}

		private DialogResult PromptToSave()
		{
			return MessageBox.Show( this, "Save changes to project " + projectFilename + "?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
		}

		/// <summary>Prompts the user to save the project if it's dirty.</summary>
		/// <param name="e">Cancel args</param>
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !QueryContinueDiscardProject();

			if ( e.Cancel == false && m_buildWorker.IsBuilding )
			{
				string message = "A build is currently in process.\nExiting now will abort this build.\n\nExit anyway?";
				e.Cancel = MessageBox.Show( this, message, "Abort build?", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.No;
			}																				  
			base.OnClosing(e);
		}

		/// <summary>
		/// Writes app configuration file
		/// </summary>
		/// <param name="e">args</param>
		protected override void OnClosed(EventArgs e)
		{
			WriteConfig();
			base.OnClosed (e);
		}

		//This makes the property grid more responsive on update
		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			this.project.IsDirty = true;
			propertyGrid.Refresh();
		}

		#endregion // Event Handlers

        /// <summary>
        /// Opens the license file in its associates application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        private void menuViewLicense_Click(object sender, System.EventArgs e)
		{
			string path = App.LicenseFilePath;
            if ( File.Exists( path ) == false ) 
                MessageBox.Show( this, "Could not find the license file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			else
				Process.Start( path );
		}

		private void traceWindow1_VisibleChanged(object sender, System.EventArgs e)
		{
			splitter1.Visible = traceWindow1.Visible;

			// make sure the splitter splits the trace window, not some other docked control
			splitter1.Top = traceWindow1.Top - splitter1.Height;
			menuViewBuildProgress.Checked = traceWindow1.Visible;
			
			// disconnect from trace events when the trace window is being hidden
			if ( !traceWindow1.Visible )
				this.traceWindow1.Disconnect();
		}

		private void menuViewBuildProgress_Click(object sender, System.EventArgs e)
		{
			traceWindow1.Visible = !traceWindow1.Visible;
		}

		private void menuViewOptions_Click(object sender, System.EventArgs e)
		{
			using( OptionsForm optionsForm = new OptionsForm( (NDocOptions)this.options.Clone() ) )
			{
				if ( optionsForm.ShowDialog() == DialogResult.OK )
				{
					this.options = optionsForm.Options;

					// save the user settings
					using( Settings settings = new Settings( Settings.UserSettingsFile ) )
					{
						settings.SetSetting( "gui", "loadLastProjectOnStart", this.options.LoadLastProjectOnStart );
						settings.SetSetting( "gui", "showProgressOnBuild", this.options.ShowProgressOnBuild );
						settings.SetSetting( "gui", "mruSize", this.options.MRUSize );
					}

					// save machine settings
					using( Settings settings = new Settings( Settings.MachineSettingsFile ) )
						settings.SetSetting( "compilers", "htmlHelpWorkshopLocation", this.options.HtmlHelpWorkshopLocation );
				}
			}
		}

		private void menuViewStatusBar_Click(object sender, System.EventArgs e)
		{
			this.statusBar.Visible = !this.statusBar.Visible;
		}

		private void statusBar_VisibleChanged(object sender, System.EventArgs e)
		{
			this.menuViewStatusBar.Checked = this.statusBar.Visible;		
		}

		private void menuNDocOnline_Click(object sender, System.EventArgs e)
		{
			Process.Start( App.WebSiteUri );
		}

		private void menuHelpContents_Click(object sender, System.EventArgs e)
		{
			Help.ShowHelp( this, App.HelpFilePath );
		}

		private void menuHelpIndex_Click(object sender, System.EventArgs e)
		{
			Help.ShowHelpIndex( this, App.HelpFilePath );
		}

		private void menuViewDescriptions_Click(object sender, System.EventArgs e)
		{
			ShowDescriptions = !ShowDescriptions;
		}

		/// <summary>
		/// Handles drag enter and raises the DragEnter event
		/// </summary>
		/// <param name="drgevent">Drag arguments</param>
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if( drgevent.Data.GetDataPresent( DataFormats.FileDrop ) && DragDropHandler.CanDrop( (string[])drgevent.Data.GetData( DataFormats.FileDrop ) ) == DropFileType.Project )
				drgevent.Effect = DragDropEffects.Link;

			else
				drgevent.Effect = DragDropEffects.None;

			base.OnDragEnter (drgevent);
		}

		/// <summary>
		/// Handles drag drop and raises the DragDrop event
		/// </summary>
		/// <param name="drgevent">Drag arguments</param>
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			// ask the user if they want to save if the current project if dirty
			if( QueryContinueDiscardProject() )
			{
				string[] files = (string[])drgevent.Data.GetData( DataFormats.FileDrop );
				FileOpen( DragDropHandler.GetProjectFilePath( files ) );
			}
			base.OnDragDrop (drgevent);
		}

		private void menuFileImportNamespaces_Click(object sender, System.EventArgs e)
		{
			using ( OpenFileDialog openFileDlg = new OpenFileDialog() )
			{
				openFileDlg.InitialDirectory = Directory.GetCurrentDirectory();
				openFileDlg.Filter = "NDoc Namespace Summary files (*.xml)|*.xml|NDoc Project files (*.ndoc)|*.ndoc|All files (*.*)|*.*" ;

				if( openFileDlg.ShowDialog() == DialogResult.OK )
				{
					StreamReader streamReader=null;
					try
					{
						streamReader = new StreamReader(openFileDlg.FileName);
						XmlTextReader reader = new XmlTextReader(streamReader);
						reader.MoveToContent();
						project.Namespaces.Read(reader);
						reader.Close();
					}
					catch(Exception ex)
					{
						ErrorForm.ShowError( ex, this );					
					}
					finally
					{
						if ( streamReader != null ) 
							streamReader.Close();
					}
				}
			}
		}

		private void menuFileExportNamespaces_Click(object sender, System.EventArgs e)
		{
			using ( SaveFileDialog saveFileDlg = new SaveFileDialog() )
			{
				saveFileDlg.InitialDirectory = Directory.GetCurrentDirectory();
				saveFileDlg.Filter = "NDoc Namespace Summary files (*.xml)|*.xml|All files (*.*)|*.*" ;

				if( saveFileDlg.ShowDialog() == DialogResult.OK )
				{
					StreamWriter streamWriter = null;
					try
					{
						streamWriter = new StreamWriter( saveFileDlg.FileName, false, new System.Text.UTF8Encoding(false) );
						XmlTextWriter writer = new XmlTextWriter( streamWriter );
						writer.Formatting = Formatting.Indented;
						writer.Indentation = 4;
						project.Namespaces.Write( writer );
						writer.Close();
						streamWriter.Close();
					}
					catch(Exception ex)
					{
						if ( streamWriter != null ) 
							streamWriter.Close();

						ErrorForm.ShowError( ex, this );
					}
				}
			}
		}

		private void assemblyListControl_EditNamespaces(object sender, System.EventArgs e)
		{
			Debug.Assert( project.ActiveConfig != null );
			IDocumenter documenter = project.ActiveConfig.CreateDocumenter();

			string message = documenter.CanBuild(project, true);
			if ( message == null )
			{
				this.statusBarTextPanel.Text="refreshing namespace list from assemblies...";				
				using ( NamespaceSummariesForm form = new NamespaceSummariesForm( project ) )
				{
					form.StartPosition = FormStartPosition.CenterParent;
					this.statusBarTextPanel.Text = "";
					form.ShowDialog(this);		
				}
			}	
			else
			{
				MessageBox.Show( this, message, "NDoc", MessageBoxButtons.OK, MessageBoxIcon.Stop );
			}
		}

		private bool ShowDescriptions
		{
			get
			{
				return this.propertyGrid.HelpVisible;
			}
			set
			{
				this.propertyGrid.HelpVisible = value;
				menuViewDescriptions.Checked = value;
			}
		}

		private void AssemblySlashDocs_Cleared(object sender, EventArgs e)
		{
			EnableAssemblyItems();
		}

		private void AssemblySlashDocs_ItemRemovedAdded(object sender, AssemblySlashDocEventArgs args)
		{
			EnableMenuItems( this.project.AssemblySlashDocs.Count > 0 );
			EnableAssemblyItems();
		}

		private void detailsMenuItem5_Click(object sender, System.EventArgs e)
		{
			this.assemblyListControl.DetailsView = true;
		}

		private void listMenuItem7_Click(object sender, System.EventArgs e)
		{
			this.assemblyListControl.DetailsView = false;
		}

		private void assemblyListControl_DetailsViewChanged(object sender, System.EventArgs e)
		{
			this.detailsMenuItem5.Enabled = !this.assemblyListControl.DetailsView;
			this.listMenuItem7.Enabled = this.assemblyListControl.DetailsView;
		}

		private void project_ActiveConfigChanged(object sender, EventArgs e)
		{
			propertyGrid.SelectedObject = project.ActiveConfig;
		}

		#region IBuildStatus Members

		private delegate void ExceptionDelegate( Exception e );

		/// <summary>
		/// Called from teh build worker when an exception occurs
		/// </summary>
		/// <param name="e">The exception</param>
		public void BuildException(Exception e)
		{
			if ( this.InvokeRequired )
			{
				this.Invoke( new ExceptionDelegate( this.BuildException ), new object[]{ e } );
			}
			else
			{
				// Process exception
				Trace.WriteLine( "An error occured while trying to build the documentation.\n" );
				App.BuildTraceError( e );

				if ( App.GetInnermostException( e ) is DocumenterException )
					ErrorForm.ShowError( "NDoc Documenter Error", e, this );

				else 
					ErrorForm.ShowError( e, this );	
			}
		}

		/// <summary>
		/// Called from the build worker when the build is complete
		/// </summary>
		/// <remarks>This method is called regardless if the build is successful or not</remarks>
		public void BuildComplete()
		{
			if ( this.InvokeRequired )
			{
				this.Invoke( new MethodInvoker( this.BuildComplete ) );
			}
			else
			{
				statusBarTextPanel.Text = "Ready";

				if ( this.traceWindow1.IsDisposed == false && this.traceWindow1.Visible )
					this.traceWindow1.Disconnect();

				TraceListener listener = Trace.Listeners["ndoc"];
				listener.Close();
				listener.Dispose();
				Trace.Listeners.Remove("ndoc");
		
				ConfigureUIForBuild( false );
				this.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// Called from teh build worker when the build is aborted
		/// </summary>
		public void BuildCancelled()
		{
			Trace.WriteLine( "Build cancelled" );
		}

		/// <summary>
		/// Receives progress reporting
		/// </summary>
		/// <param name="e">progress arguments</param>
		public void ReportProgress(ProgressArgs e)
		{
			// This gets called from another thread so we must thread marhal back to the GUI thread.
			Delegate d = new UpdateProgressDelegate( UpdateProgress );
			this.Invoke( d, new Object[] { e.Status, e.Progress } );
		}
		#endregion
	}
}
