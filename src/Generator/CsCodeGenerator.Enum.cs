// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private static readonly char[] separator = ['_'];

    private static readonly Dictionary<string, string> s_knownEnumPrefixes = new()
    {
        { "SDL_PropertyType", "SDL_PROPERTY_TYPE" },
        { "SDL_Scancode", "SDL_SCANCODE" },
        { "SDL_Keycode", "SDLK" },
        { "SDL_Keymod", "SDL_KMOD" },
        { "SDL_GamepadType", "SDL_GAMEPAD_TYPE" },
        { "SDL_GamepadButton", "SDL_GAMEPAD_BUTTON" },
        { "SDL_GamepadAxis", "SDL_GAMEPAD_AXIS" },
        { "SDL_GamepadBindingType", "SDL_GAMEPAD_BINDTYPE" },
        { "SDL_errorcode", "SDL" },
        { "SDL_InitFlags", "SDL_INIT" },
        { "SDL_MessageBoxFlags", "SDL_MESSAGEBOX" },
        { "SDL_MessageBoxColorType", "SDL_MESSAGEBOX_COLOR" },
        { "SDL_JoystickType", "SDL_JOYSTICK_TYPE" },
        { "SDL_JoystickConnectionState", "SDL_JOYSTICK_CONNECTION" },

        { "SDL_SystemCursor", "SDL_SYSTEM_CURSOR" },
        { "SDL_MouseWheelDirection", "SDL_MOUSEWHEEL" },
        { "SDL_TouchDeviceType", "SDL_TOUCH_DEVICE" },
        { "SDL_SystemTheme", "SDL_SYSTEM_THEME" },
        { "SDL_DisplayOrientation", "SDL_ORIENTATION" },
        { "SDL_WindowFlags", "SDL_WINDOW" },
        { "SDL_LogCategory", "SDL_LOG_CATEGORY" },
        { "SDL_LogPriority", "SDL_LOG_PRIORITY" },
        { "SDL_PowerState", "SDL_POWERSTATE" },
        { "SDL_SensorType", "SDL_SENSOR" },
        { "SDL_FlashOperation", "SDL_FLASH" },
        { "SDL_GLAttr", "SDL_GL" },
        { "SDL_GLContextFlag", "SDL_GL_CONTEXT" },
        { "SDL_GLContextReleaseFlag", "SDL_GL_CONTEXT_RELEASE_BEHAVIOR" },
        { "SDL_GLContextResetNotification", "SDL_GL_CONTEXT_RESET" },
        { "SDL_GLProfile", "SDL_GL_CONTEXT_PROFILE" },
        { "SDL_HitTestResult", "SDL_HITTEST" },
        { "SDL_EventType", "SDL_EVENT" },
        { "SDL_SYSWM_TYPE", "SDL_SYSWM" },
        { "SDL_HintPriority", "SDL_HINT" },
        { "SDL_BlendMode", "SDL_BLENDMODE" },
        { "SDL_BlendOperation", "SDL_BLENDOPERATION" },
        { "SDL_BlendFactor", "SDL_BLENDFACTOR" },
        { "SDL_GamepadButtonLabel", "SDL_GAMEPAD_BUTTON_LABEL" },
        { "SDL_PenAxis", "SDL_PEN_AXIS" },
        { "SDL_PenSubtype", "SDL_PEN_TYPE" },
        { "SDL_AudioFormat", "SDL_AUDIO" },

        // SDL_pixels.h
        { "SDL_PixelType", "SDL_PIXELTYPE" },
        { "SDL_BitmapOrder", "SDL_BITMAPORDER" },
        { "SDL_PackedOrder", "SDL_PACKEDORDER" },
        { "SDL_ArrayOrder", "SDL_ARRAYORDER" },
        { "SDL_PackedLayout", "SDL_PACKEDLAYOUT" },
        { "SDL_PixelFormat", "SDL_PIXELFORMAT" },
        { "SDL_ColorType", "SDL_COLOR_TYPE" },
        { "SDL_ColorRange", "SDL_COLOR_RANGE" },
        { "SDL_ColorPrimaries", "SDL_COLOR_PRIMARIES" },
        { "SDL_TransferCharacteristics", "SDL_TRANSFER_CHARACTERISTICS" },
        { "SDL_MatrixCoefficients", "SDL_MATRIX_COEFFICIENTS" },
        { "SDL_ChromaLocation", "SDL_CHROMA_LOCATION" },
        { "SDL_Colorspace", "SDL_COLORSPACE" },

        { "SDL_ScaleMode", "SDL_SCALEMODE" },
        { "SDL_FlipMode", "SDL_FLIP" },
        { "SDL_YUV_CONVERSION_MODE", "SDL_YUV_CONVERSION" },

        { "SDL_CameraPosition", "SDL_CAMERA_POSITION" },

        { "SDL_IOStatus", "SDL_IO_STATUS" },
        { "SDL_DateFormat", "SDL_DATE_FORMAT" },
        { "SDL_TimeFormat", "SDL_TIME_FORMAT" },

        { "SDL_AppResult", "SDL_APP" },

        { "SDL_WinRT_Path", "SDL_WINRT_PATH" },
        { "SDL_WinRT_DeviceFamily", "SDL_WINRT_DEVICEFAMILY" },

        // SDL_GPU
        { "SDL_GPUPrimitiveType", "SDL_GPU_PRIMITIVETYPE" },
        { "SDL_GPULoadOp", "SDL_GPU_LOADOP" },
        { "SDL_GPUStoreOp", "SDL_GPU_STOREOP" },
        { "SDL_GPUIndexElementSize", "SDL_GPU_INDEXELEMENTSIZE" },
        { "SDL_GPUTextureFormat", "SDL_GPU_TEXTUREFORMAT" },
        { "SDL_GPUVertexInputRate", "SDL_GPU_VERTEXINPUTRATE" },
        { "SDL_GPUTextureType", "SDL_GPU_TEXTURETYPE" },
        { "SDL_GPUSampleCount", "SDL_GPU_SAMPLECOUNT" },
        { "SDL_GPUCubeMapFace", "SDL_GPU_CUBEMAPFACE" },
        { "SDL_GPUTransferBufferUsage", "SDL_GPU_TRANSFERBUFFERUSAGE" },
        { "SDL_GPUShaderStage", "SDL_GPU_SHADERSTAGE" },
        { "SDL_GPUVertexElementFormat", "SDL_GPU_VERTEXELEMENTFORMAT" },
        { "SDL_GPUFillMode", "SDL_GPU_FILLMODE" },
        { "SDL_GPUCullMode", "SDL_GPU_CULLMODE" },
        { "SDL_GPUFrontFace", "SDL_GPU_FRONTFACE" },
        { "SDL_GPUCompareOp", "SDL_GPU_COMPAREOP" },
        { "SDL_GPUStencilOp", "SDL_GPU_STENCILOP" },
        { "SDL_GPUBlendOp", "SDL_GPU_BLENDOP" },
        { "SDL_GPUBlendFactor", "SDL_GPU_BLENDFACTOR" },
        { "SDL_GPUFilter", "SDL_GPU_FILTER" },
        { "SDL_GPUSamplerMipmapMode", "SDL_GPU_SAMPLERMIPMAPMODE" },
        { "SDL_GPUSamplerAddressMode", "SDL_GPU_SAMPLERADDRESSMODE" },
        { "SDL_GPUPresentMode", "SDL_GPU_PRESENTMODE" },
        { "SDL_GPUSwapchainComposition", "SDL_GPU_SWAPCHAINCOMPOSITION" },
        { "SDL_GPUDriver", "SDL_GPU_DRIVER" },

        { "SDL_AsyncIOTaskType", "SDL_ASYNCIO_TASK" },
        { "SDL_AsyncIOResult", "SDL_ASYNCIO" },
        { "SDL_FileDialogType", "SDL_FILEDIALOG" },
        { "SDL_ThreadPriority", "SDL_THREAD_PRIORITY" },
        { "SDL_ThreadState", "SDL_THREAD" },
    };

    private static readonly Dictionary<string, string> s_knownEnumValueNames = new()
    {
        { "SDL_NUM_SCANCODES", "NumScancodes" },
        { "SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT", "ReturnKeyDefault" },
        { "SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT", "EscapeKeyDefault" },
        //
        { "SDL_ADDEVENT", "AddEvent" },
        { "SDL_PEEKEVENT", "PeekEvent" },
        { "SDL_GETEVENT", "GetEvent" },
        { "SDL_PEN_NUM_AXES", "NumAxes" },
        // SDL_GPU
        { "SDL_GPU_INDEXELEMENTSIZE_16BIT", "UInt16" },
        { "SDL_GPU_INDEXELEMENTSIZE_32BIT", "UInt32" },
        { "SDL_GPU_TEXTURETYPE_2D", "Type2D" },
        { "SDL_GPU_TEXTURETYPE_2D_ARRAY", "Type2DArray" },
        { "SDL_GPU_TEXTURETYPE_3D", "Type3D" },
        { "SDL_GPU_TEXTURETYPE_CUBE", "Cube" },
        { "SDL_GPU_SAMPLECOUNT_1", "Count1" },
        { "SDL_GPU_SAMPLECOUNT_2", "Count2" },
        { "SDL_GPU_SAMPLECOUNT_4", "Count4" },
        { "SDL_GPU_SAMPLECOUNT_8", "Count8" },
        { "SDL_GPU_SAMPLECOUNT_16", "Count16" },
        { "SDL_GPU_SAMPLECOUNT_32", "Count32" },
    };

    private readonly HashSet<string> _enumConstants = [];

    private static readonly HashSet<string> s_ignoredParts = new(StringComparer.OrdinalIgnoreCase)
    {
    };

    private static readonly HashSet<string> s_preserveCaps = new(StringComparer.OrdinalIgnoreCase)
    {
        "sdl",
        "gpu",
        "sdr",
        "hdr",
        "d3d11",
        "d3d12",
    };

    private static readonly Dictionary<string, string> s_partRenames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "lctrl", "LeftControl" },
        { "Lshift", "LeftShift" },
        { "Lalt", "LeftAlt" },
        { "Lgui", "LeftGui" },
        { "Rctrl", "RightControl" },
        { "Rshift", "RightShift" },
        { "Ralt", "RightAlt" },
        { "Rgui", "RightGui" },
    };

    private static readonly HashSet<string> s_partRenamesSet = new(StringComparer.OrdinalIgnoreCase)
    {
        "LeftBracket",
        "RightBracket",
        "Backslash",
        "NonusHash",
        "Capslock",
        "PrintScreen",
        "ScrollLock",
        "PageUp",
        "PageDown",
        "NumLockClear",
        "VolumeUp",
        "VolumeDown",
        "AudioPrev",
        "AudioNext",
        "AudioStop",
        "AudioPlay",
        "AudioMute",
        "MediaSelect",
        "DisplaySwitch",
        "MultisampleBuffers",
        "MultisampleSamples",
        "OpenGL",
        "ArrayU8",
        "ArrayU16",
        "ArrayU32",
        "ArrayF16",
        "ArrayF32",
        "CaseInsensitive",
        "LeftParen",
        "RightParen",
        "BackSlash",
        "PlusMinus",
        "CapsLock",
        "MetalLib",
        "PointList",
        "LineList",
        "LineStrip",
        "TriangleList",
        "TriangleStrip",
        "PositiveX",
        "PositiveY",
        "PositiveZ",
        "NegativeX",
        "NegativeY",
        "NegativeZ",
        "UInt",
        "UInt2",
        "UInt3",
        "UInt4",
        "UByte2",
        "UByte4",
        "UShort2",
        "UShort2",
        "OpenFile",
        "SaveFile",
        "OpenFolder",
    };

    public void CollectEnums(CppCompilation compilation)
    {
        foreach (CppEnum cppEnum in compilation.Enums)
        {
            if (string.IsNullOrEmpty(cppEnum.Name))
            {
                continue;
            }

            _collectedEnums.Add(cppEnum);
        }
    }

    public void GenerateEnums()
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Enums.cs"), false, _options.Namespace, ["System"]);
        Dictionary<string, string> createdEnums = [];

        foreach (CppEnum cppEnum in _collectedEnums)
        {
            bool isBitmask =
                cppEnum.Name.EndsWith("Flags") ||
                cppEnum.Name == "SDL_Keymod" ||
                cppEnum.Name == "SDL_InitFlags" ||
                cppEnum.Name == "SDL_GLprofile" ||
                cppEnum.Name == "SDL_GLcontextFlag" ||
                cppEnum.Name == "SDL_GLcontextReleaseFlag" ||
                cppEnum.Name == "SDL_GLContextResetNotification" ||
                cppEnum.Name == "SDL_BlendMode";

            writer.WriteComment(cppEnum.Comment?.ChildrenToString() ?? string.Empty);

            if (isBitmask)
            {
                writer.WriteLine("[Flags]");
            }

            string enumCsName = GetCsCleanName(cppEnum.Name);
            string enumNamePrefix = GetEnumNamePrefix(cppEnum.Name);

            createdEnums.Add(enumCsName, cppEnum.Name);

            string baseTypeName = string.Empty;
            if (enumCsName == "SDL_WindowFlags")
            {
                baseTypeName = " : ulong";
            }
            else if (enumCsName == "SDL_AudioFormat"
                || enumCsName == "SDL_PixelFormat")
            {
                baseTypeName = " : uint";
            }

            bool noneAdded = false;
            using (writer.PushBlock($"{visibility} enum {enumCsName}{baseTypeName}"))
            {
                if (isBitmask &&
                    !cppEnum.Items.Any(enumItem => GetEnumItemName(cppEnum.Name, enumItem.Name, enumNamePrefix) == "None"))
                {
                    writer.WriteLine("None = 0,");
                    noneAdded = true;
                }

                foreach (CppEnumItem enumItem in cppEnum.Items)
                {
                    if (enumItem.Name.EndsWith("_BEGIN_RANGE") ||
                        enumItem.Name.EndsWith("_END_RANGE") ||
                        enumItem.Name.EndsWith("_RANGE_SIZE") ||
                        enumItem.Name.EndsWith("_Force32") ||
                        enumItem.Name.EndsWith("_RESERVED")
                        )
                    {
                        continue;
                    }

                    string enumItemName = GetEnumItemName(cppEnum.Name, enumItem.Name, enumNamePrefix);

                    if (enumItemName == "None" && noneAdded)
                    {
                        continue;
                    }

                    if (enumItemName == "Default")
                    {
                        continue;
                    }

                    writer.WriteComment(enumItem.Comment?.ChildrenToString() ?? string.Empty);

                    if (enumItemName != "Count" && _options.EnumWriteUnmanagedTag)
                    {
                        writer.WriteLine($"/// <unmanaged>{enumItem.Name}</unmanaged>");
                    }

                    if (enumItem.ValueExpression is CppRawExpression rawExpression)
                    {
                        if (string.IsNullOrEmpty(rawExpression.Text)
                            || enumCsName == "SDL_Colorspace")
                        {
                            writer.WriteLine($"{enumItemName} = {enumItem.Value},");
                        }
                        else if (rawExpression.Text.Contains("0x"))
                        {
                            writer.WriteLine($"{enumItemName} = {rawExpression.Text},");
                        }
                        else
                        {
                            string enumValueName = GetEnumItemName(cppEnum.Name, rawExpression.Text, enumNamePrefix);
                            writer.WriteLine($"{enumItemName} = {enumValueName},");
                        }
                    }
                    else if (enumItem.ValueExpression is CppLiteralExpression literalExpression)
                    {
                        writer.WriteLine($"{enumItemName} = {literalExpression.Value},");
                    }
                    else if (enumItem.ValueExpression is CppBinaryExpression binaryExpression)
                    {
                        StringBuilder builder = new();
                        FormatCppBinaryExpression(cppEnum, binaryExpression, builder, enumNamePrefix);
                        writer.WriteLine($"{enumItemName} = {builder},");
                    }
                    else
                    {
                        writer.WriteLine($"{enumItemName} = {enumItem.Value},");
                    }

                    _enumConstants.Add($"{enumCsName} {enumItem.Name} = {enumCsName}.{enumItemName}");
                }
            }

            writer.WriteLine();
        }
    }

    private static void FormatExpression(CppEnum @enum, CppExpression expression, StringBuilder builder, string enumNamePrefix)
    {
        if (expression is CppRawExpression rawExpression)
        {
            builder.Append(GetEnumItemName(@enum.Name, rawExpression.Text, enumNamePrefix));
        }
        else if (expression is CppLiteralExpression literalExpression)
        {
            builder.Append(literalExpression.Value);
        }
        else if (expression is CppBinaryExpression binaryExpression)
        {
            FormatCppBinaryExpression(@enum, binaryExpression, builder, enumNamePrefix);
        }
    }

    private static void FormatCppBinaryExpression(CppEnum @enum, CppBinaryExpression expression, StringBuilder builder, string enumNamePrefix)
    {
        if (expression.Arguments != null && expression.Arguments.Count > 0)
        {
            FormatExpression(@enum, expression.Arguments[0], builder, enumNamePrefix);
        }

        builder.Append(' ');
        builder.Append(expression.Operator);
        builder.Append(' ');

        if (expression.Arguments != null && expression.Arguments.Count > 1)
        {
            FormatExpression(@enum, expression.Arguments[1], builder, enumNamePrefix);
        }
    }

    public static string GetEnumNamePrefix(string typeName)
    {
        if (s_knownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
        {
            return knownValue;
        }

        List<string> parts = new(4);
        int chunkStart = 0;
        for (int i = 0; i < typeName.Length; i++)
        {
            if (char.IsUpper(typeName[i]))
            {
                if (chunkStart != i)
                {
                    parts.Add(typeName.Substring(chunkStart, i - chunkStart));
                }

                chunkStart = i;
                if (i == typeName.Length - 1)
                {
                    parts.Add(typeName.Substring(i, 1));
                }
            }
            else if (i == typeName.Length - 1)
            {
                parts.Add(typeName.Substring(chunkStart, typeName.Length - chunkStart));
            }
        }

        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i] == "Flag" ||
                parts[i] == "Flags" ||
                (parts[i] == "K" && (i + 2) < parts.Count && parts[i + 1] == "H" && parts[i + 2] == "R") ||
                (parts[i] == "A" && (i + 2) < parts.Count && parts[i + 1] == "M" && parts[i + 2] == "D") ||
                (parts[i] == "E" && (i + 2) < parts.Count && parts[i + 1] == "X" && parts[i + 2] == "T") ||
                (parts[i] == "Type" && (i + 2) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V") ||
                (parts[i] == "Type" && (i + 3) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" && parts[i + 3] == "X") ||
                (parts[i] == "Scope" && (i + 2) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V") ||
                (parts[i] == "Mode" && (i + 2) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V") ||
                (parts[i] == "Mode" && (i + 5) < parts.Count && parts[i + 1] == "I" && parts[i + 2] == "N" && parts[i + 3] == "T" && parts[i + 4] == "E" && parts[i + 5] == "L") ||
                (parts[i] == "Type" && (i + 5) < parts.Count && parts[i + 1] == "I" && parts[i + 2] == "N" && parts[i + 3] == "T" && parts[i + 4] == "E" && parts[i + 5] == "L")
                )
            {
                parts = new List<string>(parts.Take(i));
                break;
            }
        }

        return string.Join("_", parts.Select(s => s.ToUpper()));
    }

    private static string GetEnumItemName(string enumName, string cppEnumItemName, string enumNamePrefix)
    {
        string enumItemName;
        if (enumName == "SDL_GPUTextureFormat")
        {
            enumItemName = cppEnumItemName.Substring(enumNamePrefix.Length + 1);
            string[] splits = enumItemName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length <= 1)
            {
                enumItemName = char.ToUpperInvariant(enumItemName[0]) + enumItemName.Substring(1).ToLowerInvariant();
            }
            else
            {
                StringBuilder sb = new();
                foreach (string part in splits)
                {
                    if (part.Equals("UNORM", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Unorm");
                    }
                    else if (part.Equals("SNORM", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Snorm");
                    }
                    else if (part.Equals("UINT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Uint");
                    }
                    else if (part.Equals("INT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Int");
                    }
                    else if (part.Equals("FLOAT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Float");
                    }
                    else if (part.Equals("UFLOAT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Ufloat");
                    }
                    else if (part.Equals("SRGB", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Srgb");
                    }
                    else if (part.Equals("BC1", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC1");
                    }
                    else if (part.Equals("BC2", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC2");
                    }
                    else if (part.Equals("BC3", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC3");
                    }
                    else if (part.Equals("BC4", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC4");
                    }
                    else if (part.Equals("BC5", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC5");
                    }
                    else if (part.Equals("BC6H", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC6H");
                    }
                    else if (part.Equals("BC7", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("BC7");
                    }
                    else if (part.Equals("ETC2", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Etc2");
                    }
                    else if (part.Equals("EAC", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Eac");
                    }
                    else if (part.Equals("ASTC", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Astc");
                    }
                    else if (part.Equals("RGBA", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("RGBA");
                    }
                    else if (part.Equals("RGB", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("RGB");
                    }
                    else
                    {
                        sb.Append(part);
                    }
                }

                enumItemName = sb.ToString();
            }

            return enumItemName;
        }

        enumItemName = GetPrettyEnumName(cppEnumItemName, enumNamePrefix);
        if (char.IsNumber(enumItemName[0]))
        {
            if (enumName == "SDL_Keycode")
            {
                return $"D{enumItemName}";
            }

            return $"_{enumItemName}";
        }

        return enumItemName;
    }

    private static string GetPrettyEnumName(string value, string enumPrefix)
    {
        if (s_knownEnumValueNames.TryGetValue(value, out string? knownName))
        {
            return knownName;
        }

        if (!value.StartsWith(enumPrefix))
        {
            return value;
        }

        string[] parts = value[enumPrefix.Length..].Split(separator, StringSplitOptions.RemoveEmptyEntries);
        return PrettifyName(parts, enumPrefix);
    }

    private static string PrettifyName(string[] parts, string? enumPrefix = default)
    {
        StringBuilder sb = new();
        foreach (string part in parts)
        {
            if (s_ignoredParts.Contains(part))
            {
                continue;
            }

            if (s_preserveCaps.Contains(part))
            {
                sb.Append(part);
            }
            else if (s_partRenames.TryGetValue(part.ToLowerInvariant(), out string? partRemap))
            {
                sb.Append(partRemap!);
            }
            else if (s_partRenamesSet.Contains(part))
            {
                partRemap = s_partRenamesSet.First(item => item.Equals(part, StringComparison.OrdinalIgnoreCase));
                sb.Append(partRemap!);
            }
            else
            {
                sb.Append(char.ToUpper(part[0]));
                for (int i = 1; i < part.Length; i++)
                {
                    sb.Append(char.ToLower(part[i]));
                }
            }
        }

        string prettyName = sb.ToString();
        if (!string.IsNullOrEmpty(enumPrefix))
        {
            if (char.IsNumber(prettyName[0]))
            {
                if (enumPrefix.EndsWith("_IDC"))
                {
                    return "Idc" + prettyName;
                }

                if (enumPrefix.EndsWith("_POC_TYPE"))
                {
                    return "Type" + prettyName;
                }

                if (enumPrefix.EndsWith("_CTB_SIZE"))
                {
                    return "Size" + prettyName;
                }

                if (enumPrefix.EndsWith("_BLOCK_SIZE"))
                {
                    return "Size" + prettyName;
                }

                if (enumPrefix.EndsWith("_FIXED_RATE"))
                {
                    return "Rate" + prettyName;
                }

                if (enumPrefix.EndsWith("_SUBSAMPLING"))
                {
                    return "Subsampling" + prettyName;
                }

                if (enumPrefix.EndsWith("_BIT_DEPTH"))
                {
                    return "Depth" + prettyName;
                }

                return "_" + prettyName;
            }
        }

        return prettyName;
    }

    private static string NormalizeEnumValue(string value)
    {
        if (value == "(~0U)")
        {
            return "~0u";
        }

        if (value == "(~0ULL)")
        {
            return "~0ul";
        }

        if (value == "(~0U-1)")
        {
            return "~0u - 1";
        }

        if (value == "(~0U-2)")
        {
            return "~0u - 2";
        }

        if (value == "(~0U-3)")
        {
            return "~0u - 3";
        }

        if (value.StartsWith("(") && value.EndsWith(")"))
        {
            value = value.Substring(1, value.Length - 2);
        }

        return value.Replace("ULL", "UL").Replace("LL", "L");
    }
}
