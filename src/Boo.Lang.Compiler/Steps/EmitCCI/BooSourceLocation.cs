using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    class BooSourceLocation : ISourceLocation
    {
        private BooSourceDocument _document;
        private LexicalInfo _loc;

        internal BooSourceLocation(BooSourceDocument doc, LexicalInfo start)
        {
            _document = doc;
            _loc = start;
        }

        public IDocument Document
        {
            get
            {
                return _document;
            }
        }

        public int EndIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Source
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISourceDocument SourceDocument
        {
            get
            {
                return _document;
            }
        }

        public int StartIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Contains(ISourceLocation location)
        {
            throw new NotImplementedException();
        }

        public int CopyTo(int offset, char[] destination, int destinationOffset, int length)
        {
            throw new NotImplementedException();
        }
    }
}
