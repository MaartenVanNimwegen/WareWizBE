using System.Security.Cryptography;

namespace WareWiz.Services
{
    public class AuthenticateService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<AuthenticateService> _logger;
        private readonly UserService _userService;

        private const int Iterations = 10000;
        private const int SaltSize = 32;
        private const int HashSize = 32;

        public AuthenticateService(ApplicationDBContext dbContext, ILogger<AuthenticateService> logger, UserService userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;
        }

        public static string HashPassword(string password)
        {
            byte[] salt = GenerateRandomSalt();

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                byte[] hashBytes = new byte[SaltSize + HashSize];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
                Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

                string hashedPassword = Convert.ToBase64String(hashBytes);
                return hashedPassword;
            }
        }

        private static byte[] GenerateRandomSalt()
        {
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltSize];
                randomNumberGenerator.GetBytes(salt);
                return salt;
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            byte[] storedHash = new byte[HashSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(hashBytes, SaltSize, storedHash, 0, HashSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] enteredHash = pbkdf2.GetBytes(HashSize);

                for (int i = 0; i < HashSize; i++)
                {
                    if (enteredHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public async Task<bool> RegisterUser(User user)
        {
            if (user != null)
            {
                try
                {
                    var hashedPassword = HashPassword(user.Password);
                    user.Password = hashedPassword;

                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

    }
}
