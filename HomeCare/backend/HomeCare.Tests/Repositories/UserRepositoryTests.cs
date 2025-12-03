using System;
using System.Threading.Tasks;
using HomeCare.Api.DAL.Repositories;
using HomeCare.Api.Data;
using HomeCare.Api.Models;
using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public async Task AddUserAndRetrieveByEmail_Works()
        {
            using var db = GetDbContext();
            var repo = new UserRepository(db);

            var user = new User { FullName = "Test", Email = "test@test.com", PasswordHash = "abc" };

            await repo.AddAsync(user);
            var fetched = await repo.GetByEmailAsync("test@test.com");

            Assert.NotNull(fetched);
            Assert.Equal("Test", fetched!.FullName);
        }

        [Fact]
        public async Task EmailExists_ReturnsTrueForExistingEmail()
        {
            using var db = GetDbContext();
            db.Users.Add(new User { FullName = "A", Email = "exists@test.com", PasswordHash = "x" });
            await db.SaveChangesAsync();

            var repo = new UserRepository(db);

            Assert.True(await repo.EmailExistsAsync("exists@test.com"));
        }
    }
}