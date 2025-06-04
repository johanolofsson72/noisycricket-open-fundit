using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Applications.Entities;
using Shared.Documents.DTOs;

namespace Shared.Documents.Entities;

public class Document
{
    public int Id { get; set; } = 0;
    public int ApplicationId { get; set; }
    public int StatusId { get; set; } = 0;
    public int RequirementTypeId { get; set; } = 0;
    public int DeliveryTypeId { get; set; } = 0;
    [MaxLength(500)] public string FileName { get; set; } = string.Empty;
    [MaxLength(500)] public string MimeType { get; set; } = string.Empty;
    [MaxLength(5)] public string Extension { get; set; } = string.Empty;
    [MaxLength(500)] public string Path { get; set; } = string.Empty;
    [MaxLength(5000)] public string Phrases { get; set; } = string.Empty;
    [MaxLength(5000)] public string Summarize { get; set; } = string.Empty;
    public byte[] Binary { get; set; } = new byte[0];
    public List<DocumentMetaData> Metadata { get; set; } = new();
    public bool IsDelivered { get; set; }
    public bool IsSigned { get; set; }
    public bool IsCertified { get; set; }
    public bool IsLocked { get; set; }
    public DateTime DeliverDate { get; set; } = DateTime.MinValue;
    public DateTime SignedDate { get; set; } = DateTime.MinValue;
    public DateTime CertifiedDate { get; set; } = DateTime.MinValue;
    public DateTime LockedDate { get; set; } = DateTime.MinValue;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
}

public static class DocumentExtensions
{
    public static DocumentDto ToDto(this Document entity) =>
        new()
        {
            Id = entity.Id,
            ApplicationId = entity.ApplicationId,
            StatusId = entity.StatusId,
            RequirementTypeId = entity.RequirementTypeId,
            DeliveryTypeId = entity.DeliveryTypeId,
            FileName = entity.FileName,
            MimeType = entity.MimeType,
            Extension = entity.Extension,
            Path = entity.Path,
            Phrases = entity.Phrases,
            Summarize = entity.Summarize,
            Binary = entity.Binary,
            Metadata = entity.Metadata.Select(x => x.ToDto()).ToList(),
            IsDelivered = entity.IsDelivered,
            IsSigned = entity.IsSigned,
            IsCertified = entity.IsCertified,
            IsLocked = entity.IsLocked,
            DeliverDate = entity.DeliverDate,
            SignedDate = entity.SignedDate,
            CertifiedDate = entity.CertifiedDate,
            LockedDate = entity.LockedDate,
            CreatedDate = entity.CreatedDate
        };
    
    public static Document ToEntity(this DocumentDto dto) => 
        new()
        {  
            Id = dto.Id,
            ApplicationId = dto.ApplicationId,
            StatusId = dto.StatusId,
            RequirementTypeId = dto.RequirementTypeId,
            DeliveryTypeId = dto.DeliveryTypeId,
            FileName = dto.FileName,
            MimeType = dto.MimeType,
            Extension = dto.Extension,
            Path = dto.Path,
            Phrases = dto.Phrases,
            Summarize = dto.Summarize,
            Binary = dto.Binary,
            Metadata = dto.Metadata.Select(x => x.ToEntity()).ToList(),
            IsDelivered = dto.IsDelivered,
            IsSigned = dto.IsSigned,
            IsCertified = dto.IsCertified,
            IsLocked = dto.IsLocked,
            DeliverDate = dto.DeliverDate,
            SignedDate = dto.SignedDate,
            CertifiedDate = dto.CertifiedDate,
            LockedDate = dto.LockedDate,
            CreatedDate = dto.CreatedDate
        };
}




