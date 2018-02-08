# Typed File Input Component

## For Fable and Fable.React

MacOS/Linux | Windows
--- | ---
[![Travis Badge](https://travis-ci.org/GuuD/fable-file-input.svg?branch=master)](https://travis-ci.org/GuuD/fable-file-input) | [![Build status](https://ci.appveyor.com/api/projects/status/github/GuuD/fable-file-input?svg=true)](https://ci.appveyor.com/project/GuuD/fable-file-input)
[![Build History](https://buildstats.info/travisci/chart/GuuD/fable-file-input)](https://travis-ci.org/GuuD/fable-file-input/builds) | [![Build History](https://buildstats.info/appveyor/chart/GuuD/fable-file-input)](https://ci.appveyor.com/project/GuuD/fable-file-input)


## Nuget

Stable | Prerelease
--- | ---
[![NuGet Badge](https://buildstats.info/nuget/Fable.FileInput)](https://www.nuget.org/packages/Fable.FileInput/) | [![NuGet Badge](https://buildstats.info/nuget/Fable.FileInput?includePreReleases=true)](https://www.nuget.org/packages/Fable.FileInput/)


Use `singleFileInput` from `Fable.FileInput.React` instead of regular `input` from `Fable.Import.React` if you want to handle inputs of single files without fighting with auto-bindings.

### Event Handlers

- `OnFileBytesReceived of (FileInfo<JS.ArrayBuffer> -> unit)`
- `OnDataUrlReceived of (FileInfo<string> -> unit)`
- `OnTextReceived of (FileInfo<string> -> unit)`

Where `FileInfo` is:

```fsharp
type FileInfo<'t> = { Name: string; MIME: string; Data: 't }
```

------

## Multiple Files

> Will add soon <sup>tm</sup>
