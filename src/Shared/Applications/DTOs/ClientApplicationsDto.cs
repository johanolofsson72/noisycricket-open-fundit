namespace Shared.Applications.DTOs;

public class ClientApplicationsDto
{
    public List<ClientSchemaDto> ClientSchemas { get; set; } = [];
    public List<ClientApplicationStateDto> ClientApplicationStates { get; set; } = [];
}

public class ClientSchemaDto
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
}

public class ClientApplicationStateDto
{
    public int Id { get; set; } = 0;
    public int ApplicationId { get; set; } = 0;
    public string Name { get; set; } = "";
    public string TempPath { get; set; } = "";
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
}