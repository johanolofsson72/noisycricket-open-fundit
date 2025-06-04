
using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;


public class Schema
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string ClaimTag { get; set; } = "";
    public DateTime UpdatedDate { get; set; } = DateTime.MinValue;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public List<SchemaControl> Controls { get; set; } = new();
    public List<SchemaEvent> Events { get; set; } = new();
    public List<SchemaRequiredDocument> RequiredDocuments { get; set; } = new();
    public List<SchemaProgress> Progress { get; set; } = new();
    public bool Enabled { get; set; } = false;
}

public static class SchemaExtensions
{
    public static List<Schema> ToEntity(this List<SchemaDto> entities)
    {
        return entities.Select(x => x.ToEntity()).ToList();
    }
    public static List<SchemaDto> ToDto(this List<Schema> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
    
    public static SchemaDto ToDto(this Schema entity)
    {
        return new SchemaDto()
        {      
            Id = entity.Id,
            StatusId = entity.StatusId,
            Names = entity.Names,
            ClaimTag = entity.ClaimTag,
            UpdatedDate = entity.UpdatedDate,
            CreatedDate = entity.CreatedDate,
            Controls = entity.Controls.ToDto(),
            Events = entity.Events.ToSchemaEventDto(),
            RequiredDocuments = entity.RequiredDocuments.ToDto(),
            Progress = entity.Progress.ToDto(),
            Enabled = entity.Enabled
        };
    }
    
    public static Schema ToEntity(this SchemaDto entity)
    {
        return new Schema()
        {      
            Id = entity.Id,
            StatusId = entity.StatusId,
            Names = entity.Names,
            ClaimTag = entity.ClaimTag,
            UpdatedDate = entity.UpdatedDate,
            CreatedDate = entity.CreatedDate,
            Controls = entity.Controls.ToEntity(),
            Events = entity.Events.ToSchemaEvent(),
            RequiredDocuments = entity.RequiredDocuments.ToEntity(),
            Progress = entity.Progress.ToEntity(),
            Enabled = entity.Enabled
        };
    }
    
    public static OrganizationProjectDto ToOrganizationsSchemaDto(this Schema entity)
    {
        return new OrganizationProjectDto()
        {
            OrganizationProjectIdentifier = entity.Id,
            Names = entity.Names
        };
    }

    public static Schema ToEntity(this OrganizationProjectDto dto)
    {
        return new Schema()
        {
            Id = dto.OrganizationProjectIdentifier,
            Names = dto.Names
        };
    }
    
    public static Schema ToEntity(this CreateSchemaDto dto)
    {
        return new Schema
        {
            StatusId = dto.StatusId,
            Names = dto.Names,
            Controls = dto.Controls.Select(x => x.ToEntity()).ToList()
        };
    }
    
    public static Schema ToEntity(this UpdateSchemaDto dto)
    {
        return new Schema
        {
            StatusId = dto.StatusId,
            Names = dto.Names,
            Controls = dto.Controls.Select(x => x.ToEntity()).ToList()
        };
    }
    
    
}
