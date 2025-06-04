namespace Shared.Email.Entities;

public record EmailConfiguration(
    string From,
    string SmtpServer,
    int Port,
    string UserName,
    string Password,
    bool Disabled,
    bool UseAuthentication,
    string CC,
    bool UseCC);