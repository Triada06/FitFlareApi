namespace FitFlare.Api.Helpers;

public static class ApiEndPointsAdmin
{
    private const string ApiBase = "api/admin";

    public static class AppUser
    {
        private const string Base = $"{ApiBase}/appuser";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
    }

    public static class DashBoard
    {
        private const string Base = $"{ApiBase}/dashboard";
        public const string Monthly = $"{Base}/monthly";
    }

    public static class Ban
    {
        private const string Base = $"{ApiBase}/ban";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }
    public static class Post
    {
        private const string Base = $"{ApiBase}/post";
        public const string TakeDown = $"{Base}/{{id}}";
    }

    public static class Admin
    {
        private const string Base = $"{ApiBase}/admins";
        public const string GetAll = $"{Base}";
        public const string MakeAppOwner = $"{Base}/{{id}}/make-app-owner";
        public const string ConfirmOwnerTransfer = $"{Base}/confirm-owner-transfer";
        public const string PromoteToAdmin = $"{Base}/{{id}}/promote-to-admin";
        public const string RemoveAdmin = $"{Base}/{{id}}/remove-admin";
        public const string FindByEmail = $"{Base}/find-by-email";
    }
}