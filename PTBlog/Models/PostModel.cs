using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTBlog.Models;

public sealed class PostModel
{
    [Key]
    public int Id { get; set; }

    public string AuthorId { get; set; }

	[ForeignKey("AuthorId")]
	public UserModel Author { get; set; }

	public string Title { get; set; }
	public string Content { get; set; }

	[Display(Name = "Created Date")]
	[DisplayFormat(DataFormatString="{0:d}")]
	public DateTimeOffset CreatedDate { get; set; }

	[Display(Name = "Updated Date")]
	[DisplayFormat(DataFormatString = "{0:d}")]
	public DateTimeOffset? UpdatedDate { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj is PostModel other)
		{ 
			return Title == other.Title && Content == other.Content && AuthorId == other.AuthorId;
		}

		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Title, Content, AuthorId);
	}
}
