using System;
using System.Threading.Tasks;
using HomeCare.Api.DAL.Repositories;
using HomeCare.Api.Data;
using HomeCare.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeCare.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private AppDbContext GetDbContext()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(opts);
        }

        // 5️⃣ Add user + fetch by email
        [Fact]
        public async Task AddUserAndRetrieveByEmail_Works()
        {
            var db = GetDbContext();
            var repo = new UserRepository(db, Mock.Of<ILogger<UserRepository>>());

            var user = new User { FullName = "Test", Email = "test@test.com", PasswordHash = "abc" };

            await repo.AddAsync(user);
            var fetched = await repo.GetByEmailAsync("test@test.com");

            Assert.NotNull(fetched);
            Assert.Equal("Test", fetched!.FullName);
        }

        // 6️⃣ EmailExists
        [Fact]
        public async Task EmailExists_ReturnsTrueForExistingEmail()
        {
            var db = GetDbContext();
            db.AppUsers.Add(new User { FullName = "A", Email = "exists@test.com", PasswordHash = "x" });
            db.SaveChanges();

            var repo = new UserRepository(db, Mock.Of<ILogger<UserRepository>>());

            Assert.True(await repo.EmailExistsAsync("exists@test.com"));
        }

        // 7️⃣ Delete user
        [Fact]
        public async Task DeleteUser_RemovesUserFromDatabase()
        {
            var db = GetDbContext();
            db.AppUsers.Add(new User { Id = 1, FullName = "Del", Email = "del@test.com", PasswordHash = "x" });
            db.SaveChanges();

            var repo = new UserRepository(db, Mock.Of<ILogger<UserRepository>>());

            var ok = await repo.DeleteUserAsync(1);
            var stillThere = await repo.GetUserByIdAsync(1);

            Assert.True(ok);
            Assert.Null(stillThere);
        }
    }
}
