# Photosphere.SearchEngine
Simple text file indexing library

## Status
[![Build status](https://ci.appveyor.com/api/projects/status/ny3vxn69eht1j00p?svg=true)](https://ci.appveyor.com/project/sunloving/jbta)
[![NuGet](https://img.shields.io/nuget/v/Photosphere.SearchEngine.svg)](https://www.nuget.org/packages/Photosphere.SearchEngine/)
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://github.com/sunloving/photosphere-di/blob/master/LICENSE)

## Install via NuGet
```
PM> Install-Package Photosphere.SearchEngine
```

## About
The library implements means for text file indexing by words.<br/>
Features:
* build search index by files and directories;
* allow to add/remove to/from index files and directories without search locks;
* allow to search files for whole word or prefix;
* monitor files and directories for changes and operative actualize index in accordion with these changes.

With library shipped small desktop app that allows you to use all library opportunity in demo cases.

## Solution
The library main component is `SearchEngine` class, which get next functionality:
* method for adding path to index;
* method for removing path from index;
* method for search set of word/prefix entries in files;
* method for search files by word or prefix;
* set of events that raises for begining/ending file indexing/removing/updating or for file path changed.

### Index
Search index is invert index, i.e. map `'word' — 'set of entries in files that contains this word'`. As main data structure uses compressed prefix tree (PATRICIA trie). This ensures the proportionality of expected search time to length of search query.<br/>

There built direct index for fast removing of file from index by map `'file' — 'set of words from this file'`. This is necessary i.e. while file removing from file system, the file removes from index post-factum (by event). That means we doesn't have list of keys, that need to delete from index. Direct index provides such list. This is allow to avoid full index scan.<br/>

### Index maintenance
File registers into search system file by versions. A file version is complex contains with file path, last write date and creation date.<br/>

Content of added pathes monitored for file and directories creations, removings and renamings. Changes of content of indexed file leads to the file reindexing. Adding or removing a file leads to adding or removing it to/from index. File and directories renaming affects only on file pathes.<br/>

Removing of file from index is only marking non actual files versions as dead. Real index clean up will produces in background.<br/>

### Profile
* Best suited for implementations of search string with autocomplete over regular documents base.
* Best suited for not large files.
* Indexing unit is file.
* Files count doesn't matter.

## Implementation
Contains three projects:
* `Photosphere.SearchEngine`
* `Photosphere.SearchEngine.IntegrationTests`
* `Photosphere.SearchEngine.DemoApp`

Based on .NET Framework 4.7.

### Dependencies
NuGet packages:
* [`System.Runtime.CompilerServices.Unsafe`](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/) — needed for `NonBlocking.ConcurrentDictionary`;
* [`UDE.CSharp`](https://www.nuget.org/packages/UDE.CSharp) — port of Mozilla Universal Charset Detector on .NET: tool for file encoding recognition.

Vendored code, that not convenient to use as NuGet-packages:
* https://github.com/VSadov/NonBlocking — lock-free implementation of `ConcurrentDictionary`;
* https://github.com/khalidsalomao/SimpleHelpers.Net — convenient wrapper for `UDE.CSharp`;
* https://github.com/Microsoft/vscode-filewatcher-windows — mechanism for `FileSystemWatcher` events consolidation from Visual Studio Code (uses partially).

### Library
Main object is `SearchEngine` class instance, that provided all needed functionality.

#### Creation
``` C#
var searchEngine = SearchEngineFactory.New();
```
or
``` C#
var settings = new SearchEngineSettings();
var searchEngine = SearchEngineFactory.New(settings);
```
Settings object `SearchEngineSettings` has followings options:
* `SupportedFilesExtensions` — set of file extensions in lowercase; by default: `txt`, `log`, `cs`, `js`, `fs`, `css`, `sql`;
* `FileParsers` — set of parsers, that you can implement for yourself;
* `GcCollect` — flag for management force garbage collection after index cleaning;
* `CleaUpIntervalMs` — double value, determines index cleaning interval in milliseconds.

#### Adding file to index
``` C#
var isAdded = searchEngine.Add(pathToFolderOrFile);
```

#### Removing file from index
``` C#
var isRemoved = searchEngine.Remove(pathToFolderOrFile);
```

#### Events
``` C#
searchEngine.FileIndexingStarted += args => Console.WriteLine($"File {args.Path} indexing is started");
searchEngine.FileIndexingEnded += args => Console.WriteLine($"File {args.Path} indexing is ended");
searchEngine.FileRemovingStarted += args => Console.WriteLine($"File {args.Path} removing is started");
searchEngine.FileRemovingEnded += args => Console.WriteLine($"File {args.Path} removing is ended");
searchEngine.FileUpdateInitiated += args => Console.WriteLine($"File {args.Path} update is started");
searchEngine.FilePathChanged += args => Console.WriteLine($"File {args.Path} path is changed");
searchEngine.PathWatchingStarted += args => Console.WriteLine($"Path {args.Path} added to watcher");
searchEngine.PathWatchingEnded += args => Console.WriteLine($"Path {args.Path} removed from watcher");
searchEngine.FileUpdateFailed += args => Console.WriteLine($"Update of {args.Path} failed: {args.Error.Message}");
searchEngine.IndexCleanUpFailed += args => Console.WriteLine($"Index clean up failed: {args.Error.Message}");
```

#### Search
``` C#
searchEngine.Search("foo");                       // returns all entries starts with prefix "foo"
searchEngine.Search("foo", wholeWord: true);      // returns all entries of word "foo"
searchEngine.SearchFiles("foo");                  // returns all files starts with prefix "foo"
searchEngine.SearchFiles("foo", wholeWord: true); // returns all files of word "foo"
```

#### Custom file parser
For example:
``` C#
public class CsFileParser : IFileParser
{
    private static readonly string[] FilesExts = { "cs" };
    public IEnumerable<string> FileExtensions => FilesExts;

    public IEnumerable<ParsedWord> Parse(IFileVersion fileVersion, Encoding encoding = null)
    {
        // parse your file here; fileVersion.Path contains path to actual file
    }
}
```
and pass it to settings object
``` C#
var settings = new SearchEngineSettings
{
  FileParsers = new [] {new CsFileParser()}
}
var searchEngine = SearchEngineFactory.New(settings);
```
While `cs` files parsing will be apply `CsFileParser`, instead standart parser.

### Demo app
![Demo app window](https://raw.githubusercontent.com/sunloving/jbta/master/img/jbta-demo-view.png)
Simplest two-panel window. Panel with catalog tree on the left. Search index with options and result form on the right.
Search options:
* Only files — search files or prefixes;
* Whole word — by word or prefix.
