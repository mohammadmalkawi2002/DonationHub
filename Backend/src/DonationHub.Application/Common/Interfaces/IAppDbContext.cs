using DonationHub.Domain.Entities.Campaigns;
using DonationHub.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Campaign> Campaigns { get; }


    }
}

