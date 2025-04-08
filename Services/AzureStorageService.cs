using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Threading.Tasks;

public class AzureStorageService
{
    // improve : from appsettings
    private readonly string _connectionString;
    private readonly string _containerName = "tareascontainer";  // Your container name

    public AzureStorageService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"];
    }

    public async Task<string> TestConnectionAsync()
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            bool exists = await blobContainerClient.ExistsAsync();
            if (!exists)
            {
                return $"❌ The container '{_containerName}' does not exist.";
            }

            string result = $"✅ Successfully connected to Azure Blob Storage container '{_containerName}'.\n";
            result += "📄 Listing up to 5 blobs:\n";

            int count = 0;
            int pdfCount = 0;
            long totalSize = 0;

            await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
            {
                var name = blobItem.Name;
                var size = blobItem.Properties.ContentLength ?? 0;
                var lastModified = blobItem.Properties.LastModified?.DateTime.ToLocalTime().ToString("g");

                if (name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    pdfCount++;
                }

                if (count < 5)
                {
                    result += $"- {name} | Size: {size} bytes | Last modified: {lastModified}\n";
                }

                totalSize += size;
                count++;
            }

            result += $"\n🔢 Total blobs scanned: {count}";
            result += $"\n📦 Total size of listed blobs: {totalSize} bytes";
            result += $"\n📁 Number of PDF files: {pdfCount}";

            return result;
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    public async Task<string> UploadPdfAsync(string fileName, byte[] fileContent)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync();
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream(fileContent);
            await blobClient.UploadAsync(stream, overwrite: true);

            return $"✅ PDF uploaded successfully as '{fileName}'";
        }
        catch (Exception ex)
        {
            return $"❌ Error uploading PDF: {ex.Message}";
        }
    }



}

