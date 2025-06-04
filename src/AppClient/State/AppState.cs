using Microsoft.AspNetCore.Components;
using Shared.Data.DbContext;
using Shared.Organizations.DTOs;
using Shared.Organizations.Services;
using Shared.Projects.DTOs;
using Shared.Schemas.DTOs;
using Shared.Users.DTOs;
using Shared.Users.Entities;
using Shared.Users.Services;

namespace AppClient.State;

public class AppState
{
    private static AppState _instance = null!;
    private static readonly Lock _lock = new Lock();
    
    private UserService _userService = null!;
    private OrganizationService _organizationService = null!;
    
    private string _message = "";
    private int _eventCount = 0;
    private UserDto _user = new();
    private string _vat = "";
    private string _email = "";
    private OrganizationDto _organization = new();
    private List<SchemaDto> _schemas = [];
    private List<OrganizationCurrencyDto> _currencies = [];
    private List<OrganizationStatusDto> _statuses = [];
    private List<OrganizationSectionDto> _sections = [];
    private List<OrganizationActionTypeDto> _actionTypes = [];
    private List<OrganizationClaimTypeDto> _claimTypes = [];
    private List<OrganizationEventTypeDto> _eventTypes = [];
    private List<OrganizationGenderDto> _genders = [];
    private List<OrganizationMessageTypeDto> _messageTypes = [];
    private List<OrganizationPhoneNumberTypeDto> _phoneNumberTypes = [];
    private List<OrganizationReactionTypeDto> _reactionTypes = [];
    private List<OrganizationSystemMessageDestinationDto> _systemMessageDestinations = [];
    private List<OrganizationMilestoneRequirementTypeDto> _milestoneRequirementTypes = [];
    private List<OrganizationControlTypeDto> _controlTypes = [];
    private List<OrganizationDocumentTypeDto> _documentTypes = [];
    private List<OrganizationDocumentDeliveryTypeDto> _documentDeliveryTypes = [];
    private List<OrganizationApplicationBudgetTypeDto> _applicationBudgetTypes = [];
    
    public static int InstanceCount { get; private set; } = 0;

    public event EventHandler StateUpdated = null!;
    private bool _isInitialized;
    public bool IsInitialized
    {
        get => _isInitialized;
        private set
        {
            _isInitialized = value;
            NotifyPropertyChanged(new("IsInitialized", value));
        }
    }
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            NotifyPropertyChanged(new("Message", value));
        }
    }
    public int EventCount
    {
        get => _eventCount;
        set
        {
            _eventCount = value;
            NotifyPropertyChanged(new("Count", value));
        }
    }
    public UserDto User
    {
        get => _user;
        set
        {
            _user = value;
            NotifyPropertyChanged(new("User", value));
        }
    }
    public OrganizationDto Organization
    {
        get => _organization;
        set
        {
            _organization = value;
            NotifyPropertyChanged(new("Organization", value));
        }
    }
    public List<OrganizationCurrencyDto> Currencies
    {
        get => _currencies;
        set
        {
            _currencies = value;
            NotifyPropertyChanged(new("Currencies", value));
        }
    }
    public List<OrganizationStatusDto> Statuses
    {
        get => _statuses;
        set
        {
            _statuses = value;
            NotifyPropertyChanged(new("Statuses", value));
        }
    }
    public List<OrganizationSectionDto> Sections
    {
        get => _sections;
        set
        {
            _sections = value;
            NotifyPropertyChanged(new("Sections", value));
        }
    }
    public List<OrganizationActionTypeDto> ActionTypes
    {
        get => _actionTypes;
        set
        {
            _actionTypes = value;
            NotifyPropertyChanged(new("ActionTypes", value));
        }
    }
    public List<OrganizationClaimTypeDto> ClaimTypes
    {
        get => _claimTypes;
        set
        {
            _claimTypes = value;
            NotifyPropertyChanged(new("ClaimTypes", value));
        }
    }
    public List<OrganizationEventTypeDto> EventTypes
    {
        get => _eventTypes;
        set
        {
            _eventTypes = value;
            NotifyPropertyChanged(new("EventTypes", value));
        }
    }
    public List<OrganizationGenderDto> Genders
    {
        get => _genders;
        set
        {
            _genders = value;
            NotifyPropertyChanged(new("Genders", value));
        }
    }
    public List<OrganizationMessageTypeDto> MessageTypes
    {
        get => _messageTypes;
        set
        {
            _messageTypes = value;
            NotifyPropertyChanged(new("MessageTypes", value));
        }
    }
    public List<OrganizationPhoneNumberTypeDto> PhoneNumberTypes
    {
        get => _phoneNumberTypes;
        set
        {
            _phoneNumberTypes = value;
            NotifyPropertyChanged(new("PhoneNumberTypes", value));
        }
    }
    public List<OrganizationReactionTypeDto> ReactionTypes
    {
        get => _reactionTypes;
        set
        {
            _reactionTypes = value;
            NotifyPropertyChanged(new("ReactionTypes", value));
        }
    }
    public List<OrganizationSystemMessageDestinationDto> SystemMessageDestinations
    {
        get => _systemMessageDestinations;
        set
        {
            _systemMessageDestinations = value;
            NotifyPropertyChanged(new("SystemMessageDestinations", value));
        }
    }
    public List<OrganizationMilestoneRequirementTypeDto> MilestoneRequirementTypes
    {
        get => _milestoneRequirementTypes;
        set
        {
            _milestoneRequirementTypes = value;
            NotifyPropertyChanged(new("MilestoneRequirementTypes", value));
        }
    }
    public List<OrganizationControlTypeDto> ControlTypes
    {
        get => _controlTypes;
        set
        {
            _controlTypes = value;
            NotifyPropertyChanged(new("ControlTypes", value));
        }
    }
    public List<OrganizationDocumentTypeDto> DocumentTypes
    {
        get => _documentTypes;
        set
        {
            _documentTypes = value;
            NotifyPropertyChanged(new("DocumentTypes", value));
        }
    }
    public List<OrganizationDocumentDeliveryTypeDto> DocumentDeliveryTypes
    {
        get => _documentDeliveryTypes;
        set
        {
            _documentDeliveryTypes = value;
            NotifyPropertyChanged(new("DocumentDeliveryTypes", value));
        }
    }
    public List<OrganizationApplicationBudgetTypeDto> ApplicationBudgetTypes
    {
        get => _applicationBudgetTypes;
        set
        {
            _applicationBudgetTypes = value;
            NotifyPropertyChanged(new("ApplicationBudgetTypes", value));
        }
    }

    private AppState()
    {
        InstanceCount++;
        Console.WriteLine($@"[AppState] Instance Count: {InstanceCount}");
    }

    public static AppState GetInstance(UserService userService, OrganizationService organizationService)
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = new AppState();
            }

            if (!_instance.IsInitialized || _instance._userService == null || _instance._organizationService == null)
            {
                _instance._userService = userService;
                _instance._organizationService = organizationService;
            }

            return _instance;
        }
    }

    public async Task InitializeAsync(string vat, string email)
    {
        if (IsInitialized)
            return;
        
        _vat = vat;
        _email = email;
            
        await LoadUserAsync(vat, email);
        
        IsInitialized = true;
        Console.WriteLine(@"[AppState] is initialized");
    }
    
    private async Task LoadUserAsync(string vat, string email)
    {
        var userResult = await _userService.GetUserAsync(0, email, vat, new CancellationToken());
        if (!userResult.IsOk) return;
            
        var user = userResult.Value;
        
        User = user.ToDto();
            
        if (User.Organizations.FirstOrDefault() is { OrganizationIdentifier: 0 }) return;
        
        var organizationResult = await _organizationService.OrganizationByIdAsync(User.Organizations.FirstOrDefault()!.OrganizationIdentifier);
        if (!organizationResult.IsOk) return;
        
        Organization = organizationResult.Value;
        Currencies = Organization.Currencies;
        Statuses = Organization.Statuses;
        Sections = Organization.Sections;
        ActionTypes = Organization.ActionTypes;
        ClaimTypes = Organization.ClaimTypes;
        EventTypes = Organization.EventTypes;
        Genders = Organization.Genders;
        MessageTypes = Organization.MessageTypes;
        PhoneNumberTypes = Organization.PhoneNumberTypes;
        ReactionTypes = Organization.ReactionTypes;
        SystemMessageDestinations = Organization.SystemMessageDestinations;
        MilestoneRequirementTypes = Organization.MilestoneRequirementTypes; 
        ControlTypes = Organization.ControlTypes;
        DocumentTypes = Organization.DocumentTypes;
        DocumentDeliveryTypes = Organization.DocumentDeliveryTypes;
        ApplicationBudgetTypes = Organization.ApplicationBudgetTypes;
    }
    
    private async Task SaveUserAsync()
    {
        var result = await _userService.UpdateUserAsync(_user.Id,
            new UpdateUserDto()
            {
                StatusId = _user.StatusId,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Addresses = _user.Addresses,
                PhoneNumbers = _user.PhoneNumbers,
                Organizations = _user.Organizations,
                Statistics = _user.Statistics,
                Favorites = _user.Favorites
            }, new CancellationToken());

        if (!result.IsOk) return;
        
        await LoadUserAsync(_vat, _email);
    }

    // Rather than having a Parameter, we are maintaining a list of callbacks
    private List<EventCallback<StatePropertyChangedArgs>> Callbacks
        = new List<EventCallback<StatePropertyChangedArgs>>();

    // Each component will register a callback
    public void RegisterCallback(EventCallback<StatePropertyChangedArgs> callback)
    {
        // Only add if we have not already registered this callback
        if (!Callbacks.Contains(callback))
        {
            Callbacks.Add(callback);
        }
    }
    
    private void NotifyPropertyChanged(StatePropertyChangedArgs args)
    {
        foreach (var callback in Callbacks)
        {
            // Ignore exceptions due to dangling references
            try
            {
                // Invoke the callback
                callback.InvokeAsync(args);
            }
            catch { }
        }
    }

    protected virtual void OnStateUpdated()
    {
        StateUpdated?.Invoke(this, EventArgs.Empty);
    }
}

public class AppStateFactory
{
    public AppState CreateAppState(UserService userService, OrganizationService organizationService)
    {
        // Använd den statiska factory-metoden för att hantera gemensam instansiering
        return AppState.GetInstance(userService, organizationService);
    }
}

public record StatePropertyChangedArgs(string PropertyName, object? NewValue);