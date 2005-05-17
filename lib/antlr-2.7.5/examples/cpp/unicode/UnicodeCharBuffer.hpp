#ifndef INC_UnicodeCharBuffer_hpp__
#define INC_UnicodeCharBuffer_hpp__

#include <istream>
#include <cassert>
#include <antlr/config.hpp>
#include <antlr/InputBuffer.hpp>
#include <antlr/CharStreamIOException.hpp>

class ANTLR_API UnicodeCharBuffer : public antlr::InputBuffer {
public:
	typedef unsigned int char_type;	// should be 32 bits!

	/// Create a character buffer
	UnicodeCharBuffer(std::istream& inp)
	: input(inp)
	{
		//	input.exceptions(std::ios_base::badbit|
		//						  std::ios_base::failbit);
	}
	/// Get the next character from the stream
	int getChar()
	{
		char_type ch = 0;
		int inchar = input.get();
		if( inchar == EOF )
			return -1;

// This is how UTF8 is encoded
// +---------------------------+----------+----------+----------+----------+
// | Unicode scalar            | 1st      | 2nd      | 3th      | 4th      |
// +---------------------------+----------+----------+----------+----------+
// |00000000 0xxxxxxx          | 0xxxxxxx |          |          |          |
// |00000yyy yyxxxxxx          | 110yyyyy | 10xxxxxx |          |          |
// |zzzzyyyy yyxxxxxx          | 1110zzzz | 10yyyyyy | 10xxxxxx |          |
// |000uuuuu zzzzyyyy yyxxxxxx | 11110uuu | 10uuzzzz | 10yyyyyy | 10xxxxxx |
// +---------------------------+----------+----------+----------+----------+

		if( (inchar & 0x80) == 0 )
			return inchar;

		unsigned int need = 0;
		if( (inchar & 0xF8) == 0xF8 )
		{
			ch = inchar & 7;
			need = 3;
		}
		else if( (inchar & 0xE0) == 0xE0 )
		{
			ch = inchar & 0xF;
			need = 2;
		}
		else if( (inchar & 0xC0) == 0xC0 )
		{
			ch = inchar & 0x1F;
			need = 1;
		}
		else
		{
			assert("Invalid UTF8");
		}
		while( need )
		{
			inchar = input.get();
			if( inchar == EOF )
				assert("Invalid UTF8");
//				throw antlr::CharStreamIOException(std::logic_error());
			ch <<= 6;
			ch += inchar & 0x3F;
			need--;
		}
		return ch;
	}
private:
	// character source
	std::istream& input;

	// NOTE: Unimplemented
	UnicodeCharBuffer(const UnicodeCharBuffer& other);
	UnicodeCharBuffer& operator=(const UnicodeCharBuffer& other);
};

#endif //INC_UnicodeCharBuffer_hpp__
