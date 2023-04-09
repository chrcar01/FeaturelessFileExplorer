namespace FeaturelessFileExplorer;

/// <summary>
/// Represents a file or folder to be displayed in the list view.
/// </summary>
public sealed class FolderItemDisplay
{
    /// <summary>
    /// Creates a new instance of <see cref="FolderItemDisplay"/> for a folder.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fullPath"></param>
    /// <param name="size"></param>
    /// <param name="created"></param>
    /// <param name="modified"></param>
    /// <param name="displayType"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public FolderItemDisplay(string name, string fullPath, long? size, DateTime? created, DateTime? modified, string displayType)
    {
        Name = name;
        FullPath = fullPath;
        Size = size;
        Created = created;
        Modified = modified;
        var validDisplayTypes = new[] { "parent-folder", "file", "folder" };
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
    /// Type of the file or folder. Valid values are "arrow-up", "file", and "folder".
    /// </summary>
    public string DisplayType { get; }

}