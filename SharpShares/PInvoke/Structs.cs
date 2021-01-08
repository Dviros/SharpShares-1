using System.Runtime.InteropServices;

namespace SharpShares.PInvoke
{
    public class Structs
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHARE_INFO_1
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string NetName;
            public Enums.SHARE_TYPE ShareType;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Remark;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHARE_INFO_2
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_netname;
            public uint shi2_type;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_remark;
            public uint shi2_permissions;
            public uint shi2_max_uses;
            public uint shi2_current_uses;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_path;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_passwd;
        }


        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public Enums.ResourceScope Scope;
            public Enums.ResourceType ResourceType;
            public Enums.ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }
    }
}
