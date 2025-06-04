using System;
using Shared.Documents.Entities;
using Shared.Global.Entities;

namespace Shared.Messages.DTOs;

public record ClientMessageDto(
    int Id, 
    MessageType MessageType, 
    Status Status, 
    Guid RequirementId, 
    DocumentType DocumentType, 
    int ProjectId, 
    string ProjectTitle,
    int ApplicationId, 
    string ApplicationTitle,
    Status ApplicationStatus,
    int Schema,
    string Title, 
    DateTime SentDate);