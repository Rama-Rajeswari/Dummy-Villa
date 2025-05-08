using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<VillaFacility>()
           .HasKey(vf => new { vf.VillaId, vf.FacilityId }); 
          modelBuilder.Entity<VillaFacility>()
          .HasOne(vf => vf.Villa)
          .WithMany(v => v.VillaFacilities)
          .HasForeignKey(vf => vf.VillaId);

         modelBuilder.Entity<VillaFacility>()
          .HasOne(vf => vf.Facility)
          .WithMany(f => f.VillaFacilities)
          .HasForeignKey(vf => vf.FacilityId);
        }

        public DbSet<RoomPricing> RoomPricings{get;set;}
        public DbSet<RoomAvailability> RoomAvailabilities{get;set;}
        public DbSet<RoomGuestType> RoomGuestTypes{get;set;}
        public DbSet<Destination>Destinations{get;set;}
        public DbSet<GuestType>GuestTypes{get;set;}
        public DbSet<Room>Rooms{get;set;}
        public DbSet<Facility>Facilities{get;set;}
        public DbSet<VillaFacility>VillaFacilities{get;set;}
        public DbSet<Booking>Bookings{get;set;}
        
        public DbSet<ApplicationUser> ApplicationUsers{get;set;}
         public DbSet<LocalUser> LocalUsers{get;set;}
        public DbSet<Villa> Villas{get;set;}
        public DbSet<VillaNumber> VillaNumbers{get;set;}
       
    }
    
}