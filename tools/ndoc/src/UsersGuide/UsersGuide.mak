#change the value of this macro to where you keep the html help compiler
HHC= "$(PROGRAMFILES)\HTML Help Workshop\hhc.exe"


!IF !EXIST ($(HHC))
!ERROR Could not find the html help compiler
!ENDIF


.SUFFIXES :
.SUFFIXES : .chm .hhp .htm .html .css .jpg .png .gif .js .xsd


# hhc returns 1 on success, which NMAKE interprets as an error
# ignore non-zero exit codes
.IGNORE :

NDocUsersGuide.chm: UsersGuide.hhp 
# make the CHM	
	$(HHC) UsersGuide.hhp
# copy user guide to doc/help directory where setup generation will pick it up
!IF !EXIST (..\..\doc\help)
	mkdir ..\..\doc\help
!ENDIF
	copy NDocUsersGuide.chm ..\..\doc\help /y
# copy user guide to gui bin directory
	copy NDocUsersGuide.chm ..\Gui\bin\$(CONFIG) /y	
	
# turn error codes back on	
!CMDSWITCHES -I

# these are the various file types that the chm is dependent on
IMAGES = {content\images}*.gif {content\images}*.jpg
SCRIPTS = {content\script}*.js
CSS = {content\css}*.css
HTML = {content}*.htm 

EXAMPLEDIR = content\examples

# the UsersGuide.hhp pseudotarget is dependent on all of the html files in the content directory
# as well as the css, image, and script files
UsersGuide.hhp: $(HTML) $(CSS) $(SCRIPTS) $(IMAGES) $(EXAMPLEDIR)\NamespaceMap.xsd  $(EXAMPLEDIR)\NamespaceMap.xml


NAMESPACEMAPPINGDIR = ..\Documenter\NativeHtmlHelp2\Engine\NamespaceMapping

$(EXAMPLEDIR)\NamespaceMap.xsd: $(NAMESPACEMAPPINGDIR)\NamespaceMap.xsd $(NAMESPACEMAPPINGDIR)\NamespaceMap.xml
# copy examples into the content directory
!IF !EXIST ($(EXAMPLEDIR))
	mkdir $(EXAMPLEDIR)
!ENDIF
	copy /a $(NAMESPACEMAPPINGDIR)\NamespaceMap.xsd+,, $(EXAMPLEDIR)\NamespaceMap.xsd /y
	copy /a $(NAMESPACEMAPPINGDIR)\NamespaceMap.xml+,, $(EXAMPLEDIR)\NamespaceMap.xml /y 

