namespace CatalogService.Api.Authorization;

// Lokal kopi af rollenavnet fra monolitten (BlazorShared.Authorization.Constants.Roles),
// så servicen ikke afhænger af frontendens projekt. Værdien skal matche rollen i JWT-tokenet.
public static class Roles
{
    public const string ADMINISTRATORS = "Administrators";
}
