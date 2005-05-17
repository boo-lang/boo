
import sys
import os, os.path
import string
import traceback
import antlr

version = sys.version.split()[0]
if version < '2.2.1':
    False = 0
if version < '2.3':
    True = not False

import LinkExtractor

class LinkListener:

    def hrefReference(self, target, line):
        raise NotImplementedError

    def imageReference(self, imageFileName, line):
        raise NotImplementedError


class LinkChecker(LinkListener):

    ### Record which files we have seen so that we don't get into an
    #   infinite loop and for efficiency.  The absolute path is stored here
    #   to uniquely identify the files.  That is, a file can be arrived
    #   at from many different locations such as help.html from .
    #   and ../help.html from a directory below.
    #
    #   This table is shared by all instances of LinkChecker.
    #
    visited = {}

    ### A table of the images visited by any document; a cache of correctness
    imgVisited = {}

    recursionDepth = 0
    separator = "/"			# not OS sensitive in HTML
    localSeparator = None
  
    def __init__(self, document):
	self.document = document
	self.directory = "."
	LinkChecker.localSeparator = os.sep

    def checkLinkRules(self, fName, line):
	# Check case of path (check for UNIX compatibility on a PC)!
	offensive = LinkChecker.offensivePathMember(self.directory + separator + fName)
	if offensive:
	    file_ = ''
	    try:
		f = file(offensive)
		file_ = os.path.normcase(offensive)
		self.error("Case mismatch in reference " + fName + ":" +
			   os.sep + "\treal name is " +
			   os.path.basename(file_) + os.sep +
			   "\treal absolute path is " + file_, line);
		return False
	    except Exception, e:
		self.error("internal error: cannot get canonical name for " +
			   offensive, line);
	if LinkChecker.pathIsAbsolute(fName):
	    self.error("Reference to " + fName + " with absolute path", line);
	    return False;
	return True

    def doCheck(self):
	if self.document[-5:] != ".html":
	    return
	# prevent infinite recursion to this file
	if LinkChecker.isVisited(self.document):
	    return
	LinkChecker.visit(self.document)
	LinkChecker.recursionDepth += 1
	f = file(self.document)
	lexer = LinkExtractor.Lexer(f)
	lexer.addLinkListener(self)
	# this will parse whole file since all tokens are skipped
	lexer.nextToken()
	LinkChecker.recursionDepth -= 1

    def error(self, err, line):
	d = "<internal error>"
	try:
	    # f = file(self.document)
	    d = os.path.normcase(self.document)
	except Exception, e:
	    sys.stderr.write("internal error: cannot find file that has error\n")
	    sys.exit(0)
	sys.stderr.write(d + ":" + str(line) + ": error: " + err + '\n')

    def pathIsAbsolute(path):
	return path[0] == '/' or path[1] == ':'
    pathIsAbsolute = staticmethod(pathIsAbsolute)

    def fileProtocolURL(target):
	return target.find("://") == -1 and \
	       not target[:7] == "mailto:" and \
	       not target[:5] == "news:"
    fileProtocolURL = staticmethod(fileProtocolURL)

    def getParent(path):
	return os.path.join(os.path.split(path)[:-1])
    getParent = staticmethod(getParent)

    def hrefReference(self, target, line):
	sys.stdout.write(self.document + ":" + str(line) + ": href to " + target + '\n')
	# recursively check the target document unless non-file ref
	if LinkChecker.fileProtocolURL(target):
	    # prune off any #name reference on end of file
	    pound = target.find('#')
	    path = target
	    if pound != -1:
		path = target[:pound]	# rip off #name on end, leave file
		if not len(path):
		    return		# ref to name in this file

		# first check existence on disk
		f = self.directory + os.sep + path
		if not os.path.exists(f):
		    self.error("Reference to missing file " + path, line)
		    return

		# check the case
		self.checkLinkRules(path, line);

		try:
		    # Link is ok, now follow the link
		    chk = LinkChecker.Lexer(self.directory + os.sep + path)
		    chk.doCheck()
		except Exception, e:
		    self.error("Document does not exist: " + target, line)

    def imageLinkIsOk(file_):
	# f = file(file_)
	f = os.path.normcase(file_)
	b = f in LinkChecker.imgVisited.keys()
	if b:
	    return True
	return False
    imageLinkIsOk = staticmethod(imageLinkIsOk)

    def imageReference(self, imageFileName, line):
	# first check if we have seen this exact file
	try:
	    if LinkChecker.imageLinkIsOk(self.directory + os.sep + imageFileName):
		return
	    f = self.directory + os.sep + imageFileName
	    if not os.path.exists(f):
		self.error("Reference to missing file " + imageFileName, line);
		return;
	    if self.checkLinkRules(imageFileName, line):
		LinkChecker.visitImage(self.directory + os.sep + imageFileName)
	except Exception, e:
	    sys.stderr.write("internal error: " + str(e) + '\n')

    ###
    #  Given a path to a file or dir, is the case of the reference
    #  the same as the actual path on the disk?  This is only
    #  meaningful on a PC which is case-insensitive (not a real
    #  file system).
    #
    #  Returns null if there is nothing offensive and the file exists.
    #  Returns offending file/dir if it does not exist or
    #  it has there is a case mismatch for it.  The last file is checked
    #  first followed by the parent directory, recursively, all the way
    #  to the absolute or relative path root in that String; i.e., we parse
    #  from right to left.
    #
    #  Because the File object won't actually go get the real filename
    #  from the disk so we can compare, we must get a directory listing
    #  of the directory and then look for the referenced file or dir.
    #  For example, for "./images/logo.gif" we would check "./images" dir
    #  listing for "logo.gif" with the appropriate case and then check
    #  directory "." for a dir called images with the right case.  When
    #  no parent exists, we can stop looking for case problems.

    def offensivePathMember(fName):
	sys.stdout.write("caseMismatch(" + fName + ")\n");
	# have we reached the root? (stopping condition)
	if not fName or not LinkChecker.getParent(fName):
	    return None
	parent = LinkChecker.getParent(fName)
	fName = os.path.basename(fName)
	# f = file(parent)
	parentFiles = os.path.split(parent)
	sys.stdout.write("checking dir " + parent + " for " + fName + '\n')

	# handle weird stuff like "c:/doc/../foo"; skip this parent dir
	if fName == "..":
	    return LinkChecker.offensivePathMember(LinkChecker.getParent(parent))
	
	for i in range(len(parentFiles)):
	    sys.stdout.write("is it " + parentFiles[i] + "?\n")
	    if string.lower(parentFiles[i]) == fName:
		if not parentFiles[i] == fName:
		    sys.stdout.write("case mismatch " + fName + " in " +
		    		       parent + '\n')
		    return parent + LinkChecker.separator + fName
		# found a match, verify parent is ok
		return LinkChecker.offensivePathMember(parent)

	sys.stdout.write("can't find " + fName + " in " + parent + '\n')
	return parent + LinkChecker.separator + fName
    offensivePathMember = staticmethod(offensivePathMember)

    def visit(file_):
	# f = file(file_)
	f = os.path.normcase(file_)
	LinkChecker.visited[f] = True
    visit = staticmethod(visit)

    def isVisited(file_):
	# f = file(file_)
	f = os.path.normcase(file_)
	return f in LinkChecker.visited.keys()
    isVisited = staticmethod(isVisited)

    def visitImage(file_):
	# f = file(file_)
	f = os.path.normcase(file_)
	sys.stdout.write("caching image " + f + '\n')
	LinkChecker.imgVisited[f] = True
    visitImage = staticmethod(visitImage)

class Main:

    def __init__(self):
	chk = LinkChecker(sys.argv[1])
	try:
	    chk.doCheck()
	except Exception, e:
	    sys.stderr.write("Exception: " + str(e) + '\n');
	    apply(traceback.print_exception, sys.exc_info())

if __name__ == "__main__":
    Main()
