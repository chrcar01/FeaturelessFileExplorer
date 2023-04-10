# FeaturelessFileExplorer
Barebones file explorer for Windows with as few features as possible to keep it simple.  
![Featureless File Explorer](assets/file-explorer.png)
## Why

My company uses a separate privileged account to access internal resources different than the account used to login in to our [Windows] laptops.  

A need popped up to be able to open Windows Explorer to a location where that privileged account was granted access but the one used to login to Windows was not.  Getting Windows Explorer to open as a different user was not working.  Trying to get approval from the security group to use 3rd party alternatives to Windows Explorer was time consuming.  

**So the Featureless File Explorer was born** as a tool where the source code is out in the wild, can be quickly and easily scanned for evil, and does NOT use any 3rd party components.

## Included Features
* List files and folders in any directory either locally or via UNC path
* Fast and responsive UI, no blocking
* Open any file using default app associated to the file extension
* Copy any file locally
* Copy the path to any file
* Navigate up a directory tree
* Minimal lines of code that are very easy to read
* No 3rd party dependencies

## Features NOT Included
* Treeview, this added too much unnecessary complexity, this isn't supposed to replace Windows File Explorer
* Custom file and folder icons.  While pretty, in directories with many files and folders, this added considerable time as well as required Win32 Api calls which would have locked in the OS platform
* Different views are not allowed, it's Details view for everyone
* Any extra operations done on files and folders as part of the context menu or otherwise; there are only 3 operations available to performan on files: Open, Copy Local and Copy Path, that's it

![Context Menu](assets/context-menu.png)