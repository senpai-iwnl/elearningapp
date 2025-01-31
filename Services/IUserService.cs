using e_learning_app.Models;

namespace e_learning_app.Services;

public interface IUserService
{
    Task<User?> AuthenticateUser(string email, string password);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByEmail(string email);
    Task CreateUser(User user);
    Task UpdateUser(Guid id, User user);
    Task DeleteUser(Guid id);
}