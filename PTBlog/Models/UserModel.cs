using System.ComponentModel.DataAnnotations;

namespace PTBlog.Models;

public sealed class UserModel
{
    [Key]
    public Guid Id { get; set; }

    [Display(Name = "Profile Picture")]
    public string ProfilePictureURL { get; set; }

    public string Username { get; set; }

    public List<PostModel> Posts { get; set; }
}
