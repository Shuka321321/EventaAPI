using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Account;

namespace DataAccess.EF;

public partial class EventaContext : IdentityDbContext<ApplicationUser>
{
    public EventaContext(DbContextOptions<EventaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Person> Persons { get; set; }

}