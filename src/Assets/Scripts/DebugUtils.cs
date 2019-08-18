//#define DEBUG

using System;
using System.Diagnostics;

public static class DebugUtils
{
	[Conditional("DEBUG")]
	internal static void Assert(bool condition)
	{
		if (!condition)
			throw new Exception();
	}
}