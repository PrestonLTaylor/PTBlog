using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PTBlog.Data.Repositories;

namespace PTBlog.Areas.Identity.Pages.Account.Manage;

public sealed class RevokeAPIKeyModel : PageModel
{
    public RevokeAPIKeyModel(IUsersRepository userRepo)
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

        await _userRepo.RevokeApiKeyForUserAsync(user);
        return Redirect("API");
    }

    private readonly IUsersRepository _userRepo;
}
