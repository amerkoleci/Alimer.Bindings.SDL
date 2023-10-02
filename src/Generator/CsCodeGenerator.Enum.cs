// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static readonly char[] separator = ['_'];

    private static readonly Dictionary<string, string> s_knownEnumPrefixes = new()
    {
        { "SDL_Scancode", "SDL_SCANCODE" },
        { "SDL_KeyCode", "SDLK" },
        { "SDL_Keymod", "SDL_KMOD" },
        { "SDL_GamepadType", "SDL_GAMEPAD_TYPE" },
        { "SDL_GamepadButton", "SDL_GAMEPAD_BUTTON" },
        { "SDL_GamepadAxis", "SDL_GAMEPAD_AXIS" },
        { "SDL_GamepadBindingType", "SDL_GAMEPAD_BINDTYPE" },
        { "SDL_InitFlags", "SDL_INIT" },
    };

    private static readonly Dictionary<string, string> s_knownEnumValueNames = new()
    {
        { "SDL_NUM_SCANCODES", "NumScancodes" },
    };

    private static readonly HashSet<string> s_ignoredParts = new(StringComparer.OrdinalIgnoreCase)
    {
    };

    private static readonly HashSet<string> s_preserveCaps = new(StringComparer.OrdinalIgnoreCase)
    {
        "sdl",
    };

    private static readonly Dictionary<string, string> s_partRenames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "lctrl", "LeftControl" },
        { "Lshift", "LeftShirt" },
        { "Lalt", "LeftAlt" },
        { "Lgui", "LeftGui" },
        { "Rctrl", "RightControl" },
        { "Rshift", "RightShirt" },
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
    };

    public static void CollectEnums(CppCompilation compilation)
    {
        foreach (CppEnum cppEnum in compilation.Enums)
        {
            if (string.IsNullOrEmpty(cppEnum.Name))
            {
                continue;
            }

            s_collectedEnums.Add(cppEnum);
        }
    }

    public static void GenerateEnums(CppCompilation compilation)
    {
        string visibility = s_options.PublicVisiblity ? "public" : "internal";
        using CodeWriter writer = new(Path.Combine(s_options.OutputPath, "Enums.cs"), false, s_options.Namespace, ["System"]);
        Dictionary<string, string> createdEnums = [];

        foreach (CppEnum cppEnum in s_collectedEnums)
        {
            bool isBitmask =
                cppEnum.Name == "SDL_Keymod" ||
                cppEnum.Name == "SDL_InitFlags" ||
                cppEnum.Name.EndsWith("Flags");

            if (isBitmask)
            {
                writer.WriteLine("[Flags]");
            }

            string csName = GetCsCleanName(cppEnum.Name);
            string enumNamePrefix = GetEnumNamePrefix(cppEnum.Name);

            createdEnums.Add(csName, cppEnum.Name);

            bool noneAdded = false;
            using (writer.PushBlock($"{visibility} enum {csName}"))
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

                    if (enumItemName != "Count" && s_options.EnumWriteUnmanagedTag)
                    {
                        writer.WriteLine($"/// <unmanaged>{enumItem.Name}</unmanaged>");
                    }

                    if (enumItem.ValueExpression is CppRawExpression rawExpression)
                    {
                        //string enumValueName = GetEnumItemName(rawExpression.Text);
                        writer.WriteLine($"{enumItemName} = {rawExpression.Text},");
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
                }

                if (cppEnum.Name == "SDL_InitFlags")
                {
                    if (s_options.EnumWriteUnmanagedTag)
                    {
                        writer.WriteLine($"/// <unmanaged>SDL_INIT_EVERYTHING</unmanaged>");
                    }

                    writer.WriteLine($"Everything = Timer | Audio | Video | Events | Joystick | Haptic | Gamepad | Sensor,");
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
        string enumItemName = GetPrettyEnumName(cppEnumItemName, enumNamePrefix);
        if (char.IsNumber(enumItemName[0]))
        {
            if (enumName == "SDL_KeyCode")
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

        return value.Replace("ULL", "UL");
    }
}
