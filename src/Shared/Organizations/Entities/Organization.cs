using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Controls.DTOs;
using Shared.Controls.Entities;
using Shared.Global.DTOs;
using Shared.Global.Entities;
using Shared.Milestones.DTOs;
using Shared.Organizations.DTOs;
using Shared.Schemas.DTOs;
using Shared.Schemas.Entities;

namespace Shared.Organizations.Entities;

public class Organization
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string Vat { get; set; } = string.Empty;
    [MaxLength(500)] public string Mail { get; set; } = string.Empty;
    [MaxLength(500)] public string Url { get; set; } = string.Empty;
    [MaxLength(250)] public string Logo { get; set; } = string.Empty;
    [MaxLength(500)] public string Country { get; set; } = string.Empty;
    public List<OrganizationAddress> Addresses { get; set; } = [];
    public List<OrganizationPhoneNumber> PhoneNumbers { get; set; } = [];
    public List<OrganizationBankInformation> BankInformation { get; set; } = [];
    
    public List<OrganizationCurrencyDto> Currencies { get; set; } = [];
    
    public List<OrganizationStatusDto> Statuses { get; set; } = [];
    
    public List<OrganizationSectionDto> Sections { get; set; } = [];
    
    public List<OrganizationActionTypeDto> ActionTypes { get; set; } = [];
    
    public List<OrganizationClaimTypeDto> ClaimTypes { get; set; } = [];
    
    public List<OrganizationEventTypeDto> EventTypes { get; set; } = [];
    
    public List<OrganizationGenderDto> Genders { get; set; } = [];
    
    public List<OrganizationMessageTypeDto> MessageTypes { get; set; } = [];
    
    public List<OrganizationPhoneNumberTypeDto> PhoneNumberTypes { get; set; } = [];
    
    public List<OrganizationReactionTypeDto> ReactionTypes { get; set; } = [];
    
    public List<OrganizationSystemMessageDestinationDto> SystemMessageDestinations { get; set; } = [];
    
    public List<OrganizationMilestoneRequirementTypeDto> MilestoneRequirementTypes { get; set; } = [];
    
    public List<OrganizationControlTypeDto> ControlTypes { get; set; } = [];
    
    public List<OrganizationDocumentTypeDto> DocumentTypes { get; set; } = [];
    
    public List<OrganizationDocumentDeliveryTypeDto> DocumentDeliveryTypes { get; set; } = [];
    
    public List<OrganizationApplicationBudgetTypeDto> ApplicationBudgetTypes { get; set; } = [];
}


public static class OrganizationExtensions
{

    public static IEnumerable<OrganizationDto> ToDto(this IEnumerable<Organization> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<Organization> ToEntity(this IEnumerable<OrganizationDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static OrganizationDto ToDto(this Organization entity)
    {
        return new OrganizationDto()
        {
            Id = entity.Id,
            StatusId = entity.StatusId,
            Name = entity.Name,
            Vat = entity.Vat,
            Mail = entity.Mail,
            Url = entity.Url,
            Logo = entity.Logo,
            Country = entity.Country,
            Addresses = entity.Addresses.ToDto().ToList(),
            PhoneNumbers = entity.PhoneNumbers.ToDto().ToList(),
            BankInformation = entity.BankInformation.ToDto().ToList(),
            Currencies = entity.Currencies,
            Statuses = entity.Statuses,
            Sections = entity.Sections,
            ActionTypes = entity.ActionTypes,
            ClaimTypes = entity.ClaimTypes,
            EventTypes = entity.EventTypes,
            Genders = entity.Genders,
            MessageTypes = entity.MessageTypes,
            PhoneNumberTypes = entity.PhoneNumberTypes,
            ReactionTypes = entity.ReactionTypes,
            SystemMessageDestinations = entity.SystemMessageDestinations,
            MilestoneRequirementTypes = entity.MilestoneRequirementTypes,
            ControlTypes = entity.ControlTypes,
            DocumentTypes = entity.DocumentTypes,
            DocumentDeliveryTypes = entity.DocumentDeliveryTypes,
            ApplicationBudgetTypes = entity.ApplicationBudgetTypes
        };
    }

    public static Organization ToEntity(this OrganizationDto dto)
    {
        return new Organization
        {
            Id = dto.Id,
            StatusId = dto.StatusId,
            Name = dto.Name,
            Vat = dto.Vat,
            Mail = dto.Mail,
            Url = dto.Url,
            Logo = dto.Logo,
            Country = dto.Country,
            Addresses = dto.Addresses.ToEntity().ToList(),
            PhoneNumbers = dto.PhoneNumbers.ToEntity().ToList(),
            BankInformation = dto.BankInformation.ToEntity().ToList(),
            Currencies = dto.Currencies,
            Statuses = dto.Statuses,
            Sections = dto.Sections,
            ActionTypes = dto.ActionTypes,
            ClaimTypes = dto.ClaimTypes,
            EventTypes = dto.EventTypes,
            Genders = dto.Genders,
            MessageTypes = dto.MessageTypes,
            PhoneNumberTypes = dto.PhoneNumberTypes,
            ReactionTypes = dto.ReactionTypes,
            SystemMessageDestinations = dto.SystemMessageDestinations,
            MilestoneRequirementTypes = dto.MilestoneRequirementTypes,
            ControlTypes = dto.ControlTypes,
            DocumentTypes = dto.DocumentTypes,
            DocumentDeliveryTypes = dto.DocumentDeliveryTypes,
            ApplicationBudgetTypes = dto.ApplicationBudgetTypes
        };
    }
}

