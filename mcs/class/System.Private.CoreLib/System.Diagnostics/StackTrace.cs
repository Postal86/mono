using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	// Need our own stackframe class since the shared version has its own fields
	[StructLayout (LayoutKind.Sequential)]
	class MonoStackFrame
	{
		#region Keep in sync with object-internals.h
		internal int ilOffset;
		internal int nativeOffset;
		// Unused
		internal long methodAddress;
		// Unused
		internal uint methodIndex;
		internal MethodBase methodBase;
		internal string fileName;
		internal int lineNumber;
		internal int columnNumber;
		// Unused
		internal string internalMethodName;
		#endregion
	}

	partial class StackTrace
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		extern static MonoStackFrame[] get_trace (Exception e, int skipFrames, bool needFileInfo);

		[MethodImplAttribute (MethodImplOptions.NoInlining)]
		void InitializeForCurrentThread (int skipFrames, bool needFileInfo)
		{
			skipFrames += 2; // Current method + parent ctor

			StackFrame sf;
			var frames = new List<StackFrame> ();
			while ((sf = new StackFrame (skipFrames, needFileInfo)) != null && sf.GetMethod () != null) {
				frames.Add (sf);
				skipFrames++;
			}

			_stackFrames = frames.ToArray ();
			_numOfFrames = _stackFrames.Length;
		}
		
		void InitializeForException (Exception e, int skipFrames, bool needFileInfo)
		{
			var frames = get_trace (e, skipFrames, needFileInfo);
			_numOfFrames = frames.Length;
			_stackFrames = new StackFrame [_numOfFrames];
			for (int i = 0; i < _numOfFrames; ++i) {
				_stackFrames [i] = new StackFrame (frames [i]);
			}
		}
	}
}