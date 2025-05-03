namespace FitFlare.Api.Helpers;

public static class ApiEndPointsAdmin
{
    private const string ApiBase = "api/admin";

    public static class AppUser
    {
        private const string Base = $"{ApiBase}/appuser";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Ban = $"{Base}/{{id}}/ban";
    }
}