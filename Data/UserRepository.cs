using DotnetApi.Models;

namespace DotnetApi.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }
        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Remove(entityToAdd);
            }
        }
        public IEnumerable<User> GetUsers()
        {
            return _entityFramework.Users.ToList<User>();
        }
        public User GetSingleUser(int userId)
        {
            User? user = _entityFramework
            .Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();

            if (user != null)
            {
                return user;
            }

            throw new Exception($"Failed get user by id'${userId}'");
        }
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            UserJobInfo? user = _entityFramework
                .UserJobInfo
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserJobInfo>();

            if (user != null)
            {
                return user;
            }

            throw new Exception($"Failed get user job info by id'${userId}'");
        }
        public UserSalary GetSingleUserSalary(int userId)
        {
            UserSalary? user = _entityFramework
            .UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault<UserSalary>();

            if (user != null)
            {
                return user;
            }

            throw new Exception($"Failed get user salary by id'${userId}'");
        }
    }

}