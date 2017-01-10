using System;
using Microsoft.Office.Core;
using Extensibility;
using System.Runtime.InteropServices;
using EnvDTE;
using System.Diagnostics;
using NDoc.Core;
using NDoc.Documenter.Msdn;
using NDoc.Documenter.Xml;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace NDoc.Addins.NDocBuild
{
	#region Read me for Add-in installation and setup information.
	// When run, the Add-in wizard prepared the registry for the Add-in.
	// At a later time, if the Add-in becomes unavailable for reasons such as:
	//   1) You moved this project to a computer other than which is was originally created on.
	//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
	//   3) Registry corruption.
	// you will need to re-register the Add-in by building the NDocBuildSetup project 
	// by right clicking the project in the Solution Explorer, then choosing install.
	#endregion
	
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[GuidAttribute("53493055-FE21-47B0-A582-98613518D776"), ProgId("NDocBuild.Connect")]
	public class Connect : Object, Extensibility.IDTExtensibility2, IDTCommandTarget
	{
		// Application object
		private _DTE applicationObject;
		private AddIn addInInstance;

		// Events
		private EnvDTE.BuildEvents buildEvents;
		private EnvDTE.SolutionEvents solutionEvents;

		private OutputWindowPane buildPane = null;
		private System.Threading.Thread buildThread;
		
		// Paths and filenames
		private string solutionFullName;
		private string solutionPath;
		private string ndocProjectFullName;
		private string documentationFullName;

		/// <summary>
		///		Implements the constructor for the Add-in object.
		///		Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInst, ref System.Array custom)
		{
			try
			{
				// Application object
				applicationObject = (_DTE)application;
				addInInstance = (AddIn)addInInst;

				// Events we're interested in.
				EnvDTE.Events events = applicationObject.Events;
				buildEvents = (EnvDTE.BuildEvents)events.BuildEvents;
				solutionEvents = (EnvDTE.SolutionEvents)events.SolutionEvents;

				buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(this.OnBuildDone);
				solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(this.Opened);

				OutputWindow		outputWindow;

				// Get the IDE's Output Window, Build Pane
				if (buildPane == null)
				{
					outputWindow = (OutputWindow)applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Object;
					buildPane = outputWindow.OutputWindowPanes.Item("Build");
				}

				// Set up some filenames and paths that we're going to need.
				SetPathsAndFilenames();
	
				// Add our add-in commands
				AddCommand ("Build", "NDoc Build Solution Documentation", "Builds MSDN help for C# projects in the solution", "", "Tools");
				AddCommand ("SolutionProperties", "NDoc Edit Solution Properties", "Set NDoc Solution Properties", "", "Tools");
				AddCommand ("View", "NDoc View Solution Documentation", "View NDoc Solution Documentation", "", "Tools");
			}
			catch(Exception e)
			{
				Trace.WriteLine(e.Message);
			}
			
		}

		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
			buildEvents.OnBuildDone -= new _dispBuildEvents_OnBuildDoneEventHandler(this.OnBuildDone);
			solutionEvents.Opened -= new _dispSolutionEvents_OpenedEventHandler(this.Opened);
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnAddInsUpdate(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref System.Array custom)
		{
		}
		
		/// <summary>
		///      Implements the QueryStatus method of the IDTCommandTarget interface.
		///      This is called when the command's availability is updated
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to determine state for.
		/// </param>
		/// <param term='neededText'>
		///		Text that is needed for the command.
		/// </param>
		/// <param term='status'>
		///		The state of the command in the user interface.
		/// </param>
		/// <param term='commandText'>
		///		Text requested by the neededText parameter.
		/// </param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, EnvDTE.vsCommandStatusTextWanted neededText, ref EnvDTE.vsCommandStatus status, ref object commandText)
		{
			if(neededText == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if((commandName == "NDocBuild.Connect.Build") ||
					(commandName == "NDocBuild.Connect.SolutionProperties") ||
					(commandName == "NDocBuild.Connect.View"))
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
				}
			}
		}

		/// <summary>
		///      Implements the Exec method of the IDTCommandTarget interface.
		///      This is called when the command is invoked.
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to execute.
		/// </param>
		/// <param term='executeOption'>
		///		Describes how the command should be run.
		/// </param>
		/// <param term='varIn'>
		///		Parameters passed from the caller to the command handler.
		/// </param>
		/// <param term='varOut'>
		///		Parameters passed from the command handler to the caller.
		/// </param>
		/// <param term='handled'>
		///		Informs the caller if the command was handled or not.
		/// </param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == EnvDTE.vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "NDocBuild.Connect.Build")
				{
					handled = true;
					BuildDocumentation();
					return;
				}
				if(commandName == "NDocBuild.Connect.SolutionProperties")
				{
					handled = true;

					if (ndocProjectFullName == null)
					{
						MessageBox.Show("You must open a solution before you can view or change NDoc solution settings.");
					}
					else
					{
						try
						{
							NDoc.Core.Project	ndocProject = GetNDocProjectWithASD();
							SolutionSettingsForm formSolutionSettings = new SolutionSettingsForm(ndocProject);
							if (formSolutionSettings.ShowDialog() == DialogResult.OK)
							{
								ndocProject.Write(ndocProjectFullName);
							}
						}
						catch(Exception)
						{
							MessageBox.Show("An error occurred in the NDoc Solution Settings form.");
						}
					}

					return;
				}
				if(commandName == "NDocBuild.Connect.View")
				{
					handled = true;

					if (File.Exists(documentationFullName))
					{
						System.Diagnostics.Process.Start(documentationFullName);
					}
					else
					{
						MessageBox.Show("No custom documentation to show.");
					}

					return;
				}
			}
		}


		///////////////////////////////////////////////////////////////////////
		//
		//	Start of event handlers
		//
		///////////////////////////////////////////////////////////////////////

		private void OnBuildDone(EnvDTE.vsBuildScope scope, EnvDTE.vsBuildAction action) 
		{
			if ((scope == EnvDTE.vsBuildScope.vsBuildScopeSolution) &&
				((action == EnvDTE.vsBuildAction.vsBuildActionBuild) ||
				(action == EnvDTE.vsBuildAction.vsBuildActionRebuildAll)))
			{
				// Most people won't want to do this because it's too expensive
				// to do after every build but we might want to provide a setting
				// that they can turn on/off to decide if they want this feature.
				//
				//applicationObject.ExecuteCommand("NDocBuild.Connect.Build", "");
			}
		}

		private void Opened() 
		{
			SetPathsAndFilenames();
		}

		///////////////////////////////////////////////////////////////////////
		//
		//	Start of general support methods
		//
		///////////////////////////////////////////////////////////////////////

		// AddCommand
		// Add one of our commands, dealing with the errors that might result.
		//
		// szName			- name of the command being added
		// szButtonText		- text displayed on menu or button
		// szToolTip		- text displayed in tool tip
		// szKey			- default key assignment, or empty string if none
		// szMenuToAddTo	- default menu to place on, or empty if none
		//
		private void AddCommand (String szName, String szButtonText, String szToolTip, String szKey, String szMenuToAddTo)
		{
			Command cmd = null;
			Object [] GuidArray = {};

			// The IDE identifies commands by their full name, which include
			// the add-in name
			//
			String szNameFull = "NDocBuild.Connect." + szName;

			try 
			{
				cmd = applicationObject.Commands.Item (szNameFull, -1);
			}
			catch(System.ArgumentException /* e */)
			{
				// Thrown if command doesn't already exist.
			}

			if (null != cmd)
			{
				// While in development we delete the command each time in order
				// to ensure any changes we make in development are picked up.
				// Doing this will probably also remove any keybindings. You
				// will probably want to remove this prior to distributing.
				cmd.Delete();
				cmd = null;
			}

			if (null == cmd)
			{
				// Add new command to the IDE
				//
				cmd = applicationObject.Commands.AddNamedCommand (addInInstance
					, szName
					, szButtonText
					, szToolTip
					, false, 1, ref GuidArray, 0);
			}

			if ("" != szKey)
			{
				// a default keybinding specified
				//
				object [] bindings;
				bindings = (object [])cmd.Bindings;
				if (0 >= bindings.Length)
				{
					// there is no pre-existing keybinding, so add our default
					bindings = new object[1];
					bindings[0] = (object)szKey;
					cmd.Bindings = (object)bindings;
				}
			}

			if ("" != szMenuToAddTo)
			{
				// caller specified that this be added to a menu (aka, commandbar).
				//
				CommandBar commandBar = (CommandBar)applicationObject.CommandBars[szMenuToAddTo];
				cmd.AddControl(commandBar, commandBar.Controls.Count + 1);
			}
		}

		private void BuildDocumentation()
		{
			if (solutionFullName == "")
			{
				MessageBox.Show("You must open a solution in order to build NDoc documentation.");
				return;
			}

			try
			{
				NDoc.Core.Project	ndocProject = GetNDocProjectWithASD();
				MsdnDocumenter		msdnDocumenter = new MsdnDocumenter();
				MsdnDocumenter		documenter;

				// Activate the Output window, Build pane
				applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Activate();
				buildPane.Activate();

				buildPane.OutputString("------ Build started: NDoc Add-In ------\n\n");
				buildPane.OutputString(String.Format("Found {0} C# Projects with XML file specified...\n", ndocProject.AssemblySlashDocCount));

				// If we have at least one assembly to document then do it!
				if (ndocProject.AssemblySlashDocCount > 0)
				{
					buildPane.OutputString("Starting creation of MSDN help for CSharpProjects...\n");

					// Set these paths before the build so that the event handler has correct info.
					SetDocumentationPathAndFilename(ndocProject);

					// Allow developers to continue to compile their assemblies while NDoc is running.
					AppDomain.CurrentDomain.SetShadowCopyFiles();

					// Build NDoc MSDN documentation
					documenter = (MsdnDocumenter)ndocProject.GetDocumenter(msdnDocumenter.Name);
					documenter.DocBuildingStep += new DocBuildingEventHandler(OnStepUpdate);
					Build(documenter, ndocProject);
				}
				else
				{
					buildPane.OutputString("No projects were found that could be documented.\n");
				}
			}
			catch(Exception e)
			{
				Trace.WriteLine(e.Message);
			}
		}

		private void Build(MsdnDocumenter documenter, NDoc.Core.Project ndocProject)
		{
			BuildWorker buildWorker = new BuildWorker(documenter, ndocProject);
			buildThread = new System.Threading.Thread(new System.Threading.ThreadStart(buildWorker.ThreadProc));
			buildThread.Name = "NDocBuild";
			buildThread.IsBackground = true;
			buildThread.Priority = System.Threading.ThreadPriority.BelowNormal;

			try
			{
				buildThread.Start();
			}
			finally
			{
			}

			// If no exception occurred during the build, then blow outta here
			Exception ex = buildWorker.Exception;
			if (ex == null)
			{
				return;
			}

			//check if thread has been aborted
			Exception iex = ex;
			do
			{
				if (iex is System.Threading.ThreadAbortException)
				{
					return;
				}
				iex = iex.InnerException;
			} while (iex != null);

			// Process exception
			buildPane.OutputString("An error occured while trying to build the documentation...");
			if (ex is DocumenterException)
			{
				buildPane.OutputString("NDoc Documenter Error:  " + ex.InnerException);
			}
			else
			{
				buildPane.OutputString("Unknown Error:  " + ex.InnerException);
			}
		}

		private void OnStepUpdate(object sender, ProgressArgs e)
		{
			if (e.Status.StartsWith("Generat"))
			{
				buildPane.OutputString(String.Format("{0} (Please be patient, this could take several minutes)\n", e.Status));
			}
			else if (e.Status.StartsWith("Done"))
			{
				buildPane.OutputString(String.Format("{0}\n", e.Status));
				buildPane.OutputString("\nNDoc Build complete.\n\n");
				buildPane.OutputString("\"file://" + documentationFullName + "\"\n");
			}
			else
			{
				buildPane.OutputString(String.Format("{0}\n", e.Status));
			}
		}

		private NDoc.Core.Project GetNDocProjectWithASD()
		{
			NDoc.Core.Project ndocProject = GetNDocProject();
			ArrayList arrayList;

			RemoveAssemblySlashDocs(ndocProject);
			arrayList = GetAssemblySlashDocs();
			foreach (AssemblySlashDoc assemblySlashDoc in arrayList)
			{
				ndocProject.AddAssemblySlashDoc(assemblySlashDoc);
			}

			return ndocProject;
		}

		private NDoc.Core.Project GetNDocProject()
		{
			NDoc.Core.Project ndocProject = new NDoc.Core.Project();
			MsdnDocumenter msdnDocumenter = new MsdnDocumenter();
			XmlDocumenter xmlDocumenter = new XmlDocumenter();
			
			ndocProject.Documenters.Add(msdnDocumenter);
			ndocProject.Documenters.Add(xmlDocumenter);

			// If solution file doesn't exist yet then that's ok.
			try
			{
				ndocProject.Read(ndocProjectFullName);
			}
			catch(Exception){}

			return ndocProject;
		}

		private void RemoveAssemblySlashDocs(NDoc.Core.Project ndocProject)
		{
			for (int i = ndocProject.AssemblySlashDocCount - 1; i >= 0; i--)
			{
				ndocProject.RemoveAssemblySlashDoc(i);
			}
		}

		private ArrayList GetAssemblySlashDocs()
		{
			AssemblySlashDoc	assemblySlashDoc;
			ArrayList			arrayList = new ArrayList();
			string				shortAssemblyFilename;
			string				shortSlashDocFilename;
			string				assemblyFilename;
			string				slashDocFilename;
			string				projectPath;
			string				relativeOutputPath;
			string				outputPath;

			// Walk over the solution gathering up C# projects that have XML documentation turned on.
			foreach (EnvDTE.Project project in (EnvDTE.Projects)applicationObject.GetObject("CSharpProjects"))
			{
				shortSlashDocFilename = project.ConfigurationManager.ActiveConfiguration.Properties.Item("DocumentationFile").Value.ToString();

				if (shortSlashDocFilename != null && shortSlashDocFilename != "")
				{
					projectPath = project.Properties.Item("FullPath").Value.ToString();
					shortAssemblyFilename = project.Properties.Item("OutputFileName").Value.ToString();
					relativeOutputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
					outputPath = Path.Combine(projectPath, relativeOutputPath);
						
					assemblyFilename = Path.Combine(outputPath, shortAssemblyFilename);
					slashDocFilename = Path.Combine(outputPath, shortSlashDocFilename);
					assemblySlashDoc = new AssemblySlashDoc(assemblyFilename, slashDocFilename);

					arrayList.Add(assemblySlashDoc);
				}
			}

			return arrayList;
		}

		private void SetPathsAndFilenames()
		{
			solutionFullName = applicationObject.Solution.FullName;

			if (solutionFullName != "")
			{
				solutionPath = solutionFullName.Substring(0, solutionFullName.LastIndexOf('\\'));
				ndocProjectFullName = solutionFullName.Substring(0, solutionFullName.LastIndexOf('.')) + ".ndoc";

				// This call needs to occur after setting ndocProjectFullName because it uses the ndoc project to build
				// it's paths.
				SetDocumentationPathAndFilename(GetNDocProject());
			}
		}

		private void SetDocumentationPathAndFilename(NDoc.Core.Project ndocProject)
		{
			MsdnDocumenter		msdnDocumenter = new MsdnDocumenter();
			MsdnDocumenter		documenter;
			string				configDocFilename;
			string				configDocOutputDir;

			documenter = (MsdnDocumenter)ndocProject.GetDocumenter(msdnDocumenter.Name);
			configDocFilename = ((MsdnDocumenterConfig)(documenter.Config)).HtmlHelpName + ".chm";
			configDocOutputDir = ((MsdnDocumenterConfig)(documenter.Config)).OutputDirectory;

			// Need to make the relative path go off of the solution and not the devenv.exe directory.
			if (configDocOutputDir.StartsWith(".")  ||
				(configDocOutputDir.StartsWith(@"\") && !(configDocOutputDir.StartsWith(@"\\"))))
			{
				documentationFullName = Path.Combine(solutionPath, configDocOutputDir);
			}
			else
			{
				documentationFullName = configDocOutputDir;
			}

			documentationFullName += configDocFilename;
		}
	}
}