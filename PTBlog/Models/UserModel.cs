using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PTBlog.Models;

// TODO: Add maximum lengths to strings in models
public sealed class UserModel : IdentityUser
{
    [Display(Name = "Profile Picture")]
    public string ProfilePictureURL { get; set; }

    public List<PostModel> Posts { get; set; }

    public string? Username
    {
        get => UserName;
        set => UserName = value;
    }
}
