namespace FitFlare.Application.DTOs.Blob;

public class BlobDto
{
    public Stream Content { get; set; }
    public string ContentType { get; set; }

    public BlobDto(Stream content, string contentType)
    {
        Content = content;
        ContentType = contentType;
    }
}