#ifndef INC_UnicodeCharScanner_hpp__
#define INC_UnicodeCharScanner_hpp__

#include <map>
#include <cctype>

#include <antlr/config.hpp>
#include <antlr/CommonToken.hpp>
#include <antlr/TokenStream.hpp>
#include <antlr/RecognitionException.hpp>
#include <antlr/SemanticException.hpp>
#include <antlr/InputBuffer.hpp>
#include <antlr/BitSet.hpp>
#include <antlr/LexerSharedInputState.hpp>

#include "MismatchedUnicodeCharException.hpp"

/** Superclass of generated lexers
 */
class UnicodeCharScanner : public antlr::TokenStream {
protected:
	typedef antlr::RefToken (*factory_type)();
public:
	typedef unsigned int char_type;
	typedef std::map<std::string,int> string_map;

	UnicodeCharScanner( antlr::InputBuffer& cb, bool case_sensitive )
	: saveConsumedInput(true)
	, caseSensitive(case_sensitive)
	, literals()
	, inputState(new antlr::LexerInputState(cb))
	, commitToPath(false)
	, tabsize(8)
	, traceDepth(0)
	{
		setTokenObjectFactory(&antlr::CommonToken::factory);
	}
	UnicodeCharScanner( antlr::InputBuffer* cb, bool case_sensitive )
	: saveConsumedInput(true)
	, caseSensitive(case_sensitive)
	, literals()
	, inputState(new antlr::LexerInputState(cb))
	, commitToPath(false)
	, tabsize(8)
	, traceDepth(0)
	{
		setTokenObjectFactory(&antlr::CommonToken::factory);
	}
	UnicodeCharScanner( const antlr::LexerSharedInputState& state, bool case_sensitive )
	: saveConsumedInput(true)
	, caseSensitive(case_sensitive)
	, literals()
	, inputState(state)
	, commitToPath(false)
	, tabsize(8)
	, traceDepth(0)
	{
		setTokenObjectFactory(&antlr::CommonToken::factory);
	}

	virtual ~UnicodeCharScanner()
	{
	}

	virtual char_type LA(char_type i)
	{
		char_type c = inputState->getInput().LA(i);
		return c;
	}

	virtual void append(char_type c)
	{
		if (saveConsumedInput)
		{
			size_t len = text.length();

			if( (len % 256) == 0 )
				text.reserve(len+256);

// This is how UTF8 is encoded
// +---------------------------+----------+----------+----------+----------+
// | Unicode scalar            | 1st      | 2nd      | 3th      | 4th      |
// +---------------------------+----------+----------+----------+----------+
// |00000000 0xxxxxxx          | 0xxxxxxx |          |          |          |
// |00000yyy yyxxxxxx          | 110yyyyy | 10xxxxxx |          |          |
// |zzzzyyyy yyxxxxxx          | 1110zzzz | 10yyyyyy | 10xxxxxx |          |
// |000uuuuu zzzzyyyy yyxxxxxx | 11110uuu | 10uuzzzz | 10yyyyyy | 10xxxxxx |
// +---------------------------+----------+----------+----------+----------+

			if (c < 0x80)
			{
				text += c;
				return;
			}
			else if (c < 0x800)
			{
				text += ( (c >> 6) | 0xC0 );
				text += ( c & 0x3F | 0x80 );
			}
			else if (c < 0x10000)
			{
				text += ( (c >> 12) | 0xE0 );
				text += ( ((c >> 6) & 0x3F) | 0x80 );
				text += ( (c & 0x3F) | 0x80 );
			}
			else if (c < 0x200000)
			{
				text += ( (c >> 18) | 0xF0 );				// first 3 bits
				text += ( (((c >> 16) & 0x3) << 4) |
								 ((c >> 12) & 0xF) | 0x80 );
				text += ( ((c >> 6) & 0x3F) | 0x80 );
				text += ( (c & 0x3F) | 0x80 );
			}
			else
				assert(0);
		}
	}

	virtual void append(const std::string& s)
	{
		assert(0);
		if (saveConsumedInput)
			text+=s;
	}

	virtual void commit()
	{
		inputState->getInput().commit();
	}

	virtual void consume()
	{
		if (inputState->guessing == 0)
		{
			char_type c = LA(1);
			append(c);
			inputState->column++;
		}
		inputState->getInput().consume();
	}

	/** Consume chars until one matches the given char */
	virtual void consumeUntil(char_type c)
	{
		for(;;)
		{
			char_type la_1 = LA(1);
			if( static_cast<char_type>(EOF_CHAR) == la_1 || la_1 == c )
				break;
			consume();
		}
	}

	/** Consume chars until one matches the given set */
	virtual void consumeUntil(const antlr::BitSet& set)
	{
		for(;;)
		{
			char_type la_1 = LA(1);
			if( static_cast<char_type>(EOF_CHAR) == la_1 || set.member(la_1) )
				break;
			consume();
		}
	}

	/// Mark the current position and return a id for it
	virtual unsigned int mark()
	{
		return inputState->getInput().mark();
	}

	/// Rewind the scanner to a previously marked position
	virtual void rewind(unsigned int pos)
	{
		inputState->getInput().rewind(pos);
	}

	/// See if input contains character 'c' throw MismatchedUnicodeCharException if not
	virtual void match(char_type c)
	{
		char_type la_1 = LA(1);
		if ( la_1 != c )
			throw MismatchedUnicodeCharException(la_1, c, false, this);
		consume();
	}

	/** See if input contains element from bitset b
	 * throw MismatchedUnicodeCharException if not
	 */
	virtual void match(const antlr::BitSet& b)
	{
		char_type la_1 = LA(1);

		if ( !b.member(la_1) )
			throw MismatchedUnicodeCharException( la_1, b, false, this );
		consume();
	}

	/** See if input contains string 's' throw MismatchedUnicodeCharException if not
	 * @note the string cannot match EOF
	 */
	virtual void match( const char* s )
	{
		while( *s != '\0' )
		{
			// the & 0xFF is here to prevent sign extension lateron
			char_type la_1 = LA(1), c = (*s++ & 0xFF);

			if ( la_1 != c )
				throw MismatchedUnicodeCharException(la_1, c, false, this);

			consume();
		}
	}
	/** See if input contains string 's' throw MismatchedUnicodeCharException if not
	 * @note the string cannot match EOF
	 */
	virtual void match(const std::string& s)
	{
		size_t len = s.length();

		for (size_t i = 0; i < len; i++)
		{
			// the & 0xFF is here to prevent sign extension lateron
			char_type la_1 = LA(1), c = (s[i] & 0xFF);

			if ( la_1 != c )
				throw MismatchedUnicodeCharException(la_1, c, false, this);

			consume();
		}
	}
	/** See if input does not contain character 'c'
	 * throw MismatchedUnicodeCharException if not
	 */
	virtual void matchNot(char_type c)
	{
		char_type la_1 = LA(1);

		if ( la_1 == c )
			throw MismatchedUnicodeCharException(la_1, c, true, this);

		consume();
	}
	/** See if input contains character in range c1-c2
	 * throw MismatchedUnicodeCharException if not
	 */
	virtual void matchRange(char_type c1, char_type c2)
	{
		char_type la_1 = LA(1);

		if ( la_1 < c1 || la_1 > c2 )
			throw MismatchedUnicodeCharException(la_1, c1, c2, false, this);

		consume();
	}

	/// Get the line the scanner currently is in (starts at 1)
	virtual int getLine() const
	{
		return inputState->line;
	}

	/// set the line number
	virtual void setLine(int l)
	{
		inputState->line = l;
	}

	/// Get the column the scanner currently is in (starts at 1)
	virtual int getColumn() const
	{
		return inputState->column;
	}
	/// set the column number
	virtual void setColumn(int c)
	{
		inputState->column = c;
	}

	/// get the filename for the file currently used
	virtual const std::string& getFilename() const
	{
		return inputState->filename;
	}
	/// Set the filename the scanner is using (used in error messages)
	virtual void setFilename(const std::string& f)
	{
		inputState->filename = f;
	}

	virtual bool getCommitToPath() const
	{
		return commitToPath;
	}

	virtual void setCommitToPath(bool commit)
	{
		commitToPath = commit;
	}

	/** return a copy of the current text buffer */
	virtual const std::string& getText() const
	{
		return text;
	}

	virtual void setText(const std::string& s)
	{
		text = s;
	}

	virtual void resetText()
	{
		text = "";
		inputState->tokenStartColumn = inputState->column;
		inputState->tokenStartLine = inputState->line;
	}

	virtual antlr::RefToken getTokenObject() const
	{
		return _returnToken;
	}

	///{ These need different handling in unicode case

	virtual bool getCaseSensitiveLiterals() const=0;

	virtual bool getCaseSensitive() const
	{
		return caseSensitive;
	}

	virtual void setCaseSensitive(bool t)
	{
		caseSensitive = t;
	}

	/** Override this method to get more specific case handling
	 * @note some platforms probably require setting the right locale for
	 * correct functioning.
	 */
	virtual char_type toLower(char_type c) const
	{
		return std::tolower(c);
	}

	/** Used to keep track of line breaks, needs to be called from
	 * within generated lexers when a \n \r is encountered.
	 */
	virtual void newline()
	{
		++inputState->line;
		inputState->column = 1;
	}

	/** Advance the current column number by an appropriate amount according
	 * to the tabsize. This method needs to be explicitly called from the
	 * lexer rules encountering tabs.
	 */
	virtual void tab()
	{
		int c = getColumn();
		int nc = ( ((c-1)/tabsize) + 1) * tabsize + 1;      // calculate tab stop
		setColumn( nc );
	}
	/// set the tabsize. Returns the old tabsize
	int setTabsize( int size )
	{
		int oldsize = tabsize;
		tabsize = size;
		return oldsize;
	}
	/// Return the tabsize used by the scanner
	int getTabSize() const
	{
		return tabsize;
	}
	///}

	/** Report exception errors caught in nextToken() */
	virtual void reportError(const antlr::RecognitionException& ex)
	{
		std::cerr << ex.toString().c_str() << std::endl;
	}

	/** Parser error-reporting function can be overridden in subclass */
	virtual void reportError(const std::string& s)
	{
		if (getFilename() == "")
			std::cerr << "error: " << s.c_str() << std::endl;
		else
			std::cerr << getFilename().c_str() << ": error: " << s.c_str() << std::endl;
	}

	/** Parser warning-reporting function can be overridden in subclass */
	virtual void reportWarning(const std::string& s)
	{
		if (getFilename() == "")
			std::cerr << "warning: " << s.c_str() << std::endl;
		else
			std::cerr << getFilename().c_str() << ": warning: " << s.c_str() << std::endl;
	}

	virtual antlr::InputBuffer& getInputBuffer()
	{
		return inputState->getInput();
	}

	virtual antlr::LexerSharedInputState getInputState()
	{
		return inputState;
	}

	/** set the input state for the lexer.
	 * @note state is a reference counted object, hence no reference */
	virtual void setInputState(antlr::LexerSharedInputState state)
	{
		inputState = state;
	}

	/// Set the factory for created tokens
	virtual void setTokenObjectFactory(factory_type factory)
	{
		tokenFactory = factory;
	}

	/** Test the token text against the literals table
	 * Override this method to perform a different literals test
	 */
	virtual int testLiteralsTable(int ttype) const
	{
		string_map::const_iterator i = literals.find(text);
		if (i != literals.end())
			ttype = (*i).second;
		return ttype;
	}

	/** Test the text passed in against the literals table
	 * Override this method to perform a different literals test
	 * This is used primarily when you want to test a portion of
	 * a token
	 */
	virtual int testLiteralsTable(const std::string& text, int ttype) const
	{
		string_map::const_iterator i = literals.find(text);
		if (i != literals.end())
			ttype = (*i).second;
		return ttype;
	}

	/** This method is called by YourLexer::nextToken() when the lexer has
	 *  hit EOF condition.  EOF is NOT a character.
	 *  This method is not called if EOF is reached during
	 *  syntactic predicate evaluation or during evaluation
	 *  of normal lexical rules, which presumably would be
	 *  an IOException.  This traps the "normal" EOF condition.
	 *
	 *  uponEOF() is called after the complete evaluation of
	 *  the previous token and only if your parser asks
	 *  for another token beyond that last non-EOF token.
	 *
	 *  You might want to throw token or char stream exceptions
	 *  like: "Heh, premature eof" or a retry stream exception
	 *  ("I found the end of this file, go back to referencing file").
	 */
	virtual void uponEOF()
	{
	}

	/// Methods used to change tracing behavior
	void traceIndent()
	{
		for( int i = 0; i < traceDepth; i++ )
			std::cout << " ";
	}

	void traceIn(const char* rname)
	{
		traceDepth++;
		traceIndent();
		std::cout << "> lexer " << rname
			<< "; c==" << LA(1) << std::endl;
	}

	void traceOut(const char* rname)
	{
		traceIndent();
		std::cout << "< lexer " << rname
			<< "; c==" << LA(1) << std::endl;
		traceDepth--;
	}

	static const int EOF_CHAR = EOF;
protected:
	std::string text; ///< Text of current token
 	/// flag indicating wether consume saves characters
	bool saveConsumedInput;
	factory_type tokenFactory;				///< Factory for tokens
	bool caseSensitive; 						///< Is this lexer case sensitive
	string_map literals;						 // set by subclass

	antlr::RefToken _returnToken;		///< used to return tokens w/o using return val

	/// Input state, gives access to input stream, shared among different lexers
	antlr::LexerSharedInputState inputState;

	/** Used during filter mode to indicate that path is desired.
	 * A subsequent scan error will report an error as usual
	 * if acceptPath=true;
	 */
	bool commitToPath;

	unsigned int tabsize; 	///< tab size the scanner uses.

	/// Create a new RefToken of type t
	virtual antlr::RefToken makeToken(int t)
	{
		antlr::RefToken tok = tokenFactory();
		// actually at this point you want to convert the stored lexeme text
		// into the format you want to have it in in the backend...
		tok->setType(t);
		tok->setColumn(inputState->tokenStartColumn);
		tok->setLine(inputState->tokenStartLine);
		return tok;
	}

	/** Tracer class, used when -traceLexer is passed to antlr
	 */
	class Tracer {
	private:
		UnicodeCharScanner* parser;
		const char* text;

		Tracer(const Tracer& other); 					// undefined
		Tracer& operator=(const Tracer& other); 	// undefined
	public:
		Tracer( UnicodeCharScanner* p, const char* t )
		: parser(p), text(t)
		{
			parser->traceIn(text);
		}
		~Tracer()
		{
			parser->traceOut(text);
		}
	};

	int traceDepth;
private:
	UnicodeCharScanner( const UnicodeCharScanner& other ); 		  		// undefined
	UnicodeCharScanner& operator=( const UnicodeCharScanner& other );	// undefined
};

#endif //INC_UnicodeCharScanner_hpp__
