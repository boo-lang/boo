import System.Xml
import System.IO
import System.Resources
import System.Windows.Forms
import Useful.IO from Boo.Lang.Useful

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
		_document.Save(AbsolutePath)
		
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
		
	def ResetSourceFiles():
		_sourceFiles.RemoveAll()

	def AddSourceFile(item as string):
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
	project.ResetSourceFiles()

	baseURI = project.BaseURI
	for item as string in listFiles(Path.GetDirectoryName(baseURI.AbsolutePath)):
		if item =~ /\.svn/: continue
		if not item.EndsWith(".cs"): continue
		uri = baseURI.MakeRelative(System.Uri(item))
		project.AddSourceFile(uri)
	project.Save()

	print project.AbsolutePath


def updateStringResources(txtFile as string):
	fname = Path.ChangeExtension(txtFile, ".resx")
	File.Delete(fname)

	using resourceFile = ResXResourceWriter(fname):

		using file=File.OpenText(txtFile):
			for line in file:
				if line.StartsWith(";"): continue
				line = line.Trim()
				if len(line) == 0: continue
				index = line.IndexOf('=')
				key = line[:index]
				value = line[index+1:]

				resourceFile.AddResource(key, value)

	print fname

def rebase(fname as string):
	#return Path.Combine("c:/projects/boo/", fname)
	return fname

fnames = (
"src/Boo.Lang/Boo.Lang-VS2005.csproj",
"src/Boo.Lang.Parser/Boo.Lang.Parser-VS2005.csproj",
"src/Boo.Lang.Compiler/Boo.Lang.Compiler-VS2005.csproj",
#"src/Boo.Lang.Ast/Boo.Lang.Ast-VS2005.csproj",
"tests/BooCompiler.Tests/BooCompiler.Tests-VS2005.csproj",

"src/Boo.Lang.Compiler/Boo.Lang.Compiler.mdp",
)
for fname in fnames:
	updateProjectFile(rebase(fname))

updateStringResources(rebase("src/Boo.Lang/Resources/strings.txt"))
