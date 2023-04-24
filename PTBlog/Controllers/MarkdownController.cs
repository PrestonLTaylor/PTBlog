using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;

namespace PTBlog.Controllers
{
	public class MarkdownController : Controller
	{
		[HttpPost]
		public IActionResult ParseHtmlString([FromBody]string rawMarkdown)
		{
			return Ok(Markdown.ParseHtmlString(rawMarkdown));
		}
	}
}
