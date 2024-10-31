// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private void CollectStructAndUnions(CppCompilation compilation)
    {
        foreach (CppClass? cppClass in compilation.Classes)
        {
            if (cppClass.ClassKind == CppClassKind.Class ||
                cppClass.SizeOf == 0 ||
                cppClass.Name.EndsWith("_T"))
            {
                continue;
            }

            // Handled manually.
            if (cppClass.Name == "SDL_SysWMinfo" || cppClass.Name == "SDL_GUID")
            {
                continue;
            }

            _collectedStructAndUnions.Add(cppClass);
        }
    }

    private void GenerateStructAndUnions()
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";

        // Generate Structures
        using var writer = new CodeWriter(Path.Combine(_options.OutputPath, "Structs.cs"),
            false,
            _options.Namespace,
            [
                "System.Runtime.InteropServices",
                "System.Runtime.CompilerServices",
                "System.Diagnostics.CodeAnalysis",
                "System.Drawing"
            ],
            "#pragma warning disable CS0649"
            );

        // Print All classes, structs
        foreach (CppClass? cppClass in _collectedStructAndUnions)
        {
            bool isUnion = cppClass.ClassKind == CppClassKind.Union;

            string csName = cppClass.Name;
            WriteStruct(writer, cppClass, csName);
            writer.WriteLine();
        }
    }

    private void WriteStruct(CodeWriter writer, CppClass @struct, string structName)
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";
        bool isUnion = @struct.ClassKind == CppClassKind.Union;
        bool isReadOnly = false;
        string typeName = string.Empty;
        if (structName.StartsWith("SDL_Event") || structName.EndsWith("Event"))
        {
            typeName = "SDL_EventType";
        }
        else if (structName.StartsWith("SDL_HapticEffect"))
        {
            typeName = "SDL_HapticEffectType";
        }
        else if (structName.StartsWith("SDL_HapticDirection"))
        {
            typeName = "SDL_HapticDirectionType";
        }

        writer.WriteComment(@struct.Comment?.ChildrenToString() ?? string.Empty);

        if (isUnion)
        {
            writer.WriteLine("[StructLayout(LayoutKind.Explicit)]");
        }

        using (writer.PushBlock($"{visibility} partial struct {structName}"))
        {
            foreach (CppField cppField in @struct.Fields)
            {
                if (structName == "SDL_StorageInterface")
                {
                }

                WriteField(writer, cppField, isUnion, isReadOnly, typeName);
            }
        }
    }

    private void WriteField(CodeWriter writer, CppField field,
        bool isUnion = false,
        bool isReadOnly = false,
        string typeName = "")
    {
        string csFieldName = NormalizeFieldName(field.Name);

        writer.WriteComment(field.Comment?.ChildrenToString() ?? string.Empty);

        if (isUnion)
        {
            writer.WriteLine("[FieldOffset(0)]");
        }

        //writer.WriteLine($"[NativeTypeName({field.ToString()})]");
        if (field.Type is CppArrayType arrayType)
        {
            bool canUseFixed = false;
            if (arrayType.ElementType is CppPrimitiveType)
            {
                canUseFixed = true;
            }
            else if (arrayType.ElementType is CppTypedef typedef
                && typedef.ElementType is CppPrimitiveType)
            {
                canUseFixed = true;
            }

            if (canUseFixed)
            {
                string csFieldType = GetCsTypeName(arrayType.ElementType);
                writer.WriteLine($"public unsafe fixed {csFieldType} {csFieldName}[{arrayType.Size}];");
            }
            else
            {
                string csFieldType;
                if (arrayType.ElementType is CppArrayType elementArrayType)
                {
                    csFieldType = GetCsTypeName(elementArrayType.ElementType);
                    writer.WriteLine($"public unsafe fixed {csFieldType} {csFieldName}[{arrayType.Size} * {elementArrayType.Size}];");
                }
                else if (arrayType.ElementType is CppTypedef cppTypedef)
                {
                    csFieldType = GetCsTypeName(cppTypedef.ElementType);
                    writer.WriteLine($"public unsafe fixed {csFieldType} {csFieldName}[{arrayType.Size}];");
                }
                else
                {
                    csFieldType = GetCsTypeName(arrayType.ElementType);

                    writer.WriteLine($"public {csFieldName}__FixedBuffer {csFieldName};");
                    writer.WriteLine();

                    using (writer.PushBlock($"public unsafe struct {csFieldName}__FixedBuffer"))
                    {
                        for (int i = 0; i < arrayType.Size; i++)
                        {
                            writer.WriteLine($"public {csFieldType} e{i};");
                        }
                        writer.WriteLine();

                        writer.WriteLine("[UnscopedRef]");
                        using (writer.PushBlock($"public ref {csFieldType} this[int index]"))
                        {
                            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            using (writer.PushBlock("get"))
                            {
                                if (csFieldType.EndsWith('*'))
                                {
                                    using (writer.PushBlock($"fixed ({csFieldType}* pThis = &e0)"))
                                    {
                                        writer.WriteLine($"return ref pThis[index];");
                                    }
                                }
                                else
                                {
                                    writer.WriteLine($"return ref AsSpan()[index];");
                                }
                            }
                        }
                        writer.WriteLine();

                        if (!csFieldType.EndsWith('*'))
                        {
                            writer.WriteLine("[UnscopedRef]");
                            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            using (writer.PushBlock($"public Span<{csFieldType}> AsSpan()"))
                            {
                                writer.WriteLine($"return MemoryMarshal.CreateSpan(ref e0, {arrayType.Size});");
                            }
                        }
                    }
                }
            }
        }
        else if (field.Type is CppClass cppClass && cppClass.IsAnonymous)
        {
            if (cppClass.IsAnonymous)
            {
                string fullParentName = field.FullParentName;
                if (fullParentName.EndsWith("::"))
                {
                    fullParentName = fullParentName.Substring(0, fullParentName.Length - 2);
                }
                string csFieldType = $"{fullParentName}_{csFieldName}";
                writer.WriteLine($"public {csFieldType} {csFieldName};");
                writer.WriteLine("");

                WriteStruct(writer, cppClass, csFieldType);
            }
            else
            {

            }
        }
        else
        {
            // VkAllocationCallbacks members
            if (field.Type is CppPointerType pointerType &&
                pointerType.ElementType is CppFunctionType functionType)
            {
                StringBuilder builder = new();
                foreach (CppParameter parameter in functionType.Parameters)
                {
                    string paramCsType = GetCsTypeName(parameter.Type);
                    builder.Append(paramCsType).Append(", ");
                }

                string returnCsName = GetCsTypeName(functionType.ReturnType);
                builder.Append(returnCsName);
                string callingConventionCall = string.Empty;
                if (!string.IsNullOrEmpty(_options.CallingConvention))
                {
                    callingConventionCall = $"[{_options.CallingConvention}]";
                }

                writer.WriteLine($"public unsafe delegate* unmanaged{callingConventionCall}<{builder}> {csFieldName};");
                return;
            }

            string csFieldType = GetCsTypeName(field.Type);
            //if (field.Type is CppPointerType
            //    && csFieldType.EndsWith("*") == false
            //    && s_collectedHandles.ContainsKey(csFieldType) == false)
            //{
            //    csFieldType += "*";
            //}

            string fieldPrefix = isReadOnly ? "readonly " : string.Empty;
            if (csFieldType.EndsWith('*'))
            {
                fieldPrefix += "unsafe ";
            }

            if (csFieldName == "type"
                || csFieldName == "subsystem")
            {
                if (!string.IsNullOrEmpty(typeName))
                {
                    csFieldType = typeName;
                }
            }

            //if (field.Comment is not null && string.IsNullOrEmpty(field.Comment.ToString()) == false)
            //{
            //    writer.WriteLine($"/// <summary>{field.Comment}</summary>");
            //}

            writer.WriteLine($"public {fieldPrefix}{csFieldType} {csFieldName};");
        }
    }
}
