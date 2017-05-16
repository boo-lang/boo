#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Linq;
using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
    using System.Collections.Generic;

    public class ProcessSharedLocals : AbstractTransformerCompilerStep
    {
        private Method _currentMethod;

        private ClassDefinition _sharedLocalsClass;

        private readonly Dictionary<IEntity, IField> _mappings = new Dictionary<IEntity, IField>();

        private readonly List<ReferenceExpression> _references = new List<ReferenceExpression>();

        private readonly List<ILocalEntity> _shared = new List<ILocalEntity>();

        private int _closureDepth;

        public override void Dispose()
        {
            _shared.Clear();
            _references.Clear();
            _mappings.Clear();
            base.Dispose();
        }

        public override void OnField(Field node)
        {
        }

        public override void OnInterfaceDefinition(InterfaceDefinition node)
        {
        }

        public override void OnEnumDefinition(EnumDefinition node)
        {
        }

        public override void OnConstructor(Constructor node)
        {
            OnMethod(node);
        }

        public override void OnMethod(Method node)
        {
            _references.Clear();
            _mappings.Clear();
            _currentMethod = node;
            _sharedLocalsClass = null;
            _closureDepth = 0;

            Visit(node.Body);

            CreateSharedLocalsClass();
            if (null != _sharedLocalsClass)
            {
                node.DeclaringType.Members.Add(_sharedLocalsClass);
                Map();
            }
        }

        public override void OnBlockExpression(BlockExpression node)
        {
            ++_closureDepth;
            Visit(node.Body);
            --_closureDepth;
        }

        public override void OnGeneratorExpression(GeneratorExpression node)
        {
            ++_closureDepth;
            Visit(node.Iterator);
            Visit(node.Expression);
            Visit(node.Filter);
            --_closureDepth;
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            var local = node.Entity as ILocalEntity;
            if (null == local) return;
            if (local.IsPrivateScope) return;

            _references.Add(node);

            if (_closureDepth == 0) return;

            local.IsShared = _currentMethod.Locals.ContainsEntity(local)
                            || _currentMethod.Parameters.ContainsEntity(local);

        }

        private void Map()
        {
            var type = GeneratorTypeReplacer.MapTypeInMethodContext((IType)_sharedLocalsClass.Entity, _currentMethod);
            var conType = type as GenericConstructedType;
            if (conType != null)
            {
                foreach (var key in _mappings.Keys.ToArray())
                    _mappings[key] = (IField)conType.ConstructedInfo.Map(_mappings[key]);
            }
            var locals = CodeBuilder.DeclareLocal(_currentMethod, "$locals", type);

            foreach (var reference in _references)
            {
                IField mapped;
                if (!_mappings.TryGetValue(reference.Entity, out mapped)) continue;

                reference.ParentNode.Replace(
                    reference,
                    CodeBuilder.CreateMemberReference(
                        CodeBuilder.CreateReference(locals),
                        mapped));
            }

            var initializationBlock = new Block();
            initializationBlock.Add(CodeBuilder.CreateAssignment(
                        CodeBuilder.CreateReference(locals),
                        CodeBuilder.CreateConstructorInvocation(type.GetConstructors().First())));
            InitializeSharedParameters(initializationBlock, locals);
            _currentMethod.Body.Statements.Insert(0, initializationBlock);

            foreach (IEntity entity in _mappings.Keys)
            {
                _currentMethod.Locals.RemoveByEntity(entity);
            }
        }

        private void InitializeSharedParameters(Block block, InternalLocal locals)
        {
            foreach (var node in _currentMethod.Parameters)
            {
                var param = (InternalParameter)node.Entity;
                if (param.IsShared)
                {
                    block.Add(
                        CodeBuilder.CreateAssignment(
                            CodeBuilder.CreateMemberReference(
                                CodeBuilder.CreateReference(locals),
                                _mappings[param]),
                            CodeBuilder.CreateReference(param)));
                }
            }
        }

        private void CreateSharedLocalsClass()
        {
            _shared.Clear();

            CollectSharedLocalEntities(_currentMethod.Locals);
            CollectSharedLocalEntities(_currentMethod.Parameters);

            if (_shared.Count > 0)
            {
                BooClassBuilder builder = CodeBuilder.CreateClass(Context.GetUniqueName(_currentMethod.Name, "locals"));
                builder.Modifiers |= TypeMemberModifiers.Internal;
                builder.AddBaseType(TypeSystemServices.ObjectType);

                var genericsSet = new HashSet<string>();
                var replacer = new GeneratorTypeReplacer();
                foreach (ILocalEntity local in _shared)
                {
                    CheckTypeForGenericParams(local.Type, genericsSet, builder, replacer);
                    Field field = builder.AddInternalField(
                                    string.Format("${0}", local.Name),
                                    replacer.MapType(local.Type));

                    _mappings[local] = (IField)field.Entity;
                }

                builder.AddConstructor().Body.Add(
                    CodeBuilder.CreateSuperConstructorInvocation(TypeSystemServices.ObjectType));

                _sharedLocalsClass = builder.ClassDefinition;
            }
        }

        private static void CheckTypeForGenericParams(
            IType type, 
            HashSet<string> genericsSet,
            BooClassBuilder builder,
            GeneratorTypeReplacer mapper)
        {
            if (type is IGenericParameter)
            {
                if (!genericsSet.Contains(type.Name))
                {
                    builder.AddGenericParameter(type.Name);
                    genericsSet.Add(type.Name);
                }
                if (!mapper.ContainsType(type))
                {
                    mapper.Replace(
                        type,
                        (IType)builder.ClassDefinition.GenericParameters
                            .First(gp => gp.Name.Equals(type.Name)).Entity);
                }
            }
            else
            {
                var genType = type as IConstructedTypeInfo;
                if (genType != null)
                    foreach (var arg in genType.GenericArguments)
                        CheckTypeForGenericParams(arg, genericsSet, builder, mapper);
            }
        }

        private void CollectSharedLocalEntities<T>(IEnumerable<T> nodes) where T : Node
        {
            foreach (T node in nodes)
            {
                var local = (ILocalEntity)node.Entity;
                if (local.IsShared)
                    _shared.Add(local);
            }
        }
    }
}
