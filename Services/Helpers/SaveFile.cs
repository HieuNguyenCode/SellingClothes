using Microsoft.AspNetCore.Http;

namespace Services.Helpers;

public static class SaveFile
{
    public static async Task<string> SaveFileAsync(IFormFile? file, string folderName = "images")
    {
        if (file == null || file.Length == 0) return string.Empty;

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/{folderName}/{fileName}";
    }

    public static async Task<List<string>> SaveFileAsyncRange(List<IFormFile>? files, string folderName = "images")
    {
        var uploadedUrls = new List<string>();

        if (files == null || files.Count == 0) return uploadedUrls;

        foreach (var file in files)
        {
            var url = await SaveFileAsync(file, folderName);
            if (!string.IsNullOrEmpty(url)) uploadedUrls.Add(url);
        }

        return uploadedUrls;
    }

    public static void DeleteFile(string? fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;
        var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (File.Exists(physicalPath)) File.Delete(physicalPath);
    }

    public static void DeleteFileRange(List<string>? fileUrls)
    {
        if (fileUrls == null || fileUrls.Count == 0) return;

        foreach (var url in fileUrls)
            DeleteFile(url);
    }
}