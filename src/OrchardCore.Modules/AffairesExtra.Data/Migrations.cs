using System;
using OrchardCore.Data.Migration;
using AffairesExtra.Contents.Indexes;

namespace AffairesExtra.Contents
{
    public class Migrations : DataMigration
    {
        public int Create()
        {
            SchemaBuilder.CreateMapIndexTable(nameof(FarmMachineryIndex), table => table
                .Column<string>("CategoryId")
                .Column<string>("BrandId")
                .Column<string>("RegionId")
                .Column<string>("AdvertiserId")
                .Column<bool>("Condition")
                .Column<int>("Year")
                .Column<decimal>("Price")
                .Column<string>("Description")
                .Column<string>("Published")
                .Column<DateTime>("CreatedUtc")
            );

            return 1;
        }
    }
}