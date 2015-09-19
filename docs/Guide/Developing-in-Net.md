These are pages with general help on developing applications for .NET and Mono, and other topics not specific to boo itself but useful for a boo developer.

### General Info about .NET and Mono

.NET is Microsoft's application framework and runtime engine. Most people using Windows XP have .NET version 1.1, but the 2.0 version was released in the fall of 2005. Boo now supports .NET 4.0.

There is also an open source clone of .NET called [Mono](http://www.mono-project.com/Main_Page), that runs on Linux, Mac OS X, and Windows. It is derived from the open [Common Language Infrastructure](http://www.ecma-international.org/publications/standards/Ecma-335.htm) (CLI) specification used by .NET.

See also these resources:

* The [.NET Development](http://msdn.microsoft.com/en-us/library/ff361664.aspx) page at MSDN
* [Inside the .NET Framework](http://msdn.microsoft.com/en-us/library/a4t23ktk%28v=vs.71%29.aspx)
* [.NET Class Library](http://msdn.microsoft.com/en-us/library/gg145045.aspx)
* [The Code Project](http://www.codeproject.com/kb/dotnet/) - samples and tutorials

Below are some resources for developing .NET/Mono applications (primarily the open source and free resources), as well as tutorials.

### IDEs - Integrated Development Environments

* ![lightbulb](http://docs.codehaus.org/s/en_GB/3278/15/_/images/icons/emoticons/lightbulb_on.png)[SharpDevelop](http://www.icsharpcode.net/OpenSource/SD/) - free, open source, includes a forms designer/GUI builder. Windows only. SharpDevelop supports boo fully.
* [MonoDevelop ](http://monodevelop.com/)- Works with Mono on Linux or Mac. Peter Johanson has created a boo add-in for MonoDevelop.
* [Nant ](http://sourceforge.net/projects/nant/)- an XML-based build tool based on Java's ant build tool. Used to build boo itself.
* Visual Studio 2010 (and there is a free Express version of VS 2010 too). The standard commercial IDE used on Windows. People are interested in creating a boo plugin for VS.
* [Monolipse ](http://sourceforge.net/projects/monolipse/)There is a boo addin for Eclipse called monolipse. monolipse originated as another project named booclipse but has been expanded to support other EMCA languages such as C#.
* [SciBoo ](http://home.mweb.co.za/sd/sdonovan/sciboo.html)- by Steve Donovan, a ScintillaNET based editor.
* [Smulton ](http://www.peterborgapps.com/smultron/)- by Peter Borg. A Mac OS X text editor. An [add-in](http://docs.codehaus.org/download/attachments/19601/boo.plist) for Boo support is authored by Grant Morgan from the Unity3d.com team. With the Smultron command window you can automatically execute your boo code without leaving the editor.

### Web Development

See the [ASP.NET](http://docs.codehaus.org/display/BOO/ASP.NET) page and tutorials.

ASP.NET is the standard, but see also:
* [Castle ](http://www.castleproject.org/projects/)and its MonoRail and Brail projects
* [Maverick.NET](http://mavnet.sourceforge.net/)

### Developing Desktop GUI Applications

**Windows.Form**s

The standard Windows API, but you can call it from Linux as well, using Mono's Managed.Windows.Forms project. Here are some code snippets and tutorials on using Windows.Forms:

* [Windows Forms Quickstart tutorial](http://msdn.microsoft.com/en-us/library/aa308989%28v=vs.71%29.aspx) - tutorials and code snippets for using every type of control
* [Code for Windows Forms Controls](http://msdn.microsoft.com/en-us/library/aa984065%28v=vs.71%29.aspx) - also has examples of using just about every control
* [Windows Forms FAQ](http://www.syncfusion.com/FAQ/WinForms/default.asp)
* [Using .NET Windows Forms Controls](http://www.informit.com/articles/printerfriendly.asp?p=414984&rl=1) - article going over basic usage of each of the main form controls

The easiest way to create windows forms with boo is to use SharpDevelop. It has a form designer with support for boo, if you use the new version. If you use an older version or another C# form designer, you can convert the C# generated code to boo automatically.

**GTK#**  
[GTK#](http://www.mono-project.com/GtkSharp) is the GUI API used in the GNOME project for Linux, but you can use it on Windows and Mac OS X too, see these articles:
* We have a [tutorial ](https://github.com/bamboo/boo/wiki/Sample-GTK-SHARP-application)on creating a sample GTK# application with the Glade GUI designer.
* [Cross platform GUI comes to .NET](http://www.oreillynet.com/pub/wlg/5390)
* [How to Write a Basic Gtk# Program with Mono](http://www.onlamp.com/pub/a/onlamp/excerpt/MonoTDN_chap1/?page=last&x-showcontent=text&x-maxdepth=0)

**GTK# on Windows**  
There are two versions of GTK# that run on Windows.
* One runs in .NET 1.1 (here is the [installer](http://www.mono-project.com/Gtk-Sharp_Installer_for_.NET_Framework))
* the other runs in Mono, and is included in the combined [Mono installer](http://www.mono-project.com/Downloads) for Windows.

Note though, to compile apps that use the GTK# runtime for .NET 1.1, you may need to download the [.NET 1.1 SDK](http://www.microsoft.com/downloads/details.aspx?familyid=9b3a2ca6-3647-4070-9f41-a333c6b9181d&displaylang=en) to get them to compile.

Another item now available on Windows is the [Gecko Runtime Engine (GRE)] (https://developer.mozilla.org/en/docs/GRE). This lets you embed the Mozilla browser engine in your GTK# app as a WebControl.

**See also**
WX.NET and QT#

### Databases

* [ADO.NET](http://docs.codehaus.org/display/BOO/ADO.NET) - the standard API for working with relational databases, see also [Database Recipes](http://docs.codehaus.org/display/BOO/Database+Recipes) and [Database Design](http://docs.codehaus.org/display/BOO/Database+Design).
* Most relational database engines have interfaces to .NET/Mono, such as SQLite, MySQL, MS SQL Server, MS Access, Firebird...
* The [Database Recipes](http://docs.codehaus.org/display/BOO/Database+Recipes) page also shows examples of non-SQL-based database engines such as db4o.

### Math / Statistics

* [dnAnalytics](http://www.dnanalytics.net/)
* [Math.NET](http://www.mathdotnet.com/)
* Sharp3D.Math

### Graphics

* Generate PDF reports
* * [SharpPDF](http://sourceforge.net/projects/sharppdf/)
* * [Report.NET](http://report.sourceforge.net/)
* Structured Graphics
* * [Piccolo.NET](http://www.cs.umd.edu/hcil/piccolo/)
* * SVG#
* * Diagram.NET
* Plotting / Charts
* * [NPlot](http://sourceforge.net/projects/nplot/)
* * [ZedGraph](http://zedgraph.sourceforge.net/)
* Graph Libs / Graph Drawing
* * [QuickGraph](http://www.codeproject.com/cs/miscctrl/quickgraph.asp)
* * [Netron](http://sourceforge.net/projects/netron/)
* * [DiaCanvas#](http://diacanvas.sourceforge.net/csharp.php)

### 3D, Game Development

* For OpenGL and SDL, use:
* * [Tao Framework](http://www.mono-project.com/Tao) - for examples see [OpenGL 3D Samples](http://docs.codehaus.org/display/BOO/Opengl+3D+Samples).
* * [SDL.NET](http://cs-sdl.sourceforge.net/)

* For DirectX, you need to download the DirectX SDK from Microsoft and use the Managed DirectX API. See:
* * [Managed DirectX Resources](http://www.chadvernon.com/blog/resources/managed-directx-2/)
* * [Managed World](http://geekswithblogs.net/jolson/Default.aspx)

* Game Engines
* * [RealmForge ](http://realmforge.com/)- OpenGL and DirectX can be used interchangeably
* * [Irrlicht ](http://irrlicht.sourceforge.net/)- has .NET bindings, can also use OpenGl or DirectX
* * [Ovorp ](http://sourceforge.net/projects/ovorp/)- 2D game engine

* Physics
* * [ODE.NET](http://odedotnet.sourceforge.net/)

Please add any game programming tips or samples you have to this site or the [boo page](http://content.gpwiki.org/index.php/Boo) at the game programming wiki.

### Others

* [IndyProject ](http://www.indyproject.org/)- networking libs for doing ftp, soap, http, smtp, pop, nntp, etc.
* Netspell

---

See also: [GnomeFiles ](http://gnomefiles.org/)for some Mono apps and libs, [CSharp-Source](http://csharp-source.net/) for open source .NET libraries and apps, and [CodeProject ](http://www.codeproject.com/)for many useful .NET tutorials and code samples.
