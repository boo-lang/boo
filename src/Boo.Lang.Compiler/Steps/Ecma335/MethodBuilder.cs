using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class MethodBuilder : IBuilder
    {
        private readonly MetadataBuilder _asmBuilder;
        private readonly InternalMethod _method;
        private readonly MethodBodyStreamEncoder _streamEncoder;
        private readonly MethodAttributes _baseAttrs;
        private readonly bool _isDebug;
        private readonly TypeSystemBridge _typeSystem;
        private readonly InstructionEncoder _il = new(new BlobBuilder(), new ControlFlowBuilder());
        private readonly Dictionary<IType, int> _defaultValueHolders = new();
        private readonly List<IType> _locals = new();
        private readonly List<ParameterBuilder> _params = new();
        private readonly MaxStackAnalyzer _maxStackAnalyzer = new(My<TypeSystemServices>.Instance);
        readonly Dictionary<LabelHandle, int> _labelPositions = new();
        readonly List<(int position, LabelHandle[] tokens, Blob buffer)> _switches = new();
        private LocalVariableHandle _firstVar = default;
        private readonly EntityHandle _handle;

        private GenericTypeParameterBuilder[] _genParams;

        public MethodBuilder(InternalMethod method, MethodBodyStreamEncoder encoder, MethodAttributes attrs, bool debug, TypeSystemBridge typeSystem)
            : this(method, encoder, attrs, debug, typeSystem, typeSystem.ReserveMethod(method))
        { }

        public MethodBuilder(InternalMethod method, MethodBodyStreamEncoder encoder, MethodAttributes attrs, bool debug, TypeSystemBridge typeSystem, EntityHandle handle)
        {
            _asmBuilder = typeSystem.AssemblyBuilder;
            _method = method;
            _streamEncoder = encoder;
            _baseAttrs = attrs;
            _isDebug = debug;
            _typeSystem = typeSystem;
            _handle = handle;
        }

        public IEntity Entity => _method;
        public EntityHandle Handle => _handle;

        public void OpCode(ILOpCode code)
        {
            _il.OpCode(code);
            _maxStackAnalyzer.Operation(code);
        }

        public void OpCode(ILOpCode code, EntityHandle handle)
        {
            _il.OpCode(code);
            _il.Token(handle);
            _maxStackAnalyzer.Operation(code);
        }

        public void Branch(ILOpCode code, LabelHandle target)
        {
            _il.Branch(code, target);
            _maxStackAnalyzer.Operation(code);
        }

        public void Switch(LabelHandle[] labels)
        {
            OpCode(ILOpCode.Switch);
            var tokenBlob = _il.CodeBuilder.ReserveBytes((labels.Length + 1) * sizeof(int));
            var switchPos = _il.Offset;
            _switches.Add((switchPos, labels, tokenBlob));
            _maxStackAnalyzer.Operation(ILOpCode.Switch);
        }

        public void Call(ILOpCode op, IMethodBase method)
        {
            _il.OpCode(op);
            var handle = _typeSystem.LookupMethod(method);
            _il.Token(handle);
            _maxStackAnalyzer.Call(op, method);
        }

        public void CallArrayMethod(IType arrayType, string methodName, IType returnType, IType[] parameterTypes)
        {
            var handle = _typeSystem.GetArrayMethod(arrayType, methodName, returnType, parameterTypes);
            _il.OpCode(ILOpCode.Call);
            _il.Token(handle);
            _maxStackAnalyzer.CallArrayMethod(parameterTypes.Length, returnType != _typeSystem.VoidTypeEntity);
        }

        public void LoadConstant(long value)
        {
            _il.LoadConstantI8(value);
            _maxStackAnalyzer.Operation(ILOpCode.Ldc_i8);_maxStackAnalyzer.Operation(ILOpCode.Ldc_i8);
        }

        public void LoadConstant(int value)
        {
            _il.LoadConstantI4(value);
            _maxStackAnalyzer.Operation(ILOpCode.Ldc_i4);
        }

        public void LoadString(string value)
        {
            _il.LoadString(_asmBuilder.GetOrAddUserString(value));
            _maxStackAnalyzer.Operation(ILOpCode.Ldstr);
        }

        public void LoadArgumentAddress(int idx)
        {
            _il.LoadArgumentAddress(idx);
            _maxStackAnalyzer.Operation(ILOpCode.Ldarga);
        }

        public BlobBuilder CodeBuilder => _il.CodeBuilder;

        public LabelHandle DefineLabel() => _il.DefineLabel();

        public LabelHandle DefineLabelHere()
        {
            var result = _il.DefineLabel();
            _il.MarkLabel(result);
            return result;
        }

        public void MarkLabel(LabelHandle label)
        {
            _il.MarkLabel(label);
            _labelPositions.Add(label, _il.Offset);
        }

        public void Token(EntityHandle handle) => _il.Token(handle);

        public void LoadLocal(int idx) {
            _il.LoadLocal(idx);
            _maxStackAnalyzer.Operation(ILOpCode.Ldloc);
        }

        public void LoadLocalAddress(int idx) {
            _il.LoadLocalAddress(idx);
            _maxStackAnalyzer.Operation(ILOpCode.Ldloca);
        }

        public void StoreLocal(int temp) {
            _il.StoreLocal(temp);
            _maxStackAnalyzer.Operation(ILOpCode.Stloc);
        }

        public void LoadArgument(int index) {
            _il.LoadArgument(index);
            _maxStackAnalyzer.Operation(ILOpCode.Ldarg);
        }

        public void StoreArgument(int index) {
            _il.StoreArgument(index);
            _maxStackAnalyzer.Operation(ILOpCode.Starg);
        }

        //get the default value holder (a local actually) for a given type
        //default value holder pool is cleared before each method body processing
        public int GetDefaultValueHolder(IType type)
        {
            if (!_defaultValueHolders.TryGetValue(type, out int holder))
            {
                holder = AddLocal(type);
                _defaultValueHolders.Add(type, holder);
            }
            return holder;
        }

        public int AddLocal(IType type, string name = null)
        {
            var result = _locals.Count;
            _locals.Add(type);
            if (_isDebug)
            {
                var lvar = _typeSystem.DebugBuilder.AddLocalVariable(
                    name?.Contains('$') == true ? LocalVariableAttributes.DebuggerHidden : LocalVariableAttributes.None,
                    _locals.Count,
                    name != null ? _typeSystem.DebugBuilder.GetOrAddString(name) : default);
                if (_firstVar.IsNil)
                {
                    _firstVar = lvar;
                }
            }
            return result;
        }

        private readonly Stack<(LabelHandle start, LabelHandle end)> _tryLabels = new();
        private readonly Stack<(LabelHandle start, LabelHandle end, HandlerType type)> _handlers = new();
        private readonly Stack<LabelHandle> _exceptionExitPoints = new();
        private readonly Stack<EntityHandle> _catchType = new();

        private enum HandlerType
        {
            Except,
            Fail,
            Finally,
            FilterExpr,
            FilterBody
        }

        public void BeginTry()
        {
            _tryLabels.Push((DefineLabelHere(), DefineLabel()));
            _exceptionExitPoints.Push(DefineLabel());
        }

        public void EndTryBody()
        {
            Branch(ILOpCode.Leave, _exceptionExitPoints.Peek());
            MarkLabel(_tryLabels.Peek().end);
        }
        
        public void EndTry()
        {
            _tryLabels.Pop();
            MarkLabel(_exceptionExitPoints.Pop());
        }

        public void BeginFail()
        {
            _handlers.Push((DefineLabelHere(), DefineLabel(), HandlerType.Fail));
        }

#pragma warning disable IDE0042 // Deconstruct variable declaration
        public void EndFail()
        {
            var frame = _handlers.Pop();
            if (frame.type != HandlerType.Fail)
            {
                throw new InvalidOperationException("EndFail called without a matching call to BeginFail");
            }
            OpCode(ILOpCode.Endfinally);
            MarkLabel(frame.end);
            var tryFrame = _tryLabels.Peek();
            _il.ControlFlowBuilder.AddFaultRegion(tryFrame.start, tryFrame.end, frame.start, frame.end);
        }

        public void BeginFinally()
        {
            _handlers.Push((DefineLabelHere(), DefineLabel(), HandlerType.Finally));
        }

        public void EndFinally()
        {
            var frame = _handlers.Pop();
            if (frame.type != HandlerType.Finally)
            {
                throw new InvalidOperationException("EndFinally called without a matching call to BeginFinally");
            }
            OpCode(ILOpCode.Endfinally);
            MarkLabel(frame.end);
            var tryFrame = _tryLabels.Peek();
            _il.ControlFlowBuilder.AddFinallyRegion(tryFrame.start, tryFrame.end, frame.start, frame.end);
        }

        public void BeginFilter()
        {
            _handlers.Push((DefineLabelHere(), DefineLabel(), HandlerType.FilterExpr));
        }

        public void BeginFilterBody()
        {
            var frame = _handlers.Peek();
            if (frame.type != HandlerType.FilterExpr)
            {
                throw new InvalidOperationException("BeginFilterBody called without a matching call to BeginFilter");
            }
            OpCode(ILOpCode.Endfilter);
            MarkLabel(frame.end);
            var start = DefineLabelHere();
            var end = DefineLabel();
            _handlers.Push((start, end, HandlerType.FilterBody));
        }

        public void EndFilter()
        {
            var bodyFrame = _handlers.Pop();
            if (bodyFrame.type != HandlerType.FilterBody)
            {
                throw new InvalidOperationException("EndFilter called without a matching call to BeginFilterBody");
            }
            var exprFrame = _handlers.Pop();
            if (exprFrame.type != HandlerType.FilterExpr)
            {
                throw new InvalidOperationException("EndFilter called without a matching call to BeginFilter");
            }
            Branch(ILOpCode.Leave, _exceptionExitPoints.Peek());
            MarkLabel(bodyFrame.end);
            var tryFrame = _tryLabels.Peek();
            _il.ControlFlowBuilder.AddFilterRegion(tryFrame.start, tryFrame.end, bodyFrame.start, bodyFrame.end, exprFrame.start);
        }

        public void BeginCatchBlock(EntityHandle type)
        {
            _handlers.Push((DefineLabelHere(), DefineLabel(), HandlerType.Except));
            _catchType.Push(type);
        }

        public void EndCatchBlock()
        {
            var frame = _handlers.Pop();
            if (frame.type != HandlerType.Except)
            {
                throw new InvalidOperationException("EndCatchBlock called without a matching call to BeginCatchBlock");
            }
            Branch(ILOpCode.Leave, _exceptionExitPoints.Peek());
            MarkLabel(frame.end);
            var type = _catchType.Pop();
            var tryFrame = _tryLabels.Peek();
            _il.ControlFlowBuilder.AddCatchRegion(tryFrame.start, tryFrame.end, frame.start, frame.end, type);
        }
#pragma warning restore IDE0042 // Deconstruct variable declaration

        internal static MethodAttributes GetMethodAttributes(IMethod method)
		{
            var node = (Method)((IInternalEntity)method).Node;
            MethodAttributes attributes = MethodAttributes.HideBySig;
			if (node.ExplicitInfo != null)
				attributes |= MethodAttributes.NewSlot;
			if (method.IsPInvoke)
			{
				Debug.Assert(method.IsStatic);
				attributes |= MethodAttributes.PinvokeImpl;
			}
			attributes |= MethodAttributesFor(node);
			return attributes;
		}

        internal static MethodAttributes MethodAttributesFor(TypeMember method)
        {
            var attributes = MethodVisibilityAttributesFor(method);

            if (method.IsStatic)
            {
                attributes |= MethodAttributes.Static;
                if (method.Name.StartsWith("op_"))
                    attributes |= MethodAttributes.SpecialName;
            }
            else if (method.IsAbstract)
                attributes |= (MethodAttributes.Abstract | MethodAttributes.Virtual);
            else if (method.IsVirtual || method.IsOverride)
            {
                attributes |= MethodAttributes.Virtual;
                if (method.IsFinal)
                    attributes |= MethodAttributes.Final;
                if (method.IsNew)
                    attributes |= MethodAttributes.NewSlot;
            }

            return attributes;
        }

        private static MethodAttributes MethodVisibilityAttributesFor(TypeMember method)
        {
            if (method.IsPublic)
                return MethodAttributes.Public;
            if (method.IsProtected)
                return method.IsInternal ? MethodAttributes.FamORAssem : MethodAttributes.Family;
            if (method.IsInternal)
                return MethodAttributes.Assembly;
            return MethodAttributes.Private;
        }

        private (BlobHandle sig, ParameterHandle[] pHandles) BuildSignature()
        {
            var builder = new BlobEncoder(new BlobBuilder());
            var sig = builder.MethodSignature(
                SignatureCallingConvention.Default,
                _method.GenericInfo?.GenericParameters?.Length ?? 0,
                !_method.IsStatic);
            sig.Parameters(_params.Count, out var ret, out var parms);
            var returnType = _method.ReturnType;
            if ((_method.IsPInvoke && TypeSystemServices.IsUnknown(returnType)) || returnType == _typeSystem.VoidTypeEntity)
            {
                ret.Void();
            }
            else
            {
                _typeSystem.EncodeType(ret.Type(), returnType);
            }
            foreach (var param in _params)
            {
                param.Build();
                var entity = (IParameter)param.Entity;
                _typeSystem.EncodeType(parms.AddParameter().Type(entity.IsByRef), entity.Type);
            }
            var pHandles = _params.Select(p => (ParameterHandle)p.Handle).ToArray();
            return (_asmBuilder.GetOrAddBlob(builder.Builder), pHandles);
        }

        private (int offset, StandaloneSignatureHandle handle) BuildBody()
        {
            StandaloneSignatureHandle sigHandle = default;
            if (_locals.Count > 0)
            {
                var localBuilder = new BlobEncoder(new BlobBuilder());
                var sig = localBuilder.LocalVariableSignature(_locals.Count);
                foreach (var local in _locals)
                {
                    _typeSystem.EncodeType(sig.AddVariable().Type(), local.Type);
                }
                sigHandle = _asmBuilder.AddStandaloneSignature(_asmBuilder.GetOrAddBlob(localBuilder.Builder));
            }
            int stackDepth = _maxStackAnalyzer.Total;
            var offset = _streamEncoder.AddMethodBody(_il, stackDepth, sigHandle);
            return (offset, sigHandle);
        }

        private void CleanupSwitches()
        {
            foreach (var (offset, labels, blob) in _switches)
            {
                var writer = new BlobWriter(blob);
                writer.WriteUInt32((uint)labels.Length);
                foreach (var label in labels)
                {
                    writer.WriteInt32(_labelPositions[label] - offset);
                }
            }
        }

        public ParameterBuilder DefineParameter(IParameter param, int index, ParameterAttributes attributes)
        {
            var result = new ParameterBuilder(param, index, attributes, _typeSystem);
            _params.Add(result);
            return result;
        }

        public GenericTypeParameterBuilder[] DefineGenericParameters(IEnumerable<IGenericParameter> parameters)
        {
            if (_genParams != null)
            {
                throw new EcmaBuildException("Generic parameters have already been defined for this method.");
            }
            var result = parameters.Select(p => new GenericTypeParameterBuilder(p, this, _typeSystem)).ToArray();
            _genParams = result;
            return result;
        }

        private readonly Dictionary<int, LexicalInfo> _sequencePoints = new();

        public void MarkSequencePoint(LexicalInfo info)
        {
            var pos = _il.CodeBuilder.Count;
            _sequencePoints[pos] = info;
        }

        private void BuildSequencePoints(StandaloneSignatureHandle signature)
        {
            var arr = _sequencePoints.OrderBy(kvp => kvp.Key).ToArray();
            if (arr.Length == 0)
            {
                _typeSystem.DebugBuilder.AddMethodDebugInformation(default, default);
                return;
            }
            var multiDoc = arr.Select(kvp => kvp.Value.FileName).Where(f => !string.IsNullOrEmpty(f)).Distinct().Count() > 1;
            var builder = new BlobBuilder();
            builder.WriteCompressedInteger(MetadataTokens.GetRowNumber(signature));
            var first = arr.FirstOrDefault(sp => !string.IsNullOrEmpty(sp.Value.FileName)).Value;
            if (first == null) {
                return;
            }
            var docHandle = _typeSystem.LookupDocument(first.FileName);
            if (multiDoc)
            {
                builder.WriteCompressedInteger(MetadataTokens.GetRowNumber(docHandle));
            }
            var last = arr[0];
            builder.WriteCompressedInteger(last.Key);
            builder.WriteCompressedInteger(1);
            builder.WriteCompressedSignedInteger(-last.Value.Column);
            builder.WriteCompressedInteger(last.Value.Line);
            builder.WriteCompressedInteger(last.Value.Column);
            for (int i = 1; i < arr.Length; i++)
            {
                var next = arr[i];
                if (multiDoc && next.Value.FileName != last.Value.FileName)
                {
                    builder.WriteCompressedInteger(0);
                    docHandle = _typeSystem.LookupDocument(next.Value.FileName);
                    builder.WriteCompressedInteger(MetadataTokens.GetRowNumber(docHandle));
                }
                builder.WriteCompressedInteger(next.Key - last.Key);
                builder.WriteCompressedInteger(1);
                builder.WriteCompressedSignedInteger(-next.Value.Column);
                builder.WriteCompressedInteger(Math.Max(next.Value.Line - last.Value.Line, 1));
                builder.WriteCompressedSignedInteger(next.Value.Column - last.Value.Column);
                last = next;
            }
            var seqBlob = _typeSystem.DebugBuilder.GetOrAddBlob(builder);
            _typeSystem.DebugBuilder.AddMethodDebugInformation(
                multiDoc ? default : _typeSystem.LookupDocument(arr[0].Value.FileName),
                seqBlob);
        }

        public void Build()
        {
            CleanupSwitches();
            var (bodyOffset, localSig) = BuildBody();
            var (sig, pHandles) = BuildSignature();
            var explicitInfo = ((Method)_method.Node).ExplicitInfo;
            var name = _method is IConstructor ? 
                (_method.IsStatic ? ".cctor" : ".ctor") :
                (explicitInfo != null ? explicitInfo.InterfaceType.Name + "." + _method.Name : _method.Name);
            var isRuntime = ((Method)_method.Node).ImplementationFlags.HasFlag(MethodImplementationFlags.Runtime);
            var handle = _asmBuilder.AddMethodDefinition(
                GetMethodAttributes(_method) | _baseAttrs,
                isRuntime ? MethodImplAttributes.Runtime : MethodImplAttributes.IL,
                _asmBuilder.GetOrAddString(name),
                sig,
                isRuntime || _method.IsAbstract ? -1 : bodyOffset,
                pHandles.Length > 0
                    ? pHandles[0]
                    : MetadataTokens.ParameterHandle(_asmBuilder.GetRowCount(TableIndex.Param) + 1));
            if (handle != _handle)
            {
                throw new EcmaBuildException($"Method build handle {handle} does not match reserved handle {_handle}.");
            }
            if (_isDebug)
            {
                BuildSequencePoints(localSig);
                var db = _typeSystem.DebugBuilder;
                db.AddLocalScope(handle, _typeSystem.NullImportHandle, _firstVar, default, bodyOffset, _il.CodeBuilder.Count);
            }
        }
    }
}
