using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<UserEntity>, IApplicationDbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(IDateTimeService dateTime, ICurrentUserService currentUserService,
            DbContextOptions options) : base(options)
        {
            _dateTime = dateTime;
            _currentUserService = currentUserService;
        }

        public DbSet<BuildingEntity> Buildings { get; set; }
        public DbSet<FloorEntity> Floors { get; set; }
        public DbSet<DoorLockEntity> DoorLocks { get; set; }
        public DbSet<LockAccessHistoryEntity> LockAccessHistory { get; set; }
        public override DbSet<UserEntity> Users { get; set; }
        public DbSet<UserLockClaimEntity> UserLockClaims { get; set; }

        /// <summary>
        /// Save the changes and log changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<IBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService?.UserId ?? "System";
                        entry.Entity.CreatedAt = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = _currentUserService?.UserId ?? "System";
                        entry.Entity.ModifiedAt = _dateTime.Now;
                        break;
                    default:
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>().ToTable("Users");
            base.OnModelCreating(builder);
        }
    }
}
