using System.Collections.Generic;

namespace Shared.Documents.DTOs;

public class CreateDocumentTypeDto
{
    public List<string> Names { get; set; } = [];
}