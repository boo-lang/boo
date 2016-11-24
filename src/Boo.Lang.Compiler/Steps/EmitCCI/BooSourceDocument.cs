using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    class BooSourceDocument : ISourceDocument
    {

        private readonly string _filename;
        private readonly string _docname;

        public BooSourceDocument(string filename)
        {
            _filename = filename;
            _docname = Path.GetFileNameWithoutExtension(filename);
        }

        public ISourceLocation GetLocation(Ast.LexicalInfo loc)
        {
            return new BooSourceLocation(this, loc);
        }

        public int Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Location
        {
            get
            {
                return _filename;
            }
        }

        public IName Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string SourceLanguage
        {
            get
            {
                return "Boo";
            }
        }

        public ISourceLocation SourceLocation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CopyTo(int position, char[] destination, int destinationOffset, int length)
        {
            throw new NotImplementedException();
        }

        public ISourceLocation GetCorrespondingSourceLocation(ISourceLocation sourceLocationInPreviousVersionOfDocument)
        {
            throw new NotImplementedException();
        }

        public ISourceLocation GetSourceLocation(int position, int length)
        {
            throw new NotImplementedException();
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public bool IsUpdatedVersionOf(ISourceDocument sourceDocument)
        {
            return false;
        }
    }
}
