// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "GHL is an acronym", Scope = "type", Target = "~T:ChartTools.GHLChord")]

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "Files should be able to be better organized without bloating namespaces", Scope = "namespace", Target = "~N:ChartTools")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "Files should be able to be better organized without bloating namespaces", Scope = "namespace", Target = "~N:ChartTools.Extensions.Collections")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "Files should be able to be better organized without bloating namespaces", Scope = "namespace", Target = "~N:ChartTools.IO")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "Files should be able to be better organized without bloating namespaces", Scope = "namespace", Target = "~N:ChartTools.IO.Parsing")]
[assembly: SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "Music theory involves collisions between sharps and flats", Scope = "type", Target = "~T:ChartTools.Lyrics.VocalsPitchValue")]
