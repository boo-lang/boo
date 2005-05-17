import sys
import antlr
import codecs

import unicode_l

def warn(msg):
   print >>sys.stderr,"warning:",msg
   sys.stderr.flush()

def error(msg):
   print >>sys.stderr,"error:",msg
   sys.stderr.flush()

### Unicode  handling  depends very much on  whether
### your  terminal can  handle (print) unicode chars.

### To  be  sure  about  it, just create a non ASCII
### letter and try to print it. If that is not going
### to work, we create an  alternative  method which
### maps non printable chars to '?'.

c = u"\N{LATIN SMALL LETTER O WITH ACUTE}"

try:
   print c
except UnicodeEncodeError,e:
   warn("terminal can't display unicode chars.")
   sys.stderr.flush()

   ## I'm just going to redefine 'unicode' to return
   ## a ASCII string.
   def unicode(x):
      return x.__str__().encode("ascii","replace")


### Now for the input. This should ideally  be  done
### in the lexer ..

### replace  stdin  with  a  wrapper that spits out
### unicode chars.
sys.stdin = codecs.lookup('latin1')[-2](sys.stdin)

try:
   for token in unicode_l.Lexer():
      print unicode(token)
except antlr.TokenStreamException, e:
   error(str(e))
