pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = boo.pc

gtksourceviewdir = $(GTKSOURCEVIEW_PREFIX)/share/gtksourceview-1.0/language-specs/
gtksourceview_DATA = boo.lang

mimepackagesdir = $(MIME_PREFIX)/share/mime/packages/
mimepackages_DATA = boo-mime-info.xml

bin_SCRIPTS = booc booi booish

DISTCLEANFILES = booc booi booish boo.pc

EXTRA_DIST = $(pkgconfig_DATA) $(gtksourceview_DATA) $(mimeinfo_DATA) \
	     $(mimepackages_DATA)
