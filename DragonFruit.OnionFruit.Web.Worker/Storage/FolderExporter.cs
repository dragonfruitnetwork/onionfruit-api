// OnionFruit™ Web Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under Apache-2. Refer to the LICENSE file for more info

using System;
using System.IO;
using System.Threading.Tasks;
using DragonFruit.OnionFruit.Web.Worker.Storage.Abstractions;

namespace DragonFruit.OnionFruit.Web.Worker.Storage;

public class FolderExporter : IDataExporter
{
    public string FolderPath { get; set; }
    public Environment.SpecialFolder? SpecialBasePath { get; set; }

    public Task PerformUpload(IServiceProvider services, IUploadFileSource source)
    {
        return source.CopyToFolderAsync(GetFullPath());
    }

    private string GetFullPath() => SpecialBasePath.HasValue
        ? Path.Combine(Environment.GetFolderPath(SpecialBasePath.Value), FolderPath)
        : Path.GetFullPath(FolderPath);

    public override string ToString() => $"Local Folder Export (target: {GetFullPath()})";
}