﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE1006:Naming Styles",
    Justification = "Patch name is derived from the method it patches, which may be non-conforming because of Teddycode(tm)",
    Scope = "namespaceanddescendants", 
    Target = "~N:Grindless.HarmonyPatches")]

[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", 
    Justification = "Patches are made private on purpose and used via attributes only.", 
    Scope = "namespaceanddescendants", 
    Target = "~N:Grindless.HarmonyPatches")]
