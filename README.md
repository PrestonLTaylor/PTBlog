
# PTBlog

A blog web application made in ASP.NET using MVC.

# Startup 

To start the web application, run this command in the root folder:

```
dotnet run --project ./PTBlog/PTBlog.csproj
```

The console should show the URL that the web application is hosted on.

# Testing

To run the tests for the web application, run this command in the root folder:

```
dotnet test
```

# API Documentation
To get all posts on a page:
```
GET /api/v1/posts/

Example body:
{
	"Page": 1
}
```

To get a post by it's ID:
```
GET /api/v1/posts/{postId}
```

To create a post:
```
POST /api/v1/posts/

Example body:
{
	"Title": "Post Title",
	"Content": "Post Content",
}
```

To edit an existing post:
```
PUT /api/v1/posts/{postId}

Example body:
{
	"Title": "New Post Title",
	"Content": "New Post Content",
}
```

To delete an existing post:
```
DELETE /api/v1/posts/{postId}
```
