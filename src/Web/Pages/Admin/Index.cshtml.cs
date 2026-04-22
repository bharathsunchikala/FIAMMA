using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fiamma.Web.Pages.Admin;

[Authorize(Roles = "Administrators")]
public class IndexModel : PageModel
{
    public IndexModel()
    {

    }
}

