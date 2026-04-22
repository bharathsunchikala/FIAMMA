using System.ComponentModel.DataAnnotations;

namespace Fiamma.Web.ViewModels.Manage;

public class RemoveLoginViewModel
{
    [Required]
    public string LoginProvider { get; set; } = string.Empty;
    [Required]
    public string ProviderKey { get; set; } = string.Empty;
}

