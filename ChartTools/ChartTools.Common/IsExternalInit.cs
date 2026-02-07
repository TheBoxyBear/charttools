#if NETSTANDARD
using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[EditorBrowsable(EditorBrowsableState.Never)]
public record class IsExternalInit;
#endif
