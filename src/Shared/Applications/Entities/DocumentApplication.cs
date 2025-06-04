namespace Shared.Applications.Entities;

public class DocumentApplication
{
    public int Id { get; set; } = 0;
    public int ProjectId { get; set; } = 0;
    public int SchemaId { get; set; }
    public List<ApplicationControl> Controls { get; set; } = [];
    public DateTime DecisionDate { get; set; } = DateTime.MinValue;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string ProjectNumber { get; set; } = string.Empty;
    public ApplicationContact Producer { get; set; } = new ApplicationContact();
    public ApplicationContact Organization { get; set; } = new ApplicationContact();
    public ApplicationContact ProjectManager { get; set; } = new ApplicationContact();
    public ApplicationContact ProductionManager { get; set; } = new ApplicationContact();
    public ApplicationContact ScriptManager { get; set; } = new ApplicationContact();
}

public static class DocumentApplicationExtensions
{
    public static DocumentApplicationDto ToDto(this DocumentApplication entity)
    {
        return new DocumentApplicationDto()
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            SchemaId = entity.SchemaId,
            Controls = entity.Controls.Select(x => x.ToDto()).ToList(),
            DecisionDate = entity.DecisionDate,
            SchemaNames = entity.SchemaNames,
            ProjectNumber = entity.ProjectNumber,
            Producer = entity.Producer.ToDto(),
            Organization = entity.Organization.ToDto(),
            ProjectManager = entity.ProjectManager.ToDto(),
            ProductionManager = entity.ProductionManager.ToDto(),
            ScriptManager = entity.ScriptManager.ToDto()
        };
    }

    public static DocumentApplication ToEntity(this DocumentApplicationDto dto)
    {
        return new DocumentApplication
        {
            Id = dto.Id,
            ProjectId = dto.ProjectId,
            SchemaId = dto.SchemaId,
            Controls = dto.Controls.Select(x => x.ToEntity()).ToList(),
            DecisionDate = dto.DecisionDate,
            SchemaNames = dto.SchemaNames,
            ProjectNumber = dto.ProjectNumber,
            Producer = dto.Producer.ToEntity(),
            Organization = dto.Organization.ToEntity(),
            ProjectManager = dto.ProjectManager.ToEntity(),
            ProductionManager = dto.ProductionManager.ToEntity(),
            ScriptManager = dto.ScriptManager.ToEntity()
        };
    }

    public static IEnumerable<DocumentApplicationDto> ToDto(this IEnumerable<DocumentApplication> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<DocumentApplication> ToEntity(this IEnumerable<DocumentApplicationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
}