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
        foreach (var displayType in new[] { Constants.DISPLAY_TYPE_FILE, Constants.DISPLAY_TYPE_FOLDER, Constants.DISPLAY_TYPE_PARENT_FOLDER })
        {
            imageList.Images.Add(displayType, new Icon(Path.Combine(Environment.CurrentDirectory, $"{displayType}.ico")));
        }
    }

    private async Task LoadFilesAndFolders(string folderPath)
    {
        try
        {
            this.Cursor = Cursors.WaitCursor;
            folderPath = folderPath.Trim();
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

    private List<FolderItemDisplay> GetFilesAndFolders(string folderPath)
    {
        folderPath = $@"{folderPath}";
        var filesAndFolders = new List<FolderItemDisplay>();
        var folderInfo = new DirectoryInfo(folderPath);

        if (folderInfo.Parent is not null)
        {
            filesAndFolders.Add(new("(Parent)", folderPath, null, null, null, Constants.DISPLAY_TYPE_PARENT_FOLDER));
        };


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
            case Constants.DISPLAY_TYPE_FILE:
                var processStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = fid.FullPath
                };
                Process.Start(processStartInfo);
                break;

            case Constants.DISPLAY_TYPE_FOLDER:
                await LoadFilesAndFolders(fid.FullPath);
                break;

            case Constants.DISPLAY_TYPE_PARENT_FOLDER:
                var parentFullName = new DirectoryInfo(fid.FullPath).Parent?.FullName;
                if (parentFullName != null) await LoadFilesAndFolders(parentFullName);
                break;
        }
    }

    private void lvFilesAndFolders_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (!_items.Any()) return;

        var folders = _items.Where(x => x.DisplayType == Constants.DISPLAY_TYPE_FOLDER);
        var files = _items.Where(x => x.DisplayType == Constants.DISPLAY_TYPE_FILE);
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
        var parentFolderItem = _items.FirstOrDefault(x => x.DisplayType == Constants.DISPLAY_TYPE_PARENT_FOLDER);
        _items = new List<FolderItemDisplay>();
        if (parentFolderItem != null) _items.Add(parentFolderItem);
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

    private void lvFilesAndFolders_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;

        if (lvFilesAndFolders.GetItemAt(e.X, e.Y) is not { } lvi) return;

        var fid = (FolderItemDisplay)lvi.Tag;
        if (fid.DisplayType != Constants.DISPLAY_TYPE_FILE) return;


        var cms = new ContextMenuStrip();
        cms.Items.Add("Open", null, (s, ea) =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fid.FullPath
            };
            Process.Start(processStartInfo);
        });

        async void CopyLocalEventHandler(object s, EventArgs ea)
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

        cms.Items.Add("Copy Local", null, CopyLocalEventHandler!);

        cms.Items.Add("Copy Path", null, (s, ea) =>
        {
            Clipboard.SetText(fid.FullPath);
        });

        cms.Show(lvFilesAndFolders, e.Location);
    }
}