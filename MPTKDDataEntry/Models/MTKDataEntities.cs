using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace MPTKDDataEntry.Models
{
    public class MTKDataEntities : DbContext
    {
        public DbSet<BotInfo> Infos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotInfo>().ToTable("Infos");
            base.OnModelCreating(modelBuilder);
        }
    }
}