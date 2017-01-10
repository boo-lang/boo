// Console.cs - a console application for NDoc
// Copyright (C) 2001  Jason Diamond
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NDoc.Core;

namespace NDoc.ConsoleApplication
{
	class EntryPoint
	{
		private static Project project;
		private static IDocumenterConfig documenterConfig;
		private static DateTime startDateTime;

		public static int Main(string[] args)
		{
			try
			{
				WriteLogoBanner();

				project = new Project();
				IDocumenterInfo info = InstalledDocumenters.GetDocumenter("MSDN");
				if (info == null)
				{
					//MSDN documenterConfig not found, pick the first one available.
					if (InstalledDocumenters.Documenters.Count > 0)
					{
						info = (IDocumenterInfo)InstalledDocumenters.Documenters[0];
					}
					else
					{
						throw new ApplicationException("Could not find any documenter assemblies.");
					}
				}
				project.ActiveDocumenter = info;
				documenterConfig = project.ActiveConfig;

				int maxDepth = 20; //to limit recursion depth
				bool propertiesSet = false;
				bool projectSet = false;

				if (args.Length==0)
				{
					WriteUsage();
					return 1;
				}

				if (args[0].ToLower().StartsWith("-help"))
				{
					WriteHelp(args);
					return 1;
				}

				foreach (string arg in args)
				{
					if (arg.StartsWith("-"))
					{
						if (string.Compare(arg, "-verbose", true) == 0)
						{
							Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
						}
						else
						{
							string[] pair = arg.Split('=');

							if (pair.Length == 2)
							{
								string name = pair[0].Substring(1);
								string val = pair[1];

								switch (name.ToLower())
								{
									case "documenter":
										if (propertiesSet)
										{
											throw new ApplicationException("The documenter name must be specified before any documenter specific options.");
										}
										if (projectSet)
										{
											throw new ApplicationException("The documenter name must be specified before the project file.");
										}
										info = InstalledDocumenters.GetDocumenter(val.Replace("_"," "));

										if (info == null)
										{
											throw new ApplicationException("The specified documenter name is invalid.");
										}
										project.ActiveDocumenter = info;
										documenterConfig = project.ActiveConfig;
										break;
									case "project":
										if (propertiesSet)
										{
											throw new ApplicationException("The project file must be specified before any documenter specific options.");
										}
										Console.WriteLine("using project file " + val);
										project.Read(val);
										project.ActiveDocumenter = info;
										documenterConfig = project.ActiveConfig;
										projectSet = true;
										Directory.SetCurrentDirectory(Path.GetDirectoryName(val));
										Debug.WriteLine(Directory.GetCurrentDirectory());
										break;
									case "recurse":
										string[] recPair = val.Split(',');
										if (2 == recPair.Length)
										{
											maxDepth = Convert.ToInt32(recPair[1]);
										}
										RecurseDir(recPair[0], maxDepth);
										break;
									case "namespacesummaries":
										using(StreamReader streamReader = new StreamReader(val))
										{
											XmlTextReader reader = new XmlTextReader(streamReader);
											reader.MoveToContent();
											project.Namespaces.Read(reader);
											reader.Close();
											streamReader.Close();
										}
										break;
									case "referencepath":
										project.ReferencePaths.Add(new ReferencePath(val));
										break;
									default:
										documenterConfig.SetValue(name, val);
										propertiesSet = true;
										break;
								}
							}
						}
					}
					else if (arg.IndexOf(',') != -1)
					{
						string[] pair = arg.Split(',');

						if (pair.Length == 2)
						{
							project.AssemblySlashDocs.Add(
								new AssemblySlashDoc(pair[0], pair[1]));
						}
					}
					else
					{
						string doc = Path.ChangeExtension(arg, ".xml");
						if (File.Exists(doc))
						{
							project.AssemblySlashDocs.Add(
								new AssemblySlashDoc(arg, doc));
						}
						else
						{
							project.AssemblySlashDocs.Add(
								new AssemblySlashDoc(arg, ""));
						}
					}
				}

				if (project.AssemblySlashDocs.Count == 0)
				{
					Console.WriteLine("[Error] Build cannot proceed; No assemblies were specified, or none could be found.");
					//WriteUsage();
					return 1;
				}
				else
				{
					startDateTime = DateTime.UtcNow;
					IDocumenter documenter = documenterConfig.CreateDocumenter();
					documenter.DocBuildingStep += new DocBuildingEventHandler(DocBuildingStepHandler);
					documenter.Build(project);
					TimeSpan ts = DateTime.UtcNow - startDateTime;
					Console.WriteLine(String.Format("Total build time {0:f1} s", ts.TotalSeconds));
					return 0;
				}

			}
			catch( Exception except )
			{
				string errorText= BuildExceptionText(except);
				Console.WriteLine(errorText);
				System.Diagnostics.Trace.WriteLine(errorText);
				return 2;
			}
		}

		private static void WriteUsage()
		{
			Console.WriteLine();
			Console.WriteLine("usage: NDocConsole  assembly[,xmldoc] [assembly[,xmldoc]]...");
			Console.WriteLine("                    [[-referencepath=dir] [-referencepath=dir]...]");
			Console.WriteLine("                    [-namespacesummaries=filename]");
			Console.WriteLine("                    [-documenter=documenter_name]");
			Console.WriteLine("                    [[-property=value] [-property=value]...]");
			Console.WriteLine("                    [-verbose]");
			Console.WriteLine();
			Console.WriteLine("or     NDocConsole  -recurse=dir[,maxDepth]");
			Console.WriteLine("                    [[-referencepath=dir] [-referencepath=dir]...]");
			Console.WriteLine("                    [-namespacesummaries=filename]");
			Console.WriteLine("                    [-documenter=documenter_name]");
			Console.WriteLine("                    [[-property=value] [-property=value]...]");
			Console.WriteLine("                    [-verbose]");
			Console.WriteLine();
			Console.WriteLine("or     NDocConsole  [-documenter=documenter_name] -project=ndocfile [-verbose]");
			Console.WriteLine();
			Console.WriteLine("or     NDocConsole  [-help] [documenter_name [property_name]]");
			Console.WriteLine();
			Console.WriteLine();

			WriteHelpAvailableDocumenters();

			Console.WriteLine();
			Console.WriteLine(@"namespace summaries file syntax:");
			Console.WriteLine(@"    <namespaces>");
			Console.WriteLine(@"        <namespace name=""My.NameSpace"">My summary.</namespace>");
			Console.WriteLine(@"        ...");
			Console.WriteLine(@"    </namespaces>");

		}

		private static  void WriteHelp(string[] args)
		{
			if (args.Length==1)
			{
				WriteUsage();
				return;
			}

			if (args.Length>1)
			{
				IDocumenterInfo info = InstalledDocumenters.GetDocumenter(args[1].Replace("_"," "));
				if (info == null)
				{
					WriteHelpAvailableDocumenters();
					return;
				}

				IDocumenterConfig documenterConfig = info.CreateConfig( project );
				if (args.Length==2)
				{
					WriteHelpAvailableDocParameters(documenterConfig);
				}
				else
				{
					WriteHelpDocParameter(documenterConfig,args[2]);
				}
			}

		}

		private static void WriteHelpAvailableDocumenters()
		{
			Console.Write("available documenters: ");
			ArrayList docs = InstalledDocumenters.Documenters;
			for (int i = 0; i < docs.Count; i++)
			{
				if (i > 0) Console.Write(", ");
				Console.Write(((IDocumenterInfo)docs[i]).Name.Replace(" ","_"));
			}
			Console.WriteLine();
		}

		private static void WriteHelpAvailableDocParameters(IDocumenterConfig documenterConfig)
		{
			Console.WriteLine("available properties with the {0} documenterConfig:", documenterConfig.DocumenterInfo.Name);
			foreach (PropertyInfo property in documenterConfig.GetProperties())
			{
				if (!property.IsDefined(typeof(NonPersistedAttribute),true))
				{
					Console.WriteLine("    " + property.Name);
				}
			}
		}

		private static void WriteHelpDocParameter(IDocumenterConfig documenterConfig,string propertyName)
		{
			PropertyInfo foundProperty=null;

			foreach (PropertyInfo property in documenterConfig.GetProperties())
			{
				if (string.Compare(property.Name, propertyName, true) == 0)
				{
					foundProperty=property;
					break;
				}
			}

			if (foundProperty==null)
			{
				Console.WriteLine("{0} is not a property of the {1} documenterConfig...", propertyName, documenterConfig.DocumenterInfo.Name);
				Console.WriteLine("");
				WriteHelpAvailableDocParameters(documenterConfig);
			}
			else
			{
				WriteHelpPropertyDetails(foundProperty);
			}
		}

		private static void WriteHelpPropertyDetails(PropertyInfo property)
		{
			Console.WriteLine(property.Name);
			Console.WriteLine("");

			object[] descAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute),true);
			if (descAttr.Length>0)
			{
				Console.WriteLine("  Description:");
				Console.WriteLine("    " + ((System.ComponentModel.DescriptionAttribute)descAttr[0]).Description);
				Console.WriteLine();
			}

			if (property.PropertyType.IsSubclassOf(typeof(Enum)))
			{
				Console.WriteLine("  Possible Values:");
				string[] enumValues= Enum.GetNames(property.PropertyType);
				foreach(string enumValue in enumValues)
				{
					Console.WriteLine("    " + enumValue);
				}
				Console.WriteLine();
			}

			object[] defaultAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DefaultValueAttribute),true);
			if (defaultAttr.Length>0)
			{
				Console.WriteLine("  Default Value:");
				Console.WriteLine("    " + ((System.ComponentModel.DefaultValueAttribute)defaultAttr[0]).Value.ToString());
				Console.WriteLine();
			}
		}

		private static void WriteLogoBanner() 
		{
			string productName;
			string informationalVersion;
			Version assemblyVersion;
			string configurationInformation = null;
			string copyrightInformation = null;
			string companyInformation = null;
			DateTime buildDate;

			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly == null) {
				assembly = Assembly.GetCallingAssembly();
			}

			// get product name
			object[] productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			if (productAttributes.Length > 0) {
				AssemblyProductAttribute productAttribute = (AssemblyProductAttribute) productAttributes[0];
				productName = productAttribute.Product;
			} else {
				productName = assembly.GetName().Name;
			}

			// get informational version
			object[] informationalVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
			if (informationalVersionAttributes.Length > 0) {
				AssemblyInformationalVersionAttribute informationalVersionAttribute = (AssemblyInformationalVersionAttribute) informationalVersionAttributes[0];
				informationalVersion = informationalVersionAttribute.InformationalVersion;
			} else {
				FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
				informationalVersion = info.FileMajorPart + "." + info.FileMinorPart;
			}

			// get assembly version 
			assemblyVersion = assembly.GetName().Version;

			// determine build date using build number of assembly 
			// version (specified as number of days passed since 1/1/2000)
			buildDate = new DateTime(2000, 1, 1).AddDays(assemblyVersion.Build);

			// get configuration information
			object[] configurationAttributes = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
			if (configurationAttributes.Length > 0) {
				AssemblyConfigurationAttribute configurationAttribute = (AssemblyConfigurationAttribute) configurationAttributes[0];
				configurationInformation = configurationAttribute.Configuration;
			}

			// get copyright information
			object[] copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			if (copyrightAttributes.Length > 0) {
				AssemblyCopyrightAttribute copyrightAttribute = (AssemblyCopyrightAttribute) copyrightAttributes[0];
				copyrightInformation = copyrightAttribute.Copyright;
			}

			// get company information
			object[] companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			if (companyAttributes.Length > 0) {
				AssemblyCompanyAttribute companyAttribute = (AssemblyCompanyAttribute) companyAttributes[0];
				companyInformation = companyAttribute.Company;
			}

			StringBuilder logoBanner = new StringBuilder();

			logoBanner.AppendFormat(CultureInfo.InvariantCulture,
				"{0} {1} (Build {2}; {3}; {4})", productName, 
				informationalVersion, assemblyVersion.ToString(4),
				configurationInformation, buildDate.ToShortDateString()); 
			logoBanner.Append(Environment.NewLine);

			// output copyright information
			if (copyrightInformation != null && copyrightInformation.Length != 0) {
				logoBanner.Append(copyrightInformation);
				logoBanner.Append(Environment.NewLine);
			}

			// output company information
			if (companyInformation != null && companyInformation.Length != 0) {
				logoBanner.Append(companyInformation);
				logoBanner.Append(Environment.NewLine);
			}

			Console.WriteLine(logoBanner.ToString());
		}

		private static DateTime lastStepDateTime;

		private static void DocBuildingStepHandler(object sender, ProgressArgs e)
		{
			// timing
			if (lastStepDateTime.Ticks > 0)
			{
				TimeSpan ts = DateTime.UtcNow - lastStepDateTime;
				Console.WriteLine(String.Format("	Last step took {0:f1} s", ts.TotalSeconds));
			}
			lastStepDateTime = DateTime.UtcNow;

			Console.WriteLine( e.Status );
		}

		private static void RecurseDir(string dirName, int maxDepth)
		{
			if (0 == maxDepth) return;
			string docFile;
			string[] extensions = {"*.dll", "*.exe"};
			foreach (string extension in extensions)
			{
				foreach (string file in System.IO.Directory.GetFiles(dirName, extension))
				{
					docFile = Path.ChangeExtension(file, ".xml");
					if (System.IO.File.Exists(docFile))
					{
						project.AssemblySlashDocs.Add(new AssemblySlashDoc(file, docFile));
					}
					else
					{
						Console.WriteLine("[Warning] No XML doc file found for '" + file + "'");
						AssemblySlashDoc assemblySlashDoc=new AssemblySlashDoc();
						assemblySlashDoc.Assembly.Path=file;
						project.AssemblySlashDocs.Add(assemblySlashDoc);
					}
				}
			}
			foreach (string subDir in System.IO.Directory.GetDirectories(dirName))
			{
				RecurseDir(subDir, maxDepth - 1);
			}
		}

		private static string BuildExceptionText(Exception ex)
		{
			StringBuilder strBld = new StringBuilder();

			Exception tmpEx;
			tmpEx= ex;
			while (tmpEx != null)
			{
				strBld.AppendFormat("Error: {0}", tmpEx.GetType().ToString());
				strBld.Append(Environment.NewLine);
				strBld.Append(tmpEx.Message);
				strBld.Append(Environment.NewLine);
				tmpEx = tmpEx.InnerException;
			}
			strBld.Append(Environment.NewLine);

			ReflectionTypeLoadException rtle = ex as ReflectionTypeLoadException;
			if (rtle != null)
			{
				Hashtable fileLoadExceptions = new Hashtable();
				foreach(Exception loaderEx in rtle.LoaderExceptions)
				{
					System.IO.FileLoadException fileLoadEx = loaderEx as System.IO.FileLoadException;
					if (fileLoadEx !=null)
					{
						if (!fileLoadExceptions.ContainsKey(fileLoadEx.FileName))
						{
							fileLoadExceptions.Add(fileLoadEx.FileName, null);
							strBld.Append("Unable to load: " + fileLoadEx.FileName + Environment.NewLine);
						}
					}
//					else
//					{
						strBld.Append(loaderEx.Message + Environment.NewLine);
						strBld.Append(loaderEx.StackTrace + Environment.NewLine);
					//					}
				}
			}

			strBld.Append(ex.StackTrace);

			return strBld.ToString().Replace("\r\n","\n").Replace("\r","\n").Replace("\n","\r\n");
		}
	}
}
