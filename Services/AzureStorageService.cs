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
            if (exists)
            {
                return "Successfully connected to the Azure Blob Storage container.";
            }
            else
            {
                return "The container does not exist.";
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}

