using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.WriterUtilities;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    class BooResourceWriter : IResourceWriter
    {
        private Assembly _asm;
        private IName _name;
        private ResourceWriter _writer;
        private System.IO.MemoryStream _stream;

        public BooResourceWriter(Assembly asm, IName name)
        {
            _asm = asm;
            _name = name;
            _stream = new System.IO.MemoryStream();
            _writer = new ResourceWriter(_stream);
        }

        public void AddResource(string name, byte[] value)
        {
            _writer.AddResource(name, value);
        }

        public void AddResource(string name, object value)
        {
            _writer.AddResource(name, value);
        }

        public void AddResource(string name, string value)
        {
            _writer.AddResource(name, value);
        }

        public void Close()
        {
            Dispose();
        }

        public void Generate()
        {
            _writer.Generate();
            var bytes = _stream.GetBuffer();
            var res = new Resource
            {
                Data = bytes.ToList(),
                DefiningAssembly = _asm,
                IsPublic = true,
                Name = _name,
            };
            _asm.Resources.Add(res);
        }

        public void Dispose()
        {
            Generate();
            _writer.Dispose();
        }
    }
}
