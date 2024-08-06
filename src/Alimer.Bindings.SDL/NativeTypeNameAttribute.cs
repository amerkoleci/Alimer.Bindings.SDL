// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace SDL3;

/// <summary>Defines the type of a member as it was used in the native signature.</summary>
/// <remarks>Initializes a new instance of the <see cref="NativeTypeNameAttribute" /> class.</remarks>
/// <param name="name">The name of the type that was used in the native signature.</param>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
[Conditional("DEBUG")]
internal sealed partial class NativeTypeNameAttribute(string name) : Attribute
{
    /// <summary>Gets the name of the type that was used in the native signature.</summary>
    public string Name { get; } = name;
}
