
header {
import sys
import string
}

header "LinkExtractor.__init__" {
    self.listener = None
}       

options {
    language = "Python";
}

/** Parse an entire html file, firing events to a single listener
 *  for each image and href link encountered.  All tokens are
 *  defined to skip so the lexer will continue scarfing until EOF.
 */
class LinkExtractor extends Lexer;

options {
    caseSensitive=false;
    k=2;
    filter=SCARF;
    charVocabulary='\3'..'\177';
}

{
    def addLinkListener(self, listener):
        self.listener = listener

    def removeLinkListener(self, listener):
        self.listener = None

    def fireImageLinkEvent(self, target, line):
        self.listener.imageReference(target, line)

    def fireHREFLinkEvent(self, target, line):
        self.listener.hrefReference(target, line)

    /** strip quotes from "..." or '...' strings */
    def stripQuotes(src):
        h = src.find('"')
        if h == -1:
            h = src.index("'")
        t = src.rfind('"')
        if t == -1:
            t = src.rindex("'");
        if h == -1 or t == -1:
            return src
        return src[h+1:t]
    stripQuotes = staticmethod(stripQuotes)

}

AHREF
        :       "<a" WS (ATTR)+ '>'     { $skip }
        ;

IMG     :       "<img" WS (ATTR)+ '>'   { $skip }
        ;

protected
ATTR
options {
        ignore=WS;
}
        :       w:WORD '='
                (       s:STRING
                |       v:WORD
                )
                {
                    if s:
                        target = self.stripQuotes(s.getText())
                    else:
                        target = v.getText()
                    if string.lower(w.getText()) == "href":
                        self.fireHREFLinkEvent(target, self.getLine())
                    elif string.lower(w.getText()) == "src":
                        self.fireImageLinkEvent(target, self.getLine())
                }
        ;

/** Match until next whitespace; can be file, int, etc... */
protected
WORD:   (
                        options {
                                generateAmbigWarnings=false;
                        }
                :       'a'..'z' | '0'..'9' | '/' | '.' | '#' | '_'
                )+
        ;

protected
STRING
        :       '"' (~'"')* '"'
        |       '\'' (~'\'')* '\''
        ;

protected
WS      :       (       ' '
                |       '\t'
                |       '\f'
                |       (       "\r\n"  // DOS
                        |       '\r'    // Macintosh
                        |       '\n'    // Unix (the right way)
                        )
                        { $newline }
                )
                { $skip }
        ;

protected
SCARF
        :       WS      // track line numbers while you scarf
        |       .
        ;
