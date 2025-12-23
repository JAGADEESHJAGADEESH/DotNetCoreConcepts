namespace AuthService.Core.Constants
{
    public static class StoredProcedureNames
    {
        public const string USP_RegisterUser = "USP_RegisterUser";
        public const string USP_GetUserAuthInfo = "USP_GetUserAuthInfo";
        public const string USP_GetRoleByRoleId = "USP_GetRoleByRoleId";

        public const string USP_CreateRefreshToken = "USP_CreateRefreshToken";
        public const string USP_GetRefreshTokenByHash = "USP_GetRefreshTokenByHash";
        public const string USP_RevokeRefreshToken = "USP_RevokeRefreshToken";
    }
}
