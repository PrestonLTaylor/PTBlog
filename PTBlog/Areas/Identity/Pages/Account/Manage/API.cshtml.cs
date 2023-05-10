using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PTBlog.Data.Repositories;

namespace PTBlog.Areas.Identity.Pages.Account.Manage;

public sealed class ApiModel : PageModel
{
    public ApiModel(IUsersRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userRepo.GetUserByClaimAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        GeneratedApiKey = await _userRepo.GenerateNewApiKeyForUserAsync(user);
        return Page();
    }

    public string? GeneratedApiKey;
    private readonly IUsersRepository _userRepo;
}
