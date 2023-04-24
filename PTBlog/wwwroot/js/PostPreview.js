async function TogglePreview() {
	let postPreview = document.querySelector(".post-preview");
	if (postPreview.style.display === "none") {
		await UpdatePreviewContent();
		postPreview.style.display = "block";
	} else {
		postPreview.style.display = "none";
	}
}

async function UpdatePreviewContent() {
	let postPreviewTitle = document.querySelector(".post-preview-title");
	postPreviewTitle.innerHTML = `Preview for ${document.querySelector(".title-input").value}`;

	let postPreviewContent = document.querySelector(".post-preview-content");
	let content = await RenderMarkdown(document.querySelector(".content-input").value);
	postPreviewContent.innerHTML = content;

	setTimeout(function () {
		var pres = postPreviewContent.querySelectorAll("pre>code");
		for (var i = 0; i < pres.length; i++) {
			hljs.highlightBlock(pres[i]);
		}
	});
}

async function RenderMarkdown(rawMarkdown) {
	let markdown = "";
	await $.ajax({
		contentType: "application/json; charset=utf-8",
		type: "POST",
		url: '/Markdown/ParseHTMLString',
		dataType: "json",
		data: `${JSON.stringify(rawMarkdown)}`,
		success: function (renderedMarkdown) {
			markdown = renderedMarkdown.value;
		},
		error: function (req, status, error) {
			markdown = `Error rendering markdown: ${error}`;
		}
	});

	return markdown;
}