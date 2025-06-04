namespace FitFlare.Api.Helpers;

public static class ApiEndPoints
{
    private const string ApiBase = "api";

    public static class AppUser
    {
        private const string Base = $"{ApiBase}/appuser";
        public const string SignIn = $"{Base}/signin";
        public const string SignUp = $"{Base}/signup";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string EditProfile = $"{Base}/me/editprofile";
        public const string EditProfilePrivacy = $"{Base}/me/changeprivacy";
        public const string VerifyPassword = $"{Base}/me/verifypassword";
        public const string ChangePassword = $"{Base}/me/security/changepassword";
    }
    
    public static class Post
    {
        private const string Base = $"{ApiBase}/post";
        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
        public const string GetAll = $"{Base}";
        public const string Like = $"{Base}/{{id}}/like";
        public const string UnLike = $"{Base}/{{id}}/unlike";
        public const string Save = $"{Base}/{{id}}/save";
        public const string UnSave = $"{Base}/{{id}}/unsave";
    }

    public static class Comment
    {
        private const string Base = $"{ApiBase}/comment";
        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
        public const string GetByPostId = $"{Base}/post/{{postId}}";
        public const string GetAll = $"{Base}";
        public const string AddReply = $"{Base}/{{commentId}}/addreply";
        public const string Replies = $"{Base}/{{parentCommentId}}/replies";
        public const string Like = $"{Base}/{{id}}/like";
        public const string UnLike = $"{Base}/{{id}}/unlike";

    }
}