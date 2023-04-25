using System.Security.Claims;

namespace PTBlog.Claims;

public static class IsAdminRole
{
	public const string Name = "ISADMIN";

	public static bool IsUserAnAdmin(ClaimsPrincipal user)
	{
		return user.IsInRole(Name);
	}
}
