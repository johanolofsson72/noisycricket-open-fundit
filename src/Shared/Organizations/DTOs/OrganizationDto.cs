using System.Collections.Generic;
using Shared.Controls.DTOs;
using Shared.Global.DTOs;
using Shared.Milestones.DTOs;
using Shared.Schemas.DTOs;

namespace Shared.Organizations.DTOs;

public class OrganizationDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Vat { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<OrganizationAddressDto> Addresses { get; set; } = new();
    public List<OrganizationPhoneNumberDto> PhoneNumbers { get; set; } = new();
    public List<OrganizationBankInformationDto> BankInformation { get; set; } = new();
    
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



