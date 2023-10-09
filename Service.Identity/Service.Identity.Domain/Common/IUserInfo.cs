namespace Service.Identity.Domain.Common
{
    public interface IUserInfo
    {
        public long? UserId { get; }
        public string? Token { get; }
    }
}
