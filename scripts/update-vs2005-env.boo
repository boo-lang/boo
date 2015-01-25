import System.Xml
import System.IO
import System.Resources from System.Windows.Forms
import Boo.Lang.Useful.IO from Boo.Lang.Useful

class XmlDocumentWrapper:

	_document as XmlDocument

	def constructor(document as XmlDocument):
		_document = document

	def constructor(fname as string):
		_document = XmlDocument()
		_document.Load(fname)

	NamespaceURI:
		get:
			xmlns = _document.DocumentElement.Attributes["xmlns"]
			if xmlns is null: return null
			return xmlns.Value

	BaseURI:
		get:
			return System.Uri(_document.BaseURI)

	AbsolutePath:
		get:
			return BaseURI.AbsolutePath

	def Save():
		_document.PreserveWhitespace = false
		_document.Save(AbsolutePath)
		# Hack: Mono doesn't obey XML formatting options, so we just handle the text
		contents = File.ReadAllLines(AbsolutePath)
		File.WriteAllText(AbsolutePath, join(contents, "\r\n"))

class Project(XmlDocumentWrapper):

	static def Load(fname as string):
		doc = XmlDocument()
		doc.Load(fname)
		if fname.EndsWith(".mdp"):
			return MDProject(doc)
		return VS2005Project(doc)

	_sourceFiles as XmlElement
	
	def constructor(document as XmlDocument, sourceFiles as XmlElement):
		super(document)
		_sourceFiles = sourceFiles

	def SetSourceFiles(files as string*):
		_sourceFiles.RemoveAll()
		for item in List of string(files).Sort():
			_sourceFiles.AppendChild(CreateSourceElement(item))

	protected abstract def CreateSourceElement(src as string) as XmlElement:
		pass

class MDProject(Project):
	def constructor(document as XmlDocument):
		super(document, document.SelectSingleNode("/Project/Contents"))
		
	override protected def CreateSourceElement(src as string):
		file = _document.CreateElement("File", NamespaceURI)
		file.SetAttribute("name", src)
		file.SetAttribute("buildaction", "Compile")
		file.SetAttribute("subtype", "Code")
		return file

class VS2005Project(Project):

	def constructor(document as XmlDocument):
		super(document, selectCompileItemGroup(document))
		
	private def selectCompileItemGroup(document as XmlDocument):
		compileItem = document.SelectSingleNode("//*[local-name()='ItemGroup']/*[local-name()='Compile']")
		return compileItem.ParentNode

	override protected def CreateSourceElement(src as string):
		compile = _document.CreateElement("Compile", NamespaceURI)
		compile.SetAttribute("Include", src.Replace('/', '\\'))
		return compile

def updateProjectFile(fname as string):
	
	project = Project.Load(fname)		
	project.SetSourceFiles(sourceFilesFor(project))
	project.Save()

	print project.AbsolutePath
	
def sourceFilesFor(project as Project):
	baseURI = project.BaseURI
	for item as string in listFiles(Path.GetDirectoryName(baseURI.AbsolutePath)):
		if item =~ /\.svn/: continue
		if not item.EndsWith(".cs"): continue
		if item =~ /\/\\(bin|obj)\/\\/: continue 
		yield baseURI.MakeRelativeUri(System.Uri(item)).ToString()


def rebase(fname as string):
	#return Path.Combine("c:/projects/boo/", fname)
	return fname

fnames = (
"src/Boo.Lang/Boo.Lang.csproj",
"src/Boo.Lang.Parser/Boo.Lang.Parser.csproj",
"src/Boo.Lang.Compiler/Boo.Lang.Compiler.csproj",
"tests/BooCompiler.Tests/BooCompiler.Tests.csproj",
"tests/Boo.Lang.Runtime.Tests/Boo.Lang.Runtime.Tests.csproj",
)
for fname in fnames:
	updateProjectFile(rebase(fname))
