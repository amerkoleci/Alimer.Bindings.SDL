// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_ignoreConstants = new(StringComparer.OrdinalIgnoreCase)
    {
        "SDL_FALSE",
        "SDL_TRUE",
        "alloca",
        "SDL_HAS_BUILTIN",
        "SDL_arraysize",
        "SDL_STRINGIFY_ARG",
        "SDL_reinterpret_cast",
        "SDL_static_cast",
        "SDL_const_cast",
        "SDL_SIZE_MAX",
        "SDL_SINT64_C",
        "SDL_UINT64_C",
        "SDL_FOURCC",
        "SDL_memcpy",
        "SDL_memset",
        "SDL_zero",
        "SDL_zerop",
        "SDL_zeroa",
        "SDL_min",
        "SDL_max",
        "SDL_clamp",
        "SDL_copyp",
        "SDL_memmove",
        "SDL_stack_alloc",
        "SDL_IN_BYTECAP",
        "SDL_INOUT_Z_CAP",
        "SDL_OUT_Z_CAP",
        "SDL_OUT_CAP",
        "SDL_OUT_BYTECAP",
        "SDL_OUT_Z_BYTECAP",
        "SDL_SCANF_FORMAT_STRING",
        "SDL_COMPILE_TIME_ASSERT",
        "SDL_CompilerBarrier",
        "SDL_MemoryBarrierRelease",
        "SDL_MemoryBarrierAcquire",
        "SDL_CPUPauseInstruction",
        "SDL_AtomicIncRef",
        "SDL_AtomicDecRef",
        "SDL_DEFINE_AUDIO_FORMAT",
        "SDL_size_mul_overflow",
        "SDL_size_add_overflow",
        "SDL_AUDIO_S16",
        "SDL_AUDIO_S32",
        "SDL_AUDIO_F32",
        "SDL_VERSIONNUM",
        "SDL_VERSIONNUM_MAJOR",
        "SDL_VERSIONNUM_MINOR",
        "SDL_VERSIONNUM_MICRO",
        "SDL_VERSION",
        "SDL_VERSION_ATLEAST",
        "SDL_BeginThreadFunction",
        "SDL_EndThreadFunction",
        "SDL_CreateThread",
        "SDL_CreateThreadWithProperties",
        "main",
        "SDL_size_mul_check_overflow",
        "SDL_size_add_check_overflow",
        "SDL_INIT_INTERFACE",
        "SDL_BUTTON_MASK",
        "SDL_SCANF_VARARG_FUNC",
        "SDL_SCANF_VARARG_FUNCV",
    };

    private readonly List<CppMacro> _collectedMacros = [];

    private void CollectConstants(CppCompilation compilation)
    {
        foreach (CppMacro cppMacro in compilation.Macros)
        {
            if (string.IsNullOrEmpty(cppMacro.Value)
                || cppMacro.Name.EndsWith("_H_", StringComparison.OrdinalIgnoreCase)
                || cppMacro.Name.Equals("SDL_SCANCODE_TO_KEYCODE", StringComparison.OrdinalIgnoreCase)
                || cppMacro.Name.Equals("SDL_INIT_EVERYTHING", StringComparison.OrdinalIgnoreCase)
                || cppMacro.Name.Equals("VK_DEFINE_HANDLE", StringComparison.OrdinalIgnoreCase)
                || cppMacro.Name.Equals("VK_DEFINE_NON_DISPATCHABLE_HANDLE", StringComparison.OrdinalIgnoreCase)
                || cppMacro.Name.Equals("VK_DEFINE_NON_DISPATCHABLE_HANDLE", StringComparison.OrdinalIgnoreCase)
                )
            {
                continue;
            }

            if (cppMacro.Name.StartsWith("SDL_PLATFORM_"))
                continue;

            if (cppMacro.Name == "SDL_OutOfMemory" ||
                cppMacro.Name == "SDL_Unsupported" ||
                cppMacro.Name == "SDL_InvalidParamError" ||
                cppMacro.Name == "SDL_BUTTON" ||
                cppMacro.Name == "SDL_BUTTON_LMASK" ||
                cppMacro.Name == "SDL_BUTTON_MMASK" ||
                cppMacro.Name == "SDL_BUTTON_RMASK" ||
                cppMacro.Name == "SDL_BUTTON_X1MASK" ||
                cppMacro.Name == "SDL_BUTTON_X2MASK" ||
                cppMacro.Name == "SDL_MS_TO_NS" ||
                cppMacro.Name == "SDL_NS_TO_MS" ||
                cppMacro.Name == "SDL_US_TO_NS" ||
                cppMacro.Name == "SDL_NS_TO_US" ||
                cppMacro.Name == "SDL_TOUCH_MOUSEID" ||
                cppMacro.Name == "SDL_MOUSE_TOUCHID" ||
                cppMacro.Name.StartsWith("SDL_WINDOWPOS_") ||
                cppMacro.Name == "SDL_AUDIO_BITSIZE" ||
                cppMacro.Name == "SDL_AUDIO_BYTESIZE" ||
                cppMacro.Name.StartsWith("SDL_AUDIO_IS")
                || cppMacro.Name == "SDL_AUDIO_FRAMESIZE"
                || cppMacro.Name == "SDL_SYSWM_INFO_SIZE_V1"
                || cppMacro.Name == "SDL_SYSWM_CURRENT_INFO_SIZE"
                || cppMacro.Name.StartsWith("SDL_HAPTIC_")
                || cppMacro.Name == "SDL_PEN_INVALID"
                || cppMacro.Name == "SDL_PEN_MOUSEID"
                || cppMacro.Name == "SDL_PEN_TOUCHID"
                || cppMacro.Name == "SDL_PEN_INFO_UNKNOWN"
                || cppMacro.Name == "SDL_PEN_CAPABILITY"
                || cppMacro.Name == "SDL_PEN_AXIS_CAPABILITY"
                || cppMacro.Name == "SDL_PEN_AXIS_BIDIRECTIONAL_MASKS"
                || cppMacro.Name == "SDL_MUSTLOCK"
                || cppMacro.Name == "SDL_DEFINE_PIXELFOURCC"
                || cppMacro.Name == "SDL_DEFINE_PIXELFORMAT"
                || cppMacro.Name == "SDL_BYTESPERPIXEL"
                || cppMacro.Name == "SDL_BITSPERPIXEL"
                || cppMacro.Name == "SDL_Colour"
                || cppMacro.Name == "SDL_FColour"
                || cppMacro.Name.StartsWith("SDL_PIXEL")
                || cppMacro.Name.StartsWith("SDL_ISPIXELFORMAT")
                || cppMacro.Name == "SDL_DEFINE_COLORSPACE"
                || cppMacro.Name.StartsWith("SDL_COLORSPACE")
                || cppMacro.Name.StartsWith("SDL_ISCOLORSPACE_")
                || cppMacro.Name == "SDL_SECONDS_TO_NS"
                || cppMacro.Name == "SDL_NS_TO_SECONDS"
                || cppMacro.Name.StartsWith("SDL_MIN_")
                || cppMacro.Name.StartsWith("SDL_MAX_")
                || cppMacro.Name.StartsWith("SDL_PRI")
                || cppMacro.Name.StartsWith("SDL_ICONV_")
                || cppMacro.Name.StartsWith("SDL_iconv_")
                )
            {
                continue;
            }

            if (cppMacro.Name.StartsWith("SDL_PEN_") && cppMacro.Name.EndsWith("_MASK"))
            {
                continue;
            }

            if (s_ignoreConstants.Contains(cppMacro.Name))
                continue;

            if (cppMacro.Value.StartsWith("SDL_THREAD_ANNOTATION_ATTRIBUTE__"))
                continue;

            _collectedMacros.Add(cppMacro);
        }
    }

    private void GenerateConstants()
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Constants.cs"), false, _options.Namespace, []);
        using (writer.PushBlock($"{visibility} static partial class {_options.ClassName}"))
        {
            foreach (CppMacro cppMacro in _collectedMacros)
            {
                if (cppMacro.Name.StartsWith("SDL_PROP_GAMEPAD_CAP_") && cppMacro.Name.EndsWith("_BOOLEAN"))
                {
                    writer.WriteLine($"public static ReadOnlySpan<byte> {cppMacro.Name} => {cppMacro.Value};");
                    continue;
                }

                //string csName = GetPrettyEnumName(cppMacro.Name, "VK_");

                string modifier = "static";
                string csDataType = "int";
                string macroValue = NormalizeEnumValue(cppMacro.Value);
                if (macroValue.EndsWith("F", StringComparison.OrdinalIgnoreCase))
                {
                    modifier = "const";
                    csDataType = "float";
                }
                else if (macroValue.EndsWith("LL", StringComparison.OrdinalIgnoreCase))
                {
                    modifier = "const";
                    csDataType = "long";
                }
                else if (macroValue.EndsWith("UL", StringComparison.OrdinalIgnoreCase))
                {
                    modifier = "const";
                    csDataType = "ulong";
                }
                else if (macroValue.EndsWith("U", StringComparison.OrdinalIgnoreCase))
                {
                    modifier = "const";
                    csDataType = "uint";
                }
                else if (uint.TryParse(macroValue, out _) || macroValue.StartsWith("0x"))
                {
                    modifier = "const";
                    csDataType = "uint";
                }
                else if (macroValue.Contains("<<"))
                {
                    modifier = "const";
                    csDataType = "int";
                }
                else if (macroValue.StartsWith("\"", StringComparison.OrdinalIgnoreCase)
                    && macroValue.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
                {
                    csDataType = "ReadOnlySpan<byte>";
                }

                if (cppMacro.Name == "SDL_OutOfMemory" || cppMacro.Name == "SDL_Unsupported")
                {
                    modifier = "static readonly";
                    csDataType = "int";
                }
                if (cppMacro.Name == "SDL_JOYSTICK_AXIS_MAX"
                    || cppMacro.Name == "SDL_JOYSTICK_AXIS_MIN"
                    || cppMacro.Name == "SDL_MIX_MAXVOLUME"
                    || cppMacro.Name == "SDL_TEXTEDITINGEVENT_TEXT_SIZE"
                    || cppMacro.Name == "SDL_TEXTINPUTEVENT_TEXT_SIZE")
                {
                    modifier = "const";
                    csDataType = "int";
                }
                if (cppMacro.Name == "SDL_IPHONE_MAX_GFORCE")
                {
                    modifier = "const";
                    csDataType = "float";
                    macroValue = "5.0f";
                }
                if (cppMacro.Name == "SDL_HAT_RIGHTUP"
                    || cppMacro.Name == "SDL_HAT_RIGHTDOWN"
                    || cppMacro.Name == "SDL_HAT_LEFTUP"
                    || cppMacro.Name == "SDL_HAT_LEFTDOWN"
                    || cppMacro.Name.StartsWith("SDL_PEN_")
                    || cppMacro.Name.StartsWith("SDL_GPU_"))
                {
                    modifier = "const";
                    csDataType = "uint";
                }
                if (cppMacro.Name == "SDL_NS_PER_SECOND")
                {
                    modifier = "const";
                    csDataType = "ulong";
                }
                if (cppMacro.Name == "SDL_PI_D")
                {
                    modifier = "const";
                    csDataType = "double";
                }

                if (cppMacro.Name == "SDL_AUDIO_MASK_BITSIZE" ||
                    cppMacro.Name == "SDL_AUDIO_MASK_FLOAT" ||
                    cppMacro.Name == "SDL_AUDIO_MASK_BIG_ENDIAN" ||
                    cppMacro.Name == "SDL_AUDIO_MASK_SIGNED" ||
                    cppMacro.Name == "SDL_AUDIO_S16" ||
                    cppMacro.Name == "SDL_AUDIO_S32"
                    || cppMacro.Name == "SDL_AUDIO_F32"
                    || cppMacro.Name.StartsWith("SDL_HAPTIC_"))
                {
                    modifier = "const";
                    csDataType = "uint";
                }

                if (cppMacro.Name.StartsWith("SDL_AUDIO_S")
                    || cppMacro.Name.StartsWith("SDL_AUDIO_F"))
                {
                    modifier = "const";
                    csDataType = "ushort";
                }

                if (cppMacro.Name == "SDL_AUDIO_DEVICE_DEFAULT_OUTPUT"
                    || cppMacro.Name == "SDL_AUDIO_DEVICE_DEFAULT_CAPTURE")
                {
                    modifier = "static readonly";
                    csDataType = "SDL_AudioDeviceID";
                }

                if (cppMacro.Name == "SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK"
                    || cppMacro.Name == "SDL_AUDIO_DEVICE_DEFAULT_RECORDING")
                {
                    csDataType = "SDL_AudioDeviceID";
                }

                if (cppMacro.Name.StartsWith("SDLK_"))
                {
                    modifier = "const";
                    csDataType = "uint";

                    if (macroValue.StartsWith("SDL_SCANCODE_TO_KEYCODE"))
                    {
                        string enumValueName = GetEnumItemName("SDL_Scancode", cppMacro.Tokens[2].Text, "SDL_SCANCODE");
                        macroValue = $"((int)SDL_Scancode.{enumValueName} | SDLK_SCANCODE_MASK)";
                    }
                }
                else if (cppMacro.Name.StartsWith("SDL_KMOD_"))
                {
                    modifier = "const";
                    csDataType = "ushort";
                    if (macroValue.EndsWith("u"))
                        macroValue = macroValue.Substring(0, macroValue.Length - 1);
                }

                if (cppMacro.Name.StartsWith("SDL_WINDOW_") && cppMacro.Tokens.Count > 2)
                {
                    modifier = "const";
                    csDataType = "ulong";
                    macroValue = cppMacro.Tokens[2].Text;
                }

                if (cppMacro.Name == "SDL_FALSE" || cppMacro.Name == "SDL_TRUE")
                {
                    csDataType = "SDL_bool";
                    macroValue = $"(SDL_bool)({macroValue})";
                }

                if (cppMacro.Name == "SDL_MAJOR_VERSION"
                    || cppMacro.Name == "SDL_MINOR_VERSION"
                    || cppMacro.Name == "SDL_MICRO_VERSION"
                    || cppMacro.Name.StartsWith("SDL_PEN_FLAG_"))
                {
                    modifier = "const";
                    csDataType = "int";
                }
                else if (cppMacro.Name == "SDL_RWLOCK_TIMEDOUT"
                    || cppMacro.Name == "SDL_GLOB_CASEINSENSITIVE")
                {
                    modifier = "const";
                    csDataType = "uint";
                }
                else if (cppMacro.Name.StartsWith("SDL_GPU_COLORCOMPONENT_"))
                {
                    modifier = "const";
                    csDataType = "byte";
                    macroValue = $"(byte)({macroValue})";
                }

                //writer.WriteLine($"/// <unmanaged>{cppMacro.Name}</unmanaged>");
                if (modifier == "static" && csDataType == "ReadOnlySpan<byte>")
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} => {macroValue}u8;");
                }
                else
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} = {macroValue};");
                }
            }

            writer.WriteLine();

            foreach (string enumConstant in _enumConstants)
            {
                writer.WriteLine($"public const {enumConstant};");
            }
        }
    }
}
