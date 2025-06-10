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
        public const string Search = $"{Base}/search";
        public const string Stories = $"{Base}/{{id}}/stories";
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
        public const string GetByTag = $"{Base}/bytag/{{tagId}}";
        public const string Feed = $"{Base}/feed";

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

    public static class Tag
    {
        private const string Base = $"{ApiBase}/tag";
        public const string Search = $"{Base}/search";
    }

    public static class Follow
    {
        private const string Base = $"{ApiBase}/follow";
        public const string FollowUser = $"{Base}/{{userId}}";
        public const string UnFollowUser = $"{Base}/{{userId}}";
        public const string GetFollowersByIdUserId = $"{Base}/{{userId}}/followers";
        public const string GetFollowingsByIdUserId = $"{Base}/{{userId}}/followings";
        public const string AcceptFollowRequest = $"{Base}/{{userId}}/acceptfollowrequest";
    }

    public static class Notification
    {
        private const string Base = $"{ApiBase}/notification";
        public const string Create = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";
        public const string MarkAsRead = $"{Base}/{{id}}/markasread";
        public const string MarkAllAsRead = $"{Base}/markallasread";
        public const string GetAllByUserId = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
    }

    public static class Story
    {
        private const string Base = $"{ApiBase}/story";
        public const string Create = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
        public const string GetViewers = $"{Base}/{{id}}/viewers";
        public const string View = $"{Base}/{{id}}/view";
        public const string AuthUserStories = $"{Base}";
    }
}