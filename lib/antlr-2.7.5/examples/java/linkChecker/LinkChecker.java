import java.io.*;
import antlr.*;
import java.util.Hashtable;

class LinkChecker implements LinkListener {
  /** Which directory is the document in? */
  private String directory = "."; // default to current dir
  /** Which document are we to process? */
  private String document;

  /** Record which files we have seen so that we don't get into an
   *  infinite loop and for efficiency.  The absolute path is stored here
   *  to uniquely identify the files.  That is, a file can be arrived
   *  at from many different locations such as help.html from .
   *  and ../help.html from a directory below.
   *
   *  This table is shared by all instances of LinkChecker.
   */
  private static Hashtable visited = new Hashtable(100);

  /** A table of the images visited by any document; a cache of correctness */
  private static Hashtable imgVisited = new Hashtable(100);

  private static int recursionDepth = 0;
  private static final String separator = "/"; // not OS sensitive in HTML
  private static final String localSeparator =
	  System.getProperty("file.separator");

  
  public LinkChecker(String document) {
	this.document = document;
	this.directory = pathMinusFile(document);
  }  
public boolean checkLinkRules(String fName, int line) {
	// Check case of path (check for UNIX compatibility on a PC)!
	String offensive = offensivePathMember(directory + separator + fName);
	if (offensive != null) {
		String file="";
		try {
			File f = new File(offensive);
			file = f.getCanonicalPath();
			error("Case mismatch in reference " + fName + ":"+
					System.getProperty("line.separator")+"\treal name is "+
					fileMinusPathLocal(file)+System.getProperty("line.separator")+
					"\treal absolute path is "+file, line);
			return false;
		}
		catch (IOException io) {
			error("internal error: cannot get canonical name for "+offensive, line);
		}
	}
	if (new File(fName).isAbsolute()) {
		error("Reference to " + fName + " with absolute path", line);
		return false;
	}
	return true;
}
public void doCheck() throws IOException {
	if ( !document.endsWith(".html") ) {
		return;
	}

	// prevent infinite recursion to this file
	if (visited(document)) {
		return;
	}
	visit(document);
	recursionDepth++;
	FileReader f = new FileReader(document);
	LinkExtractor lexer = new LinkExtractor(f);
	lexer.addLinkListener(this);
	// this will parse whole file since all tokens are skipped
  try {
    lexer.nextToken();
  }
  catch (antlr.TokenStreamException e) {
    error("internal error:" + e,1);
  }
	recursionDepth--;
}
  public void error(String err, int line) {
	String d="<internal error>";
	try {
		File f = new File(document);
		d = f.getCanonicalPath();
	}
	catch (IOException io) {
		System.err.println("internal error: cannot find file that has error");
		System.exit(0);
	}
	System.err.println(d+":"+line+":"+System.getProperty("line.separator")+"\t"+err);
  }  
  public static boolean fileAbsolute(String path) {
	return path.startsWith("/") || path.charAt(1)==':';
  }  
  /** Return file from end of HTML path; i.e., use '/' separator */
  public static String fileMinusPath(String f) {
	int endOfPath = f.lastIndexOf(separator);
	if ( endOfPath == -1 ) {
	  return f;	// no path found
	}	
	return f.substring(endOfPath+1);
  }    
  /** Return file from end of locally correct path; i.e., use '/' or '\' separator */
  public static String fileMinusPathLocal(String f) {
	int endOfPath = f.lastIndexOf(localSeparator);
	if ( endOfPath == -1 ) {
	  return f;	// no path found
	}	
	return f.substring(endOfPath+1);
  }        
  public static boolean fileProtocolURL(String target) {
	return target.indexOf("://") == -1 &&
		!target.startsWith("mailto:") &&
		!target.startsWith("news:");
  }    
  public static String getParent(String path) {
	int index = path.lastIndexOf(separator);
	if (index < 0) {
	  return null;
	}
	if ( !fileAbsolute(path) || path.indexOf(separator) != index ) {
	  return path.substring(0, index);
	}
	if (index < path.length() - 1) {
	  return path.substring(0, index + 1);
	}
	return null;
  }  
public void hrefReference(String target, int line) {
	// System.out.println(document+":"+line+": href to "+target);
	// recursively check the target document unless non-file ref
	if (fileProtocolURL(target)) {
		// prune off any #name reference on end of file
		int pound = target.indexOf('#');
		String path = target;
		if (pound != -1) {
			path = target.substring(0, pound); // rip off #name on end, leave file
			if (path.length() == 0) {
				return; // ref to name in this file
			}
		}

		// first check existence on disk
		File f = new File(directory + separator + path);
		if (!f.exists()) {
			error("Reference to missing file " + path, line);
			return;
		}

		// check the case
		checkLinkRules(path, line);

		try {
			// Link is ok, now follow the link
			LinkChecker chk = new LinkChecker(directory + separator + path);
			chk.doCheck();
		} catch (IOException io) {
			error("Document does not exist: " + target, line);
		}
	}
}
  public static boolean imageLinkIsOk(String file) throws IOException {
	File f = new File(file);
	file = f.getCanonicalPath();
	Boolean b = (Boolean)imgVisited.get(file);
	if ( b!=null ) {
		return b.booleanValue();
	}
	return false;
  }            
public void imageReference(String imageFileName, int line) {
	// first check if we have seen this exact file
	try {
		if (imageLinkIsOk(directory+separator+imageFileName)) {
			return;
		}
		File f = new File(directory + separator + imageFileName);
		if (!f.exists()) {
			error("Reference to missing file " + imageFileName, line);
			return;
		}
		if (checkLinkRules(imageFileName, line)) {
			visitImage(directory+separator+imageFileName);
		}
	} catch (IOException io) {
		if (!(io instanceof FileNotFoundException)) {
			System.err.println("internal error: " + io.getMessage());
		}
	}
}
/** Given a path to a file or dir, is the case of the reference
   *  the same as the actual path on the disk?  This is only
   *  meaningful on a PC which is case-insensitive (not a real
   *  file system).
   *
   *  Returns null if there is nothing offensive and the file exists.
   *  Returns offending file/dir if it does not exist or
   *  it has there is a case mismatch for it.  The last file is checked
   *  first followed by the parent directory, recursively, all the way
   *  to the absolute or relative path root in that String; i.e., we parse
   *  from right to left.
   *
   *  Because the File object won't actually go get the real filename
   *  from the disk so we can compare, we must get a directory listing
   *  of the directory and then look for the referenced file or dir.
   *  For example, for "./images/logo.gif" we would check "./images" dir
   *  listing for "logo.gif" with the appropriate case and then check
   *  directory "." for a dir called images with the right case.  When
   *  no parent exists, we can stop looking for case problems.
   */
public static String offensivePathMember(String fName) {
	// System.out.println("caseMismatch(" + fName + ")");
	// have we reached the root? (stopping condition)
	if (fName==null || getParent(fName) == null) {
		return null;
	}
	String parent = getParent(fName);
	fName = fileMinusPath(fName);
	File f = new File(parent);
	String[] parentFiles = f.list();
	// System.out.println("checking dir " + parent + " for " + fName);

	// handle weird stuff like "c:/doc/../foo"; skip this parent dir
	if ( fName.equals("..") ) {
		return offensivePathMember(getParent(parent));
	}
	
	for (int i = 0; i < parentFiles.length; i++) {
		// System.out.println("is it " + parentFiles[i] + "?");
		if (parentFiles[i].equalsIgnoreCase(fName)) {
			if (!parentFiles[i].equals(fName)) {
				// System.out.println("case mismatch " + fName + " in " + parent);
				return parent + separator + fName;
			}
			// found a match, verify parent is ok
			return offensivePathMember(parent);
		}
	}
	// System.out.println("can't find " + fName + " in " + parent);
	return parent + separator + fName;
}
  public static String pathMinusFile(String f) {
	int endOfPath = f.lastIndexOf(separator);
	if ( endOfPath == -1 ) {
	  return "."; // no path found: use current directory
	}
	return f.substring(0, endOfPath);
  }  
  public static void visit(String file) throws IOException {
	File f = new File(file);
	file = f.getCanonicalPath();
	visited.put(file, new Boolean(true));
  }    
  public static boolean visited(String file) throws IOException {
	File f = new File(file);
	file = f.getCanonicalPath();
	return visited.get(file) != null;
  }    
  public static void visitImage(String file) throws IOException {
	File f = new File(file);
	file = f.getCanonicalPath();
	// System.out.println("caching image "+file);
	imgVisited.put(file, new Boolean(true));
  }            
}
