using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Persistence.Configuration
{
    public static class SeedData
    {
        public static async Task InitializeDataAsync(IServiceProvider service)
        {
            var context = service.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            await context.Database.EnsureCreatedAsync();

            var userAdmin = new UserEntity
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "clay@admin.com",
                NormalizedEmail = "CLAY@ADMIN.COM",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                UserType = UserTypeEnum.Admin,
                EmailConfirmed = true
            };

            if (!context.Users.Any(u => u.UserName == userAdmin.UserName))
            {
                var hashed = new PasswordHasher<UserEntity>().HashPassword(userAdmin, "SuperAdmin@2022");
                userAdmin.PasswordHash = hashed;

                var userStore = new UserStore<UserEntity>(context);
                var result = await userStore.CreateAsync(userAdmin);
                if (result.Succeeded)
                {
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Buildings.Any(b => b.Name == "Clay Solutions"))
            {
                await context.Buildings.AddAsync(new BuildingEntity
                {
                    Name = "Clay Solutions",
                    Address = "Clay Solutions B.V., Schipluidenlaan, Amsterdam, Netherlands",
                    RecordStatus = RecordStatusEnum.Active,
                    Longitude = 52.35669211861516,
                    Latitude = 4.839742794399227
                });
                await context.SaveChangesAsync();
                if (!context.Floors.Any())
                {
                    var firstBuldingId = context.Buildings.First().Id;
                    for (int i = 0; i < 3; i++)
                    {
                        if (!context.Floors.Any(b => b.Index == i))
                        {
                            await context.Floors.AddAsync(new FloorEntity
                            {
                                Index = i,
                                BuildingId = firstBuldingId,
                                RecordStatus = RecordStatusEnum.Active
                            });
                        }
                    }
                    await context.SaveChangesAsync();

                    if (!context.DoorLocks.Any())
                    {
                        var floorId = context.Floors.Where(f => f.Index == 1).First().Id;
                        await context.DoorLocks.AddAsync(new DoorLockEntity
                        {
                            FloorId = floorId,
                            DoorKeyCode = "A1C2E3G4",
                            DoorName = "Tunnel Door",
                            RecordStatus = RecordStatusEnum.Active,
                            Longitude = 52.35669211861516,
                            Latitude = 4.839742794399227
                        });
                        await context.DoorLocks.AddAsync(new DoorLockEntity
                        {
                            FloorId = floorId,
                            DoorKeyCode = "A2C3E4G5",
                            DoorName = "Office Door",
                            RecordStatus = RecordStatusEnum.Active,
                            Longitude = 52.35669211861516,
                            Latitude = 4.839742794399227
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }

        }
    }
}
