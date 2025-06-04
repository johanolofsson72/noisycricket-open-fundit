using System;
using Shared.Global.Entities;

namespace Shared.Messages.DTOs;

public record AdminMessageDto(
    int Id,
    MessageType MessageType,
    int Schema,
    string ProjectNumber,
    string ProjectTitle,
    string Title,
    string Code,
    string Url,
    DateTime CreatedDate,
    DateTime ExpireDate);