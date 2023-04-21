using System.ComponentModel.DataAnnotations;

namespace PTBlog.Models;

public sealed record PostDTO([Required] string Title = "", [Required] string Content = "");