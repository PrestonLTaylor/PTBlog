using System.Security.Claims;

namespace PTBlog.Claims;

public static class IsAdminClaim
{
	public const string Name = "isAdmin";
	public const string DefaultValue = "False";

	public static bool IsUserAnAdmin(ClaimsIdentity user)
	{
		return user.HasClaim(Name, true.ToString());
	}
}
