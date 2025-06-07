using FitFlare.Application.DTOs.Tag;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class TagMapping
{
    public static TagDto MapToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            UsedCount = tag.UsedCount,
        };
    }
}