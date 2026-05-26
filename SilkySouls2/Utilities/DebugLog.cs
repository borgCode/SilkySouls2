using System;
using System.Diagnostics;

namespace SilkySouls2.Utilities;

public static class DebugLog
{
    [Conditional("DEBUG")]
    public static void Log(string msg) => Console.WriteLine(msg);
}
