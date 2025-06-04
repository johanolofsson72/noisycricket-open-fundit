
namespace Shared.Applications.Entities;

public class ApplicationNameEmailPhonenumberGender
{
    public int NameEmailPhonenumberGenderIdentifier { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phonenumber { get; set; } = string.Empty;
    public int GenderId { get; set; } = 0;
}