#ifndef INC_MismatchedUnicodeCharException_hpp__
#define INC_MismatchedUnicodeCharException_hpp__

/* ANTLR Translator Generator
 * Project led by Terence Parr at http://www.jGuru.com
 * Software rights: http://www.antlr.org/license.html
 *
 * $Id:$
 */

#include <antlr/config.hpp>
#include <antlr/RecognitionException.hpp>
#include <antlr/BitSet.hpp>
#include <antlr/String.hpp>

class UnicodeCharScanner;

class MismatchedUnicodeCharException : public antlr::RecognitionException {
public:
	typedef unsigned int char_type;
	typedef enum {
		CHAR = 1,
		NOT_CHAR = 2,
		RANGE = 3,
		NOT_RANGE = 4,
		SET = 5,
		NOT_SET = 6
	} MATCH_TYPE;

	MismatchedUnicodeCharException();

	// Expected range / not range
	MismatchedUnicodeCharException(
		char_type c,
		char_type lower,
		char_type up,
		bool matchNot,
		UnicodeCharScanner* cs
	);

	// Expected char / not char
	MismatchedUnicodeCharException(
		char_type c,
		char_type expect,
		bool matchNot,
		UnicodeCharScanner* cs
	);

	// Expected BitSet / not BitSet
	MismatchedUnicodeCharException(
		char_type c,
		antlr::BitSet s,
		bool matchNot,
		UnicodeCharScanner* cs
	);

	~MismatchedUnicodeCharException() throw();

	/**
	 * Returns a clean error message (no line number/column information)
	 */
	std::string getMessage() const;
private:
	// One of the above
	MATCH_TYPE mismatchType;

	// what was found on the input stream
	char_type foundChar;

	// For CHAR/NOT_CHAR and RANGE/NOT_RANGE
	char_type expecting;

	// For RANGE/NOT_RANGE (expecting is lower bound of range)
	char_type upper;

	// For SET/NOT_SET
	antlr::BitSet set;
	// who knows...they may want to ask scanner questions
	UnicodeCharScanner* scanner;
};

#endif
