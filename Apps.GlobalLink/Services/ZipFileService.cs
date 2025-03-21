using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Apps.GlobalLink.Services;

public class ZipFileService(IFileManagementClient fileManagementClient) : IZipFileService
{
    private readonly IFileManagementClient _fileManagementClient = fileManagementClient;

    public async Task<List<FileReference>> ExtractZipFilesAsync(MemoryStream zipStream)
    {
        using var zip = new ZipFile(zipStream);
        var files = new List<FileReference>();
        
        foreach (ZipEntry entry in zip)
        {
            if (!entry.CanDecompress || entry.IsDirectory)
            {
                continue;
            }

            using var stream = zip.GetInputStream(entry);
            var fileName = NormalizeFileName(entry.Name);
            var uploadedFile = await _fileManagementClient.UploadAsync(
                stream, 
                MimeTypes.GetMimeType(fileName), 
                fileName);
                
            files.Add(uploadedFile);
        }

        return files;
    }

    public string ExtractFileNameFromContentDisposition(string? contentDisposition)
    {
        if (string.IsNullOrEmpty(contentDisposition))
        {
            return "download.zip";
        }

        try
        {
            return new System.Net.Mime.ContentDisposition(contentDisposition).FileName
                ?? "download.zip";
        }
        catch
        {
            var match = System.Text.RegularExpressions.Regex
                .Match(contentDisposition, @"filename=""?([^""]+)""?");
            return match.Success ? match.Groups[1].Value : "download.zip";
        }
    }

    private string NormalizeFileName(string fileName)
    {
        if(fileName.Contains("/"))
        {
            fileName = fileName.Split('/').LastOrDefault() ?? fileName;
        }
        if (fileName.Contains("\\"))
        {
            fileName = fileName.Split('\\').LastOrDefault() ?? fileName;
        }
        return fileName;
    }
}
