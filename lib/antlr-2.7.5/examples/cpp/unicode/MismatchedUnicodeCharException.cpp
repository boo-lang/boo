
#include <iostream>

#include <antlr/config.hpp>
#include <antlr/RecognitionException.hpp>
#include <antlr/BitSet.hpp>
#include <antlr/String.hpp>
#include "MismatchedUnicodeCharException.hpp"
#include "UnicodeCharScanner.hpp"


MismatchedUnicodeCharException::MismatchedUnicodeCharException()
: RecognitionException("Mismatched char")
{
}

// Expected range / not range
MismatchedUnicodeCharException::MismatchedUnicodeCharException(
	char_type c,
	char_type lower,
	char_type up,
	bool matchNot,
	UnicodeCharScanner* cs
)
: RecognitionException("Mismatched char",
							  cs->getFilename(),
							  cs->getLine(), cs->getColumn())
, mismatchType(matchNot ? NOT_RANGE : RANGE)
, foundChar(c)
, expecting(lower)
, upper(up)
, scanner(cs)
{
}

// Expected char / not char
MismatchedUnicodeCharException::MismatchedUnicodeCharException(
	char_type c,
	char_type expect,
	bool matchNot,
	UnicodeCharScanner* cs
) : RecognitionException("Mismatched char",
                      cs->getFilename(),
							 cs->getLine(), cs->getColumn())
, mismatchType(matchNot ? NOT_CHAR : CHAR)
, foundChar(c)
, expecting(expect)
, scanner(cs)
{
}

// Expected BitSet / not BitSet
MismatchedUnicodeCharException::MismatchedUnicodeCharException(
	char_type c,
	antlr::BitSet s,
	bool matchNot,
	UnicodeCharScanner* cs
) : RecognitionException("Mismatched char",
                      cs->getFilename(),
							 cs->getLine(), cs->getColumn())
, mismatchType(matchNot ? NOT_SET : SET)
, foundChar(c)
, set(s)
, scanner(cs)
{
}

MismatchedUnicodeCharException::~MismatchedUnicodeCharException() throw() {}

/**
 * Returns a clean error message (no line number/column information)
 */
std::string MismatchedUnicodeCharException::getMessage() const
{
	ANTLR_USE_NAMESPACE(std)string s;

	switch (mismatchType) {
	case CHAR :
		s += "expecting '" + antlr::charName(expecting) + "', found '" + antlr::charName(foundChar) + "'";
		break;
	case NOT_CHAR :
		s += "expecting anything but '" + antlr::charName(expecting) + "'; got it anyway";
		break;
	case RANGE :
		s += "expecting token in range: '" + antlr::charName(expecting) + "'..'" + antlr::charName(upper) + "', found '" + antlr::charName(foundChar) + "'";
		break;
	case NOT_RANGE :
		s += "expecting token NOT in range: " + antlr::charName(expecting) + "'..'" + antlr::charName(upper) + "', found '" + antlr::charName(foundChar) + "'";
		break;
	case SET :
	case NOT_SET :
		{
			s += ANTLR_USE_NAMESPACE(std)string("expecting ") + (mismatchType == NOT_SET ? "NOT " : "") + "one of (";
			ANTLR_USE_NAMESPACE(std)vector<unsigned int> elems = set.toArray();
			for ( unsigned int i = 0; i < elems.size(); i++ )
			{
				s += " '";
				s += antlr::charName(elems[i]);
				s += "'";
			}
			s += "), found '" + antlr::charName(foundChar) + "'";
		}
		break;
	default :
		s += RecognitionException::getMessage();
		break;
	}

	return s;
}
