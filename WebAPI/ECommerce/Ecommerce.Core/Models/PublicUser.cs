namespace Ecommerce.Core.Models
{
    public class PublicUser
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public PublicUser(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}