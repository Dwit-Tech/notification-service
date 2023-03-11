using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DwitTech.NotificationService.Data.Entities;

namespace DwitTech.NotificationService.Data.Context
{
    public class NotificationDbContext : DbContext
    {

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }
        public DbSet<Email> Emails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.AddEntityConfigurations(assembly);
            base.OnModelCreating(builder);
        }
    }
}
