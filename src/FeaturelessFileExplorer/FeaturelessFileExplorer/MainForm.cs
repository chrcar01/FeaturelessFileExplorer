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

    private List<FolderItemDisplay> GetFilesAndFolders(string folderPath)
    {
        var filesAndFolders = new List<FolderItemDisplay>();
        var folderInfo = new DirectoryInfo(folderPath);
        if (folderInfo.Parent is not null)
        {
            filesAndFolders.Add(new("(Parent)", folderPath, string.Empty, string.Empty, string.Empty, "arrow-up"));
        };

        filesAndFolders.AddRange(Directory
            .EnumerateDirectories(folderPath)
            .Select(d => new DirectoryInfo(d))
            .Select(di => new FolderItemDisplay(
                di.Name,
                di.FullName,
                string.Empty,
                di.CreationTime.ToString(CultureInfo.InvariantCulture),
                di.LastAccessTime.ToString(CultureInfo.InvariantCulture),
                "folder")));

        filesAndFolders.AddRange(Directory
            .EnumerateFiles(folderPath)
            .Select(f => new FileInfo(f))
            .Select(fi => new FolderItemDisplay(
                fi.Name,
                fi.FullName,
                fi.Length.ToString(),
                fi.CreationTime.ToString(CultureInfo.InvariantCulture),
                fi.LastAccessTime.ToString(CultureInfo.InvariantCulture),
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
        e.Item = new ListViewItem(new[] { fid.Name, fid.Size, fid.Created, fid.Modified })
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
}