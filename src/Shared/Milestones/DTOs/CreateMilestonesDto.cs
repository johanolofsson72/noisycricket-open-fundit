namespace Shared.Milestones.DTOs;

public record CreateMilestonesDto(
    int ApplicationId,
    decimal ReceivedAmount
);