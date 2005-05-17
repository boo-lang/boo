// $ANTLR 2.7.5RC1 (20041124-137): "code.g" -> "CodeLexer.java"$

package antlr.actions.python;

import java.io.InputStream;
import antlr.TokenStreamException;
import antlr.TokenStreamIOException;
import antlr.TokenStreamRecognitionException;
import antlr.CharStreamException;
import antlr.CharStreamIOException;
import antlr.ANTLRException;
import java.io.Reader;
import java.util.Hashtable;
import antlr.CharScanner;
import antlr.InputBuffer;
import antlr.ByteBuffer;
import antlr.CharBuffer;
import antlr.Token;
import antlr.CommonToken;
import antlr.RecognitionException;
import antlr.NoViableAltForCharException;
import antlr.MismatchedCharException;
import antlr.TokenStream;
import antlr.ANTLRHashString;
import antlr.LexerSharedInputState;
import antlr.collections.impl.BitSet;
import antlr.SemanticException;

import java.io.StringReader;
import antlr.collections.impl.Vector;
import antlr.*;

public class CodeLexer extends antlr.CharScanner implements CodeLexerTokenTypes, TokenStream
 {

	protected int lineOffset = 0;
	private Tool antlrTool;	// The ANTLR tool

 	public CodeLexer ( 
        String s,
        String fname,
        int line,
        Tool tool
    )
    {
		this(new StringReader(s));
        setLine(line);
        setFilename(fname);
        this.antlrTool = tool;
	}

	public void setLineOffset(int lineOffset) {
		setLine(lineOffset);
	}

	public void reportError(RecognitionException e)
	{
		antlrTool.error(
            "Syntax error in action: "+e,
            getFilename(),getLine(),getColumn());
	}

	public void reportError(String s)
	{
		antlrTool.error(s,getFilename(),getLine(),getColumn());
	}

	public void reportWarning(String s)
	{
		if ( getFilename()==null ) {
			antlrTool.warning(s);
		}
		else {
			antlrTool.warning(s,getFilename(),getLine(), getColumn());
		}
	}
public CodeLexer(InputStream in) {
	this(new ByteBuffer(in));
}
public CodeLexer(Reader in) {
	this(new CharBuffer(in));
}
public CodeLexer(InputBuffer ib) {
	this(new LexerSharedInputState(ib));
}
public CodeLexer(LexerSharedInputState state) {
	super(state);
	caseSensitiveLiterals = true;
	setCaseSensitive(true);
	literals = new Hashtable();
}

public Token nextToken() throws TokenStreamException {
	Token theRetToken=null;
tryAgain:
	for (;;) {
		Token _token = null;
		int _ttype = Token.INVALID_TYPE;
		resetText();
		try {   // for char stream error handling
			try {   // for lexical error handling
				{
					mACTION(true);
					theRetToken=_returnToken;
				}
				
				if ( _returnToken==null ) continue tryAgain; // found SKIP token
				_ttype = _returnToken.getType();
				_returnToken.setType(_ttype);
				return _returnToken;
			}
			catch (RecognitionException e) {
				throw new TokenStreamRecognitionException(e);
			}
		}
		catch (CharStreamException cse) {
			if ( cse instanceof CharStreamIOException ) {
				throw new TokenStreamIOException(((CharStreamIOException)cse).io);
			}
			else {
				throw new TokenStreamException(cse.getMessage());
			}
		}
	}
}

	public final void mACTION(boolean _createToken) throws RecognitionException, CharStreamException, TokenStreamException {
		int _ttype; Token _token=null; int _begin=text.length();
		_ttype = ACTION;
		int _saveIndex;
		
		{
		_loop3:
		do {
			if (((LA(1) >= '\u0003' && LA(1) <= '\u00ff'))) {
				mSTUFF(false);
			}
			else {
				break _loop3;
			}
			
		} while (true);
		}
		if ( _createToken && _token==null && _ttype!=Token.SKIP ) {
			_token = makeToken(_ttype);
			_token.setText(new String(text.getBuffer(), _begin, text.length()-_begin));
		}
		_returnToken = _token;
	}
	
	protected final void mSTUFF(boolean _createToken) throws RecognitionException, CharStreamException, TokenStreamException {
		int _ttype; Token _token=null; int _begin=text.length();
		_ttype = STUFF;
		int _saveIndex;
		
		if ((LA(1)=='/') && (LA(2)=='*'||LA(2)=='/')) {
			mCOMMENT(false);
		}
		else if ((LA(1)=='\r') && (LA(2)=='\n')) {
			match("\r\n");
			newline();
		}
		else if ((LA(1)=='/') && (_tokenSet_0.member(LA(2)))) {
			match('/');
			{
			match(_tokenSet_0);
			}
		}
		else if ((LA(1)=='\r') && (true)) {
			match('\r');
			newline();
		}
		else if ((LA(1)=='\n')) {
			match('\n');
			newline();
		}
		else if ((_tokenSet_1.member(LA(1)))) {
			{
			match(_tokenSet_1);
			}
		}
		else {
			throw new NoViableAltForCharException((char)LA(1), getFilename(), getLine(), getColumn());
		}
		
		if ( _createToken && _token==null && _ttype!=Token.SKIP ) {
			_token = makeToken(_ttype);
			_token.setText(new String(text.getBuffer(), _begin, text.length()-_begin));
		}
		_returnToken = _token;
	}
	
	protected final void mCOMMENT(boolean _createToken) throws RecognitionException, CharStreamException, TokenStreamException {
		int _ttype; Token _token=null; int _begin=text.length();
		_ttype = COMMENT;
		int _saveIndex;
		
		if ((LA(1)=='/') && (LA(2)=='/')) {
			mSL_COMMENT(false);
		}
		else if ((LA(1)=='/') && (LA(2)=='*')) {
			mML_COMMENT(false);
		}
		else {
			throw new NoViableAltForCharException((char)LA(1), getFilename(), getLine(), getColumn());
		}
		
		if ( _createToken && _token==null && _ttype!=Token.SKIP ) {
			_token = makeToken(_ttype);
			_token.setText(new String(text.getBuffer(), _begin, text.length()-_begin));
		}
		_returnToken = _token;
	}
	
	protected final void mSL_COMMENT(boolean _createToken) throws RecognitionException, CharStreamException, TokenStreamException {
		int _ttype; Token _token=null; int _begin=text.length();
		_ttype = SL_COMMENT;
		int _saveIndex;
		
		_saveIndex=text.length();
		match("//");
		text.setLength(_saveIndex);
		
		/* rewrite comment symbol */
		text.append("#");
		
		{
		_loop10:
		do {
			// nongreedy exit test
			if ((LA(1)=='\n'||LA(1)=='\r') && (true)) break _loop10;
			if (((LA(1) >= '\u0003' && LA(1) <= '\u00ff')) && ((LA(2) >= '\u0003' && LA(2) <= '\u00ff'))) {
				matchNot(EOF_CHAR);
			}
			else {
				break _loop10;
			}
			
		} while (true);
		}
		{
		if ((LA(1)=='\r') && (LA(2)=='\n')) {
			match("\r\n");
		}
		else if ((LA(1)=='\n')) {
			match('\n');
		}
		else if ((LA(1)=='\r') && (true)) {
			match('\r');
		}
		else {
			throw new NoViableAltForCharException((char)LA(1), getFilename(), getLine(), getColumn());
		}
		
		}
		
		newline();
		
		if ( _createToken && _token==null && _ttype!=Token.SKIP ) {
			_token = makeToken(_ttype);
			_token.setText(new String(text.getBuffer(), _begin, text.length()-_begin));
		}
		_returnToken = _token;
	}
	
	protected final void mML_COMMENT(boolean _createToken) throws RecognitionException, CharStreamException, TokenStreamException {
		int _ttype; Token _token=null; int _begin=text.length();
		_ttype = ML_COMMENT;
		int _saveIndex;
		
		int offset = 0;
		
		
		_saveIndex=text.length();
		match("/*");
		text.setLength(_saveIndex);
		
		/* rewrite comment symbol */
		text.append("#");
		
		{
		_loop17:
		do {
			// nongreedy exit test
			if ((LA(1)=='*') && (LA(2)=='/')) break _loop17;
			if ((LA(1)=='\r') && (LA(2)=='\n')) {
				match('\r');
				match('\n');
				_saveIndex=text.length();
				mIGNWS(false);
				text.setLength(_saveIndex);
				
				newline();
				text.append("# ");
				
			}
			else if ((LA(1)=='\r') && ((LA(2) >= '\u0003' && LA(2) <= '\u00ff'))) {
				match('\r');
				_saveIndex=text.length();
				mIGNWS(false);
				text.setLength(_saveIndex);
				
				newline();
				text.append("# ");
				
			}
			else if ((LA(1)=='\n') && ((LA(2) >= '\u0003' && LA(2) <= '\u00ff'))) {
				match('\n');
				_saveIndex=text.length();
				mIGNWS(false);
				text.setLength(_saveIndex);
				
				newline();
				text.append("# ");
				
			}
			else if (((LA(1) >= '\u0003' && LA(1) <= '\u00ff')) && ((LA(2) >= '\u0003' && LA(2) <= '\u00ff'))) {
				matchNot(EOF_CHAR);
			}
			else {
				break _loop17;
			}
			
		} while (true);
		}
		
		/* force a newline */
		text.append("\n");
		
		_saveIndex=text.length();
		match("*/");
		text.setLength(_saveIndex);
		if ( _createToken && _token==null && _ttype!=Token.SKIP ) {
			_token = makeToken(_ttype);
			_token.setText(new String(text.getBuffer(), _begin, text.length()-_begin));
		}
		_returnToken = _token;
	}
	
	protected final void mIGNWS(boolean _createToken) throws RecognitionException, CharStreamException, TokenStreamException {
		int _ttype; Token _token=null; int _begin=text.length();
		_ttype = IGNWS;
		int _saveIndex;
		
		{
		_loop14:
		do {
			if ((LA(1)==' ') && ((LA(2) >= '\u0003' && LA(2) <= '\u00ff'))) {
				match(' ');
			}
			else if ((LA(1)=='\t') && ((LA(2) >= '\u0003' && LA(2) <= '\u00ff'))) {
				match('\t');
			}
			else {
				break _loop14;
			}
			
		} while (true);
		}
		if ( _createToken && _token==null && _ttype!=Token.SKIP ) {
			_token = makeToken(_ttype);
			_token.setText(new String(text.getBuffer(), _begin, text.length()-_begin));
		}
		_returnToken = _token;
	}
	
	
	private static final long[] mk_tokenSet_0() {
		long[] data = new long[8];
		data[0]=-145135534866440L;
		for (int i = 1; i<=3; i++) { data[i]=-1L; }
		return data;
	}
	public static final BitSet _tokenSet_0 = new BitSet(mk_tokenSet_0());
	private static final long[] mk_tokenSet_1() {
		long[] data = new long[8];
		data[0]=-140737488364552L;
		for (int i = 1; i<=3; i++) { data[i]=-1L; }
		return data;
	}
	public static final BitSet _tokenSet_1 = new BitSet(mk_tokenSet_1());
	
	}
