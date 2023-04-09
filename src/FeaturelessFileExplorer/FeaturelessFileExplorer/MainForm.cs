using System.Diagnostics;
using System.Globalization;

namespace FeaturelessFileExplorer;

public partial class MainForm : Form
{
    private List<FolderItemDisplay> _items = new();
    
    public MainForm()
    {
        InitializeComponent();
        Load += MainForm_Load;
    }

    private string SortDirection { get; set; } = "asc";

    private void MainForm_Load(object? sender, EventArgs e)
    {
        lvFilesAndFolders.Columns.Add("Name", 400);
        lvFilesAndFolders.Columns.Add("Size", 200);
        lvFilesAndFolders.Columns.Add("Created", 200);
        lvFilesAndFolders.Columns.Add("Modified", 200);
        imageList.Images.Add("arrow-up", new Icon(Path.Combine(Environment.CurrentDirectory, "arrow-up.ico")));
        imageList.Images.Add("folder", new Icon(Path.Combine(Environment.CurrentDirectory, "folder.ico")));
        imageList.Images.Add("file", new Icon(Path.Combine(Environment.CurrentDirectory, "file.ico")));
    }

    private async Task LoadFilesAndFolders(string folderPath)
    {
        try
        {
            this.Cursor = Cursors.WaitCursor;

            var items = new List<FolderItemDisplay>();
            await Task.Run(() => items = GetFilesAndFolders(folderPath));
            _items = items;
            lvFilesAndFolders.BeginUpdate();
            lvFilesAndFolders.VirtualListSize = _items.Count;
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

    private FolderItemDisplay ArrowUpFolderItemDisplay(string folderPath) => 
        new("(Parent)", folderPath, null, null, null, "arrow-up");

    private List<FolderItemDisplay> GetFilesAndFolders(string folderPath)
    {
        var filesAndFolders = new List<FolderItemDisplay>();
        var folderInfo = new DirectoryInfo(folderPath);
        
        if (folderInfo.Parent is not null)
        {
            filesAndFolders.Add(ArrowUpFolderItemDisplay(folderPath));
        };

        filesAndFolders.AddRange(Directory
            .EnumerateDirectories(folderPath)
            .Select(d => new DirectoryInfo(d))
            .Select(di => new FolderItemDisplay(
                di.Name,
                di.FullName,
                null,
                di.CreationTime,
                di.LastWriteTime,
                "folder")));

        filesAndFolders.AddRange(Directory
            .EnumerateFiles(folderPath)
            .Select(f => new FileInfo(f))
            .Select(fi => new FolderItemDisplay(
                fi.Name,
                fi.FullName,
                fi.Length,
                fi.CreationTime,
                fi.LastWriteTime,
                "file")));

        return filesAndFolders;
    }

    private async void txtFolder_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;

        // Turn off the 'Ding' sound when Enter pressed in TextBox
        // https://stackoverflow.com/a/31347256/1594171
        e.SuppressKeyPress = true;

        await LoadFilesAndFolders(txtFolder.Text);
    }

    private void lvFilesAndFolders_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
    {
        var fid = _items[e.ItemIndex];
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

    private async void lvFilesAndFolders_DoubleClick(object sender, EventArgs e)
    {
        if (lvFilesAndFolders.SelectedIndices.Count == 0) return;

        var fid = _items[lvFilesAndFolders.SelectedIndices[0]];
        switch (fid.DisplayType)
        {
            case "file":
                var processStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = fid.FullPath
                };
                Process.Start(processStartInfo);
                break;
            case "folder":
                await LoadFilesAndFolders(fid.FullPath);
                break;
            case "arrow-up":
                var parentFullName = new DirectoryInfo(fid.FullPath).Parent?.FullName;
                if (parentFullName != null) await LoadFilesAndFolders(parentFullName);
                break;
        }
    }

    private void lvFilesAndFolders_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        var folders = _items.Where(x => x.DisplayType == "folder");
        var files = _items.Where(x => x.DisplayType == "file");
        var direction = SortDirection == "asc" ? "desc" : "asc";
        SortDirection = direction;
        switch (e.Column)
        {
            case 0:
                folders = SortDirection == "asc" ? folders.OrderBy(x => x.Name) : folders.OrderByDescending(x => x.Name);
                files = SortDirection == "asc" ? files.OrderBy(x => x.Name) : files.OrderByDescending(x => x.Name);
                break;
            case 1:
                folders = SortDirection == "asc" ? folders.OrderBy(x => x.Size) : folders.OrderByDescending(x => x.Size);
                files = SortDirection == "asc" ? files.OrderBy(x => x.Size) : files.OrderByDescending(x => x.Size);
                break;
            case 2:
                folders = SortDirection == "asc" ? folders.OrderBy(x => x.Created) : folders.OrderByDescending(x => x.Created);
                files = SortDirection == "asc" ? files.OrderBy(x => x.Created) : files.OrderByDescending(x => x.Created);
                break;
            case 3:
                folders = SortDirection == "asc" ? folders.OrderBy(x => x.Modified) : folders.OrderByDescending(x => x.Modified);
                files = SortDirection == "asc" ? files.OrderBy(x => x.Modified) : files.OrderByDescending(x => x.Modified);
                break;
        }
        var arrowUp = _items.FirstOrDefault(x => x.DisplayType == "arrow-up");
        _items = new List<FolderItemDisplay>();
        if (arrowUp != null) _items.Add(arrowUp);
        if (SortDirection == "asc")
        {
            _items.AddRange(folders);
            _items.AddRange(files);
        }
        else
        {
            _items.AddRange(files);
            _items.AddRange(folders);
        }
        
        
        
        lvFilesAndFolders.BeginUpdate();

        // adding a glyph to indicate sorting is not as simple as it should be...
        // this is the dumbest hack ever, but it works
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

        lvFilesAndFolders.VirtualListSize = _items.Count;
        lvFilesAndFolders.Refresh();
        lvFilesAndFolders.EndUpdate();
        
    }
}