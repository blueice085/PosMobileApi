namespace PosMobileApi.Services
{
    public class BasicAuthUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public interface IBasicAuthUserService
    {
        Task<BasicAuthUser> Authenticate(string username, string password);
        Task<IEnumerable<BasicAuthUser>> GetAll();
    }

    public class BasicAuthUserService : IBasicAuthUserService
    {
        private List<BasicAuthUser> _users = new();
        public BasicAuthUserService(IConfiguration configuration)
        {
            var username = configuration["BasicAuth:username"];
            var password = configuration["BasicAuth:password"];

            _users.Add(new BasicAuthUser
            {
                UserId = Guid.NewGuid().ToString(),
                UserName = username,
                Password = password
            });
        }

        public async Task<BasicAuthUser> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.UserName == username && x.Password == password));

            if (user == null)
                return null;

            user.Password = null;
            return user;
        }

        public async Task<IEnumerable<BasicAuthUser>> GetAll()
        {
            return await Task.Run(() => _users.Select(x =>
            {
                x.Password = null;
                return x;
            }));
        }
    }
}
