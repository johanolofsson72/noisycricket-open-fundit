namespace Shared.Email.Entities;

public record EmailMessage(
    string To,
    string Subject,
    string Content,
    string Attachment,
    bool Force = false);