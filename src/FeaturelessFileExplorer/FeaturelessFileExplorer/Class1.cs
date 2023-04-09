using System.Runtime.InteropServices;

namespace FeaturelessFileExplorer;

public static class KnownFolders
{
    private enum KnownFolder
    {
        Contacts,
        Downloads,
        Favorites
    }

    public static string Contacts => SHGetKnownFolderPath(_guids[KnownFolder.Contacts], 0);
    public static string Downloads => SHGetKnownFolderPath(_guids[KnownFolder.Downloads], 0);
    public static string Favorites => SHGetKnownFolderPath(_guids[KnownFolder.Favorites], 0);

    private static readonly Dictionary<KnownFolder, Guid> _guids = new()
    {
        [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
        [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
        [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD")
    };

    [DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern string SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
        nint hToken = 0);
}