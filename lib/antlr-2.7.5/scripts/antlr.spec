Summary: The ANTLR Parser Framework
Name:    antlr
Version: 2.7.5
Release: 1
License: Public Domain
Group:   Development/Tools
Source:  http://www.antlr.org/download/antlr-%{version}.tar.gz
URL:     http://www.antlr.org

%description
ANTLR, ANother Tool for Language Recognition, (formerly PCCTS) is a
language tool that provides a framework for constructing recognizers,
compilers, and translators using Java, C#, C++ or Python. ANTLR
provides  excellent support for tree construction, tree walking, and 
translation.

%prep
%setup -q

%build
%configure --disable-examples
make

%install
rm -rf $RPM_BUILD_ROOT

make install

%files
%defattr(-,root,root)

/usr/bin/antlr
/usr/bin/antlr-config
/usr/sbin/pyantlr.sh
/usr/include/antlr/ANTLRException.hpp
/usr/include/antlr/ANTLRUtil.hpp
/usr/include/antlr/ASTArray.hpp
/usr/include/antlr/ASTFactory.hpp
/usr/include/antlr/AST.hpp
/usr/include/antlr/ASTNULLType.hpp
/usr/include/antlr/ASTPair.hpp
/usr/include/antlr/ASTRefCount.hpp
/usr/include/antlr/BaseAST.hpp
/usr/include/antlr/BitSet.hpp
/usr/include/antlr/CharBuffer.hpp
/usr/include/antlr/CharInputBuffer.hpp
/usr/include/antlr/CharScanner.hpp
/usr/include/antlr/CharStreamException.hpp
/usr/include/antlr/CharStreamIOException.hpp
/usr/include/antlr/CircularQueue.hpp
/usr/include/antlr/CommonAST.hpp
/usr/include/antlr/CommonASTWithHiddenTokens.hpp
/usr/include/antlr/CommonHiddenStreamToken.hpp
/usr/include/antlr/CommonToken.hpp
/usr/include/antlr/config.hpp
/usr/include/antlr/InputBuffer.hpp
/usr/include/antlr/IOException.hpp
/usr/include/antlr/LexerSharedInputState.hpp
/usr/include/antlr/LLkParser.hpp
/usr/include/antlr/MismatchedCharException.hpp
/usr/include/antlr/MismatchedTokenException.hpp
/usr/include/antlr/NoViableAltException.hpp
/usr/include/antlr/NoViableAltForCharException.hpp
/usr/include/antlr/Parser.hpp
/usr/include/antlr/ParserSharedInputState.hpp
/usr/include/antlr/RecognitionException.hpp
/usr/include/antlr/RefCount.hpp
/usr/include/antlr/SemanticException.hpp
/usr/include/antlr/String.hpp
/usr/include/antlr/TokenBuffer.hpp
/usr/include/antlr/Token.hpp
/usr/include/antlr/TokenStreamBasicFilter.hpp
/usr/include/antlr/TokenStreamException.hpp
/usr/include/antlr/TokenStreamHiddenTokenFilter.hpp
/usr/include/antlr/TokenStream.hpp
/usr/include/antlr/TokenStreamIOException.hpp
/usr/include/antlr/TokenStreamRecognitionException.hpp
/usr/include/antlr/TokenStreamRetryException.hpp
/usr/include/antlr/TokenStreamRewriteEngine.hpp
/usr/include/antlr/TokenStreamSelector.hpp
/usr/include/antlr/TokenWithIndex.hpp
/usr/include/antlr/TreeParser.hpp
/usr/include/antlr/TreeParserSharedInputState.hpp
/usr/lib/antlr.jar
/usr/lib/libantlr.a
/usr/lib/antlr.py
/usr/share/2.7.5/antlr.jar
/usr/share/2.7.5/__init__.py
/usr/share/2.7.5/antlr.py
/usr/share/2.7.5/antlr-mode.el
/usr/share/2.7.5/antlr-jedit.xml
/usr/share/doc/2.7.5/closure.gif
/usr/share/doc/2.7.5/cpp-runtime.html
/usr/share/doc/2.7.5/csharp-runtime.html
/usr/share/doc/2.7.5/err.html
/usr/share/doc/2.7.5/glossary.html
/usr/share/doc/2.7.5/hidden.stream.gif
/usr/share/doc/2.7.5/index.html
/usr/share/doc/2.7.5/inheritance.html
/usr/share/doc/2.7.5/j-guru-blue.jpg
/usr/share/doc/2.7.5/jguru-logo.gif
/usr/share/doc/2.7.5/lexer.html
/usr/share/doc/2.7.5/lexer.to.parser.tokens.gif
/usr/share/doc/2.7.5/logo.gif
/usr/share/doc/2.7.5/metalang.html
/usr/share/doc/2.7.5/optional.gif
/usr/share/doc/2.7.5/options.html
/usr/share/doc/2.7.5/posclosure.gif
/usr/share/doc/2.7.5/python-runtime.html
/usr/share/doc/2.7.5/runtime.html
/usr/share/doc/2.7.5/sor.html
/usr/share/doc/2.7.5/stream.perspectives.gif
/usr/share/doc/2.7.5/stream.selector.gif
/usr/share/doc/2.7.5/streams.html
/usr/share/doc/2.7.5/stream.splitter.gif
/usr/share/doc/2.7.5/subrule.gif
/usr/share/doc/2.7.5/trees.html
/usr/share/doc/2.7.5/vocab.html
/usr/share/doc/2.7.5/LICENSE.txt
/usr/share/doc/2.7.5/README.txt
/usr/share/doc/2.7.5/INSTALL.txt


%clean


%changelog
* Tue Jan 11 2005 Wolfgang Haefelinger <ora dot et dot labora at web
dot de> 
  Build RPM on Mandrake 1o 
* Thu Aug 21 2003 Ric Klaren <klaren@cs.utwente.nl>
- First stab at RPM for RH9
