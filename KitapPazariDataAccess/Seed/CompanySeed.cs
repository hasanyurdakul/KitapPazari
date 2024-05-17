using KitapPazariModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitapPazariDataAccess.Seed
{
    public class CompanySeed : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasData(
                new Company
                {
                    Id = 1,
                    Name = "Tech Solution",
                    StreetAddress = "123 Tech St.",
                    City = "Tech City",
                    PostalCode = "123123",
                    State = "IL",
                    PhoneNumber = "5554443322"
                }, 
                new Company
                {
                    Id = 2,
                    Name = "Readers Club",
                    StreetAddress = "Readers St.",
                    City = "Readers City",
                    PostalCode = "345345",
                    State = "IL",
                    PhoneNumber = "6665552233"
                },
                new Company
                {
                    Id = 3,
                    Name = "Vivid Books",
                    StreetAddress = "23 Vivid St.",
                    City = "Vivid City",
                    PostalCode = "890890",
                    State = "LA",
                    PhoneNumber = "8887776565"
                },
                new Company
                {
                    Id = 4,
                    Name = "Book Worms",
                    StreetAddress = "Worms St.",
                    City = "Worms City",
                    PostalCode = "567567",
                    State = "NV",
                    PhoneNumber = "9997775567"
                });
        }
    }
}
