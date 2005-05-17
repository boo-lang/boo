#ifndef INC_LexTokenStream_hpp__
#define INC_LexTokenStream_hpp__

#include <cstdio>
#include "antlr/TokenStream.hpp"

class LexTokenStream : public ANTLR_USE_NAMESPACE(antlr)TokenStream {
public:
	LexTokenStream(std::FILE* fp);
	void newLine();
	ANTLR_USE_NAMESPACE(antlr)RefToken nextToken();
private:
	int line;
	bool reachedEOF;
};

#endif //INC_LexTokenStream_hpp__
