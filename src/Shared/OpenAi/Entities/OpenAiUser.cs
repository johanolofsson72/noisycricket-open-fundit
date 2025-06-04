using System;
using System.Collections.Generic;

namespace Shared.OpenAi.Entities;

public class OpenAiUser
{
    public int Id { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public string UserName { get; set; } = string.Empty;
    public List<OpenAiUserOrganization> Organizations { get; set; } = [];
    public DateTime ExpireDate { get; set; } = DateTime.MinValue; 
}