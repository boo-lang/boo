#! /usr/bin/python -t
## --*- python -*--

import sys
import antlr
    
version = sys.version.split()[0]
if version < '2.2.1':
    False = 0
if version < '2.3':
    True = not False

class CharScanner(antlr.CharScanner):
   def __init__(self,*args):
      super(CharScanner,self).__init__(*args)
      self.altcomment = True
      self.state_with_syntax = False

   ### check whether a string contains a lower case char
   def haslowerchar(self,s):
      return (s.upper() != s)

   def handle_comment(self):
      la1 = self.LA(1)
      if not la1:
         self.throw_no_viable_alt_for_char(la1)
      elif la1 in '-':
         self.match("--")
      elif la1 in '\n':
         self.match('\n')
         self.newline()
      elif la1 in '\r': 
         self.match('\r')
         if self.LA(2) == '\n':
            self.match('\n')
         self.newline()
      elif la1 in u'\u000b' :
         self.match(u'\u000b')
         self.newline()
      elif la1 in u'\u000c':
         self.match('\u000c')
         self.newline()
      else:
         self.throw_no_viable_alt_for_char(la1)


   def throw_no_viable_alt_for_char(self,la1):
      raise antlr.NoViableAltForCharException(
         la1, 
         self.getFilename(), 
         self.getLine(), 
         self.getColumn()
         )
      
   def chr_ws_erase(self,string,*chars):
      return string


if __name__ == '__main__' :
   ### create my lexer ..
   ### print "reading from test.in .."
   Lexer = lexer.Lexer("test.in")


   token = Lexer.nextToken()
   while not token.isEOF():
      ### Is there a way to simplify this loop??
      ### this looks complicated to me. However, we can't simply
      ### return none to check  for EOF as we would like to know
      ### where  EOF appeared  (file, line, col etc). This would
      ### be lost. Or we could return NIL in case of EOF and, if
      ### we are really want to know more about EOF ask lexer to
      ### provide this information. But this would extend the
      ### lexer's interface. Another idea would be to return EOF
      ### by  exception, but EOF is actually not an exception at
      ### all.
      ### handle token
      print token
      token = Lexer.nextToken()

      
