using Microsoft.Extensions.Options;

namespace CoahuilaRelacion.Api.Services;

public class FileStorageService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic",
        "image/heif"
    };

    private readonly StorageOptions _options;
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IOptions<StorageOptions> options, IWebHostEnvironment env)
    {
        _options = options.Value;
        _env = env;
    }

    public async Task<(string RelativePath, string FileName)> SaveImageAsync(IFormFile file, CancellationToken ct = default)
    {
        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Unsupported image content type.");
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var imagesRoot = Path.Combine(_env.ContentRootPath, _options.ImagesRoot);
        Directory.CreateDirectory(imagesRoot);

        var fullPath = Path.Combine(imagesRoot, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, ct);

        var relativePath = Path.Combine("uploads", fileName).Replace("\\", "/");
        return (relativePath, fileName);
    }

    public Task DeleteImageAsync(string? relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        var fileName = Path.GetFileName(relativePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Task.CompletedTask;
        }

        var imagesRoot = Path.Combine(_env.ContentRootPath, _options.ImagesRoot);
        var fullPath = Path.Combine(imagesRoot, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
