#ifndef INC_Main_hpp__
#define INC_Main_hpp__

#include "antlr/TokenStreamSelector.hpp"

class PParser;
class PLexer;

// Define a selector that can handle nested include files.
// These variables are public so the parser/lexer can see them.
extern ANTLR_USE_NAMESPACE(antlr)TokenStreamSelector selector;
extern PParser* parser;
extern PLexer* mainLexer;

#endif //INC_Main_hpp__
