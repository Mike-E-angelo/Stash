using System.Runtime.CompilerServices;

namespace Scrutor.Decorate.Model;

public delegate ConfiguredValueTaskAwaitable Await<in T>(T parameter);