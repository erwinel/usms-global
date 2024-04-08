namespace SnTsTypeGenerator.Services;

internal record SnAccessToken(string AccessToken, string RefreshToken, DateTime ExpiresOn);
