using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace XwaManager;

[SecurityCritical, SuppressUnmanagedCodeSecurity]
internal static class NativeMethods
{
    [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
    public static extern long StrFormatByteSize(
        long fileSize,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer,
        int bufferSize);
}
