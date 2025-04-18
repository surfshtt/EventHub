namespace EventHub.Models
{
    public class User
    {
        public string userName { set; get; }
        public string password { set; get; }
        public string role { set; get; }
        public string gmail { set; get; }

        public User(string userName, string password, string role, string gmail)
        {
            this.userName = userName;
            this.password = password;
            this.role = role;
            this.gmail = gmail;
        }
    }
}
