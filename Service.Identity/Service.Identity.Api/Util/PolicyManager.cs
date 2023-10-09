namespace Service.Identity.Api.Util;

public static class UserPolicy
{
    public const string READ = "Auth.User.Read";
    public const string CREATE = "Auth.User.Create";
    public const string UPDATE = "Auth.User.Update";
    public const string DELETE = "Auth.User.Delete";
    public const string TwoFactorEnabled = "Auth.User.TwoFactorEnabled";

}


public static class RolePolicy
{
    public const string READ = "Auth.Role.Read";
    public const string CREATE = "Auth.Role.Create";
    public const string UPDATE = "Auth.Role.Update";
    public const string DELETE = "Auth.Role.Delete";
}