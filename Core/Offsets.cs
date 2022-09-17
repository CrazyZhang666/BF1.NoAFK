namespace BF1.NoAFK.Core;

public static class Offsets
{
    public const long OFFSET_CLIENTGAMECONTEXT = 0x1437F7758;
    public const long OFFSET_GAMERENDERER = 0x1439E6D08;
    public const long OFFSET_OBFUSCATIONMGR = 0x14351D058;

    ////////////////////////////////////////////////////////////////////

    public const int ServerName_Offset = 0x3A1F3F8;

    public static int[] ServerName = new int[] { 0x30, 0x00 };
}
