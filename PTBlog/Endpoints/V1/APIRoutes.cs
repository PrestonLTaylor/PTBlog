namespace PTBlog.Endpoints.V1;

public static class APIRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = $"{Root}/{Version}";

    public static class Posts
    {
        public const string GetAll = $"{Base}/posts";
        public const string Get = $"{Base}/posts/{{postId}}";
        public const string Create = $"{Base}/posts";
        public const string Edit = $"{Base}/posts/{{postId}}";
        public const string Delete = $"{Base}/posts/{{postId}}";
    }
}
