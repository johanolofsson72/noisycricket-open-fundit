using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Messages.DTOs;

namespace Shared.Messages.Entities;


public class Message
{
    public int Id { get; set; } = 0;
    public MessageContactDto Receiver { get; set; } = new MessageContactDto();
    public int EventId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public int MessageTypeId { get; set; } = 0;
    public bool Outgoing { get; set; } = false;
    public bool Incoming { get; set; } = false;
    public int StatusId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    [MaxLength(500)] public string OrganizationName { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    [MaxLength(500)] public string ProjectTitle { get; set; } = string.Empty;
    [MaxLength(500)] public string ProjectNumber { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    [MaxLength(500)] public string ApplicationTitle { get; set; } = string.Empty;
    public int ApplicationStatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    [MaxLength(500)] public string DocumentLink { get; set; } = string.Empty;
    public int DocumentType { get; set; } = 0;
    [MaxLength(500)] public string Title { get; set; } = string.Empty;
    public int ReactionDescription { get; set; }  = 0;
    public int SystemMessageDestination { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime ExecutionDate { get; set; } = DateTime.MinValue;
    public DateTime ExpireDate { get; set; } = DateTime.MinValue;
}

public static class MessageExtensions
{
    public static IEnumerable<MessageDto> ToDto(this IEnumerable<Message> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<Message> ToEntity(this IEnumerable<MessageDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static MessageDto ToDto(this Message entity)
    {
        return new MessageDto()
        {
            Id = entity.Id,
            Receiver = entity.Receiver,
            EventId = entity.EventId,
            EventTypeId = entity.EventTypeId,
            MessageTypeId = entity.MessageTypeId,
            Outgoing = entity.Outgoing,
            Incoming = entity.Incoming,
            StatusId = entity.StatusId,
            OrganizationId = entity.OrganizationId,
            OrganizationName = entity.OrganizationName,
            ProjectId = entity.ProjectId,
            ProjectTitle = entity.ProjectTitle,
            ProjectNumber = entity.ProjectNumber,
            ApplicationId = entity.ApplicationId,
            ApplicationTitle = entity.ApplicationTitle,
            ApplicationStatusId = entity.ApplicationStatusId,
            SchemaId = entity.SchemaId,
            SchemaNames = entity.SchemaNames,
            DocumentLink = entity.DocumentLink,
            DocumentType = entity.DocumentType,
            Title = entity.Title,
            ReactionDescription = entity.ReactionDescription,
            SystemMessageDestination = entity.SystemMessageDestination,
            CreatedDate = entity.CreatedDate,
            ExecutionDate = entity.ExecutionDate,
            ExpireDate = entity.ExpireDate
        };
    }
    
    public static Message ToEntity(this MessageDto dto)
    {   
        return new Message()
        {
            Id = dto.Id,
            Receiver = dto.Receiver,
            EventId = dto.EventId,
            EventTypeId = dto.EventTypeId,
            MessageTypeId = dto.MessageTypeId,
            Outgoing = dto.Outgoing,
            Incoming = dto.Incoming,
            StatusId = dto.StatusId,
            OrganizationId = dto.OrganizationId,
            OrganizationName = dto.OrganizationName,
            ProjectId = dto.ProjectId,
            ProjectTitle = dto.ProjectTitle,
            ProjectNumber = dto.ProjectNumber,
            ApplicationId = dto.ApplicationId,
            ApplicationTitle = dto.ApplicationTitle,
            ApplicationStatusId = dto.ApplicationStatusId,
            SchemaId = dto.SchemaId,
            SchemaNames = dto.SchemaNames,
            DocumentLink = dto.DocumentLink,
            DocumentType = dto.DocumentType,
            Title = dto.Title,
            ReactionDescription = dto.ReactionDescription,
            SystemMessageDestination = dto.SystemMessageDestination,
            CreatedDate = dto.CreatedDate,
            ExecutionDate = dto.ExecutionDate,
            ExpireDate = dto.ExpireDate
        };
    }
    
}



