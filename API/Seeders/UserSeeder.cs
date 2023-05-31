using API.Data;
using API.Entities;
using System.Security.Cryptography;
using System.Text;

namespace API.Seeders
{
    public class UserSeeder
    {
        private readonly DataContext _context;
        public UserSeeder(DataContext context)
        {
            this._context = context;
        }

        public void SeedData()
        {
            try
            {
                if (!this._context.Users.Any())
                {
                    using var hmac = new HMACSHA512();

                    var user = new AppUser
                    {
                        UserName = "frank",
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("33@Elrangers")),
                        PasswordSalt = hmac.Key,
                        FirstName = "Frank",
                        LastName = "Fontcha",
                        Email = "franckfontcha@gmail.com",
                        Role = ((int)RoleEnum.suadmin),
                        Status = ((int)StatusEnum.enable),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    this._context.Users.Add(user);

                    this._context.SaveChanges();
                }
            }
            catch (System.Exception)
            {}
        }
    }
}