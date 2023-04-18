using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTBlog.Models;

public sealed class PostModel
{
    [Key]
    public int Id { get; set; }

    public Guid AuthorGuid { get; set; }

	[ForeignKey("AuthorGuid")]
	public UserModel Author { get; set; }

	public string Title { get; set; }
	public string Content { get; set; }

	[Display(Name = "Created Date")]
	[DisplayFormat(DataFormatString="{0:d}")]
	public DateTimeOffset CreatedDate { get; set; }

	[Display(Name = "Updated Date")]
	[DisplayFormat(DataFormatString = "{0:d}")]
	public DateTimeOffset? UpdatedDate { get; set; }
}
