using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace FeaturelessFileExplorer;

public partial class MainForm : Form
{
    private List<FolderItemDisplay> _listViewItems = new();

    public MainForm()
    {
        InitializeComponent();
        Load += MainForm_Load;
    }

    private string SortDirection { get; set; } = "asc";

    private void MainForm_Load(object? sender, EventArgs e)
    {
        // set up the list view columns
        lvFilesAndFolders.Columns.Add("Name", 400);
        lvFilesAndFolders.Columns.Add("Size", 200);
        lvFilesAndFolders.Columns.Add("Created", 200);
        lvFilesAndFolders.Columns.Add("Modified", 200);

        // preload the image list with the icons embedded in assembly, these are 
        // displayed in the list view
        foreach (var displayType in new[] { Constants.DISPLAY_TYPE_FILE, Constants.DISPLAY_TYPE_FOLDER, Constants.DISPLAY_TYPE_PARENT_FOLDER })
        {
            imageList.Images.Add(displayType, new Icon(ReadEmbeddedIcon($"{displayType}.ico")));
        }
    }

    /// <summary>
    /// Reads the icons out of the assemmbly and returns a stream
    /// </summary>
    /// <param name="fileName">Name of the embedded file to retrieve.</param>
    /// <returns>Stream containing the embedded file.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Stream ReadEmbeddedIcon(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = $"FeaturelessFileExplorer.icons.{fileName}";
        return assembly.GetManifestResourceStream(resourcePath)
            ?? throw new InvalidOperationException($"Could not find embedded resource: {resourcePath}");
    }

    /// <summary>
    /// Handles loading the files and folders for the given folder path and updating the UI.
    /// </summary>
    /// <param name="folderPath">Path to the folder being listed.</param>
    /// <returns><see cref="Task"/></returns>
    private async Task LoadFilesAndFolders(string folderPath)
    {
        try
        {
            this.Cursor = Cursors.WaitCursor;
            folderPath = folderPath.Trim();
            var items = new List<FolderItemDisplay>();
            await Task.Run(() => items = GetFilesAndFolders(folderPath));
            _listViewItems = items;
            lvFilesAndFolders.BeginUpdate();
            lvFilesAndFolders.VirtualListSize = _listViewItems.Count;
            lvFilesAndFolders.Refresh();
            lvFilesAndFolders.EndUpdate();
            txtFolder.Text = folderPath;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    /// <summary>
    /// Returns the files and folders for the given folder path.
    /// </summary>
    /// <param name="folderPath">Path of the folder to retrieve.</param>
    /// <returns><see cref="List{T}"/> of <see cref="FolderItemDisplay"/> items.</returns>
    private List<FolderItemDisplay> GetFilesAndFolders(string folderPath)
    {
        folderPath = $@"{folderPath}";
        var filesAndFolders = new List<FolderItemDisplay>();
        var folderInfo = new DirectoryInfo(folderPath);

        // if there's a parent folder, add it to the list
        if (folderInfo.Parent is not null)
        {
            filesAndFolders.Add(new("(Parent)", folderPath, null, null, null, Constants.DISPLAY_TYPE_PARENT_FOLDER));
        };

        // match behavior of Windows Explorer, directories are listed first by default
        foreach (var d in Directory.EnumerateDirectories(folderPath))
        {
            var di = new DirectoryInfo(d);
            filesAndFolders.Add(
                new(di.Name,
                    d,
                    null,
                    di.CreationTime,
                    di.LastAccessTime,
                    Constants.DISPLAY_TYPE_FOLDER));
        }

        foreach (var f in Directory.EnumerateFiles(folderPath))
        {
            var fi = new FileInfo(f);
            filesAndFolders.Add(
                new(fi.Name, 
                    f,
                    fi.Length,
                    fi.CreationTime,
                    fi.LastAccessTime,
                    Constants.DISPLAY_TYPE_FILE));
        }

        return filesAndFolders;
    }

    /// <summary>
    /// When the user presses Enter in the folder path text box, load the files and folders for that path.
    /// </summary>
    /// <param name="sender"><see cref="TextBox"/></param>
    /// <param name="e"><see cref="KeyEventArgs"/></param>
    private async void txtFolder_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;

        // Turn off the 'Ding' sound when Enter pressed in TextBox
        // https://stackoverflow.com/a/31347256/1594171
        e.SuppressKeyPress = true;

        await LoadFilesAndFolders(txtFolder.Text);
    }

    /// <summary>
    /// ListView uses this event to populate the items in the list view.
    /// </summary>
    /// <param name="sender"><see cref="ListView"/></param>
    /// <param name="e"><see cref="RetrieveVirtualItemEventArgs"/></param>
    private void lvFilesAndFolders_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
    {
        var fid = _listViewItems[e.ItemIndex];
        e.Item = new ListViewItem(new[]
        {
            fid.Name,
            fid.Size.HasValue ? fid.Size.ToString() : string.Empty,
            fid.Created.HasValue ? fid.Created.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
            fid.Modified.HasValue ? fid.Modified.Value.ToString(CultureInfo.InvariantCulture) : string.Empty
        })
        {
            ImageIndex = imageList.Images.IndexOfKey(fid.DisplayType),
            Tag = fid
        };
    }

    /// <summary>
    /// Handles the double click event on the list view.
    /// </summary>
    /// <param name="sender"><see cref="ListView"/></param>
    /// <param name="e"><see cref="EventArgs"/></param>
    private async void lvFilesAndFolders_DoubleClick(object sender, EventArgs e)
    {
        if (lvFilesAndFolders.SelectedIndices.Count == 0) return;

        var fid = _listViewItems[lvFilesAndFolders.SelectedIndices[0]];
        switch (fid.DisplayType)
        {
            // Try and open the file in the default application
            case Constants.DISPLAY_TYPE_FILE:
                var processStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = fid.FullPath
                };
                Process.Start(processStartInfo);
                break;

            // Load the files and folders for the selected folder
            case Constants.DISPLAY_TYPE_FOLDER:
                await LoadFilesAndFolders(fid.FullPath);
                break;

            // Load the files and folders for the parent folder
            case Constants.DISPLAY_TYPE_PARENT_FOLDER:
                var parentFullName = new DirectoryInfo(fid.FullPath).Parent?.FullName;
                if (parentFullName != null) await LoadFilesAndFolders(parentFullName);
                break;
        }
    }

    /// <summary>
    /// Handles sorting the list view items.
    /// </summary>
    /// <param name="listViewItems">Items to be sorted.</param>
    /// <param name="sortByColumnIndex">Index of the column to sort by.</param>
    /// <param name="direction">Direction we want to sort by, asc or desc.</param>
    /// <returns>Sorted list of items.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static List<FolderItemDisplay> SortItems(List<FolderItemDisplay> listViewItems, int sortByColumnIndex, string direction)
    {
        IEnumerable<FolderItemDisplay> Sort(IEnumerable<FolderItemDisplay> items) =>
            sortByColumnIndex switch
            {
                0 => direction == "asc" ? items.OrderBy(x => x.Name) : items.OrderByDescending(x => x.Name),
                1 => direction == "asc" ? items.OrderBy(x => x.Size) : items.OrderByDescending(x => x.Size),
                2 => direction == "asc" ? items.OrderBy(x => x.Created) : items.OrderByDescending(x => x.Created),
                3 => direction == "asc" ? items.OrderBy(x => x.Modified) : items.OrderByDescending(x => x.Modified),
                _ => throw new ArgumentOutOfRangeException(nameof(sortByColumnIndex))
            };

        var folders = Sort(listViewItems.Where(x => x.DisplayType == Constants.DISPLAY_TYPE_FOLDER));
        var files = Sort(listViewItems.Where(x => x.DisplayType == Constants.DISPLAY_TYPE_FILE));

        var sortedItems = new List<FolderItemDisplay>();

        // Parent folder navigation entry should always be first if it exists
        var parentFolderItem = listViewItems.FirstOrDefault(x => x.DisplayType == Constants.DISPLAY_TYPE_PARENT_FOLDER);
        if (parentFolderItem != null) sortedItems.Add(parentFolderItem);

        sortedItems.AddRange(direction == "asc" ? folders.Concat(files) : files.Concat(folders));

        return sortedItems;
    }

    /// <summary>
    /// Handles the ColumnClick event on the list view.  This is where we sort the list view.
    /// </summary>
    /// <param name="sender"><see cref="ListView"/></param>
    /// <param name="e"><see cref="ColumnClickEventArgs"/></param>
    private void lvFilesAndFolders_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (!_listViewItems.Any()) return;

        var direction = SortDirection == "asc" ? "desc" : "asc";
        SortDirection = direction;
        _listViewItems = SortItems(_listViewItems, e.Column, direction);

        lvFilesAndFolders.BeginUpdate();
        // Add the up/down arrows to the column header being sorted
        var columns = new[] { "Name", "Size", "Created", "Modified" };
        var upArrow = "▲   ";
        var downArrow = "▼   ";
        for (var i = 0; i < lvFilesAndFolders.Columns.Count; i++)
        {
            if (i == e.Column)
            {
                lvFilesAndFolders.Columns[i].Text = SortDirection == "asc" ? upArrow + columns[i] : downArrow + columns[i];
            }
            else
            {
                lvFilesAndFolders.Columns[i].Text = columns[i];
            }
        }
        lvFilesAndFolders.VirtualListSize = _listViewItems.Count;
        lvFilesAndFolders.Refresh();
        lvFilesAndFolders.EndUpdate();

    }

    private void lvFilesAndFolders_MouseDown(object sender, MouseEventArgs e)
    {
        // we only care about right clicks
        if (e.Button != MouseButtons.Right) return;

        // if we didn't click on an item, return
        if (lvFilesAndFolders.GetItemAt(e.X, e.Y) is not { } lvi) return;

        // if we didn't click on a file, return
        var fid = (FolderItemDisplay)lvi.Tag;
        if (fid.DisplayType != Constants.DISPLAY_TYPE_FILE) return;

        // create the context menu
        var cms = new ContextMenuStrip();
        cms.Items.Add("Open", null, OpenItemEventHandler(fid));
        cms.Items.Add("Copy Local", null, CopyLocalEventHandler(fid));
        cms.Items.Add("Copy Path", null, CopyPathEventHandler(fid));

        // show it in the right spot on the list view
        cms.Show(lvFilesAndFolders, e.Location);
    }

    /// <summary>
    /// Opens a file using the default application.
    /// </summary>
    /// <param name="fid"><see cref="FolderItemDisplay"/> to open.</param>
    /// <returns><see cref="EventHandler"/></returns>
    private EventHandler OpenItemEventHandler(FolderItemDisplay fid) =>
        (_, _) =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fid.FullPath
            };
            Process.Start(processStartInfo);
        };

    /// <summary>
    /// Copies the full path of a file to the clipboard.
    /// </summary>
    /// <param name="fid"><see cref="FolderItemDisplay"/> item to copy path from.</param>
    /// <returns><see cref="EventHandler"/></returns>
    private EventHandler CopyPathEventHandler(FolderItemDisplay fid) =>
        (_, _) =>
        {
            Clipboard.SetText(fid.FullPath);
        };

    /// <summary>
    /// Copies a file to the downloads folder.
    /// </summary>
    /// <param name="fid"><see cref="FolderItemDisplay"/> to copy.</param>
    /// <returns><see cref="EventHandler"/></returns>
    private EventHandler CopyLocalEventHandler(FolderItemDisplay fid)
    {
        async void LocalEventHandler(object sender, EventArgs args)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var sourceFile = fid.FullPath;
                var fileInfo = new FileInfo(fid.FullPath);
                var downloadsDir = Path.Combine(Environment.CurrentDirectory, "downloads");
                if (!Directory.Exists(downloadsDir))
                {
                    Directory.CreateDirectory(downloadsDir);
                }

                var destinationFile = Path.Combine(downloadsDir, fileInfo.Name);
                if (File.Exists(destinationFile))
                {
                    var result = MessageBox.Show($@"File {fileInfo.Name} already exists in Downloads folder. Overwrite?", @"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No) return;
                }

                await using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                await using var destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                await sourceStream.CopyToAsync(destinationStream);
                MessageBox.Show($@"File copied to {destinationFile}", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        return LocalEventHandler!;
    }
}