using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Microsoft.EntityFrameworkCore;
using FINAL2.Model;

namespace FINAL2.Data
{


    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Loan> Loans { get; set; }
        //public DbSet<Accountant> Accountants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loan>()
                        .HasOne(x => x.Person)
                        .WithMany()
                        .HasForeignKey(x => x.PersonId)
                        .IsRequired();



        }
    }
}