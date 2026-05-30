using System;
using System.Runtime.InteropServices;

namespace ns8;

[Serializable]
[Flags]
[ComVisible(true)]
public enum FileAccess
{
	Read = 1,
	Write = 2,
	ReadWrite = 3
}
