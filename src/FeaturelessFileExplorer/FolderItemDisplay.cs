namespace FeaturelessFileExplorer;

/// <summary>
/// Represents a file or folder to be displayed in the list view.
/// </summary>
public sealed class FolderItemDisplay
{
    /// <summary>
    /// Creates a new instance of <see cref="FolderItemDisplay"/> for a folder.
    /// </summary>
    /// <param name="name">Name of the file or folder.  This is what's displayed in the ListView.</param>
    /// <param name="fullPath">Absolute path to the file or folder.</param>
    /// <param name="size">Size of the file in kilobytes.</param>
    /// <param name="created"><see cref="DateTime"/> the item was created.</param>
    /// <param name="modified"><see cref="DateTime"/> the item was last modified.</param>
    /// <param name="displayType">Type of the file or folder. Valid values are "parent-folder", "file", and "folder".</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public FolderItemDisplay(string name, string fullPath, long? size, DateTime? created, DateTime? modified, string displayType)
    {
        Name = name;
        FullPath = fullPath;
        Size = size;
        Created = created;
        Modified = modified;
        var validDisplayTypes = new[] { Constants.DISPLAY_TYPE_FILE, Constants.DISPLAY_TYPE_FOLDER, Constants.DISPLAY_TYPE_PARENT_FOLDER };
        if (!validDisplayTypes.Contains(displayType)) throw new ArgumentOutOfRangeException($@"Invalid display type: {displayType}", nameof(displayType));
        DisplayType = displayType;
    }

    /// <summary>
    /// Name of the file or folder.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Full path of the file or folder.
    /// </summary>
    public string FullPath { get; }

    /// <summary>
    /// Size of the item, in bytes.
    /// </summary>
    public long? Size { get; }

    /// <summary>
    /// Date and time the file or folder was created.
    /// </summary>
    public DateTime? Created { get; }

    /// <summary>
    /// Date and time the file or folder was last modified.
    /// </summary>
    public DateTime? Modified { get; }

    /// <summary>
    /// Type of the file or folder. Valid values are "parent-folder", "file", and "folder".
    /// </summary>
    public string DisplayType { get; }

}