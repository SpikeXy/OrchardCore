using System;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace AffairesExtra.Contents.Indexes
{
    public class FarmMachineryIndex : MapIndex
    {
        public string CategoryId { get; set; }
        public string BrandId { get; set; }
        public string RegionId { get; set; }
        public string AdvertiserId { get; set; }
        public bool Condition { get; set; }
        public int? Year { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedUtc { get; set; }
    }

    public class FarmMachineryIndexProvider : IndexProvider<ContentItem>
    {
        public override void Describe(DescribeContext<ContentItem> context)
        {
            context.For<FarmMachineryIndex>()
                .Map(contentItem =>
                {
                    if (contentItem.ContentType == "MotorEquipment")
                    {
                        string categoryId = null;
                        if (contentItem.Content.MotorEquipment.Category.ContentItemIds.Count > 0)
                        {
                            categoryId = contentItem.Content.MotorEquipment.Category.ContentItemIds.ToString(Formatting.None);
                        }

                        string brandId = null;
                        if (contentItem.Content.MotorEquipment.Brand.ContentItemIds.Count > 0)
                        {
                            brandId = contentItem.Content.MotorEquipment.Brand.ContentItemIds[0];
                        }

                        string regionId = null;
                        if (contentItem.Content.MotorEquipment.Region.ContentItemIds.Count > 0)
                        {
                            regionId = contentItem.Content.MotorEquipment.Region.ContentItemIds[0];
                        }

                        string advertiserId = null;
                        if (contentItem.Content.MotorEquipment.Advertiser.ContentItemIds.Count > 0)
                        {
                            advertiserId = contentItem.Content.MotorEquipment.Advertiser.ContentItemIds[0];
                        }

                        int? year = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.MotorEquipment.Year.Value.ToString()))
                        {
                            year = Convert.ToInt32(contentItem.Content.MotorEquipment.Year.Value.ToString());
                        }

                        decimal? price = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.MotorEquipment.Price.Value.ToString()))
                        {
                            price = Convert.ToDecimal(contentItem.Content.MotorEquipment.Price.Value.ToString());
                        }

                        return new FarmMachineryIndex
                        {
                            CategoryId = categoryId,
                            BrandId = brandId,
                            RegionId = regionId,
                            AdvertiserId = advertiserId,
                            Condition = contentItem.Content.MotorEquipment.Condition.Value ?? false,
                            Year = year,
                            Price = price,
                            Published = contentItem.Published,
                            Description = contentItem.Content.MotorEquipment.AdditionalDescription.Text,
                            CreatedUtc = contentItem.CreatedUtc ?? DateTime.UtcNow
                        };
                    }
                    else if (contentItem.ContentType == "AccessoryEquipment")
                    {
                        string categoryId = null;
                        if (contentItem.Content.AccessoryEquipment.Category.ContentItemIds.Count > 0)
                        {
                            categoryId = contentItem.Content.AccessoryEquipment.Category.ContentItemIds.ToString(Formatting.None);
                        }

                        string brandId = null;
                        if (contentItem.Content.AccessoryEquipment.Brand.ContentItemIds.Count > 0)
                        {
                            brandId = contentItem.Content.AccessoryEquipment.Brand.ContentItemIds[0];
                        }

                        string regionId = null;
                        if (contentItem.Content.AccessoryEquipment.Region.ContentItemIds.Count > 0)
                        {
                            regionId = contentItem.Content.AccessoryEquipment.Region.ContentItemIds[0];
                        }

                        string advertiserId = null;
                        if (contentItem.Content.AccessoryEquipment.Advertiser.ContentItemIds.Count > 0)
                        {
                            advertiserId = contentItem.Content.AccessoryEquipment.Advertiser.ContentItemIds[0];
                        }

                        int? year = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.AccessoryEquipment.Year.Value.ToString()))
                        {
                            year = Convert.ToInt32(contentItem.Content.AccessoryEquipment.Year.Value.ToString());
                        }

                        decimal? price = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.AccessoryEquipment.Price.Value.ToString()))
                        {
                            price = Convert.ToDecimal(contentItem.Content.AccessoryEquipment.Price.Value.ToString());
                        }

                        return new FarmMachineryIndex
                        {
                            CategoryId = categoryId,
                            BrandId = brandId,
                            RegionId = regionId,
                            AdvertiserId = advertiserId,
                            Condition = contentItem.Content.AccessoryEquipment.Condition.Value ?? false,
                            Year = year,
                            Price = price,
                            Published = contentItem.Published,
                            Description = contentItem.Content.AccessoryEquipment.AdditionalDescription.Text,
                            CreatedUtc = contentItem.CreatedUtc ?? DateTime.UtcNow
                        };
                    }
                    else if (contentItem.ContentType == "OtherEquipment")
                    {
                        string categoryId = null;
                        if (contentItem.Content.OtherEquipment.Category.ContentItemIds.Count > 0)
                        {
                            categoryId = contentItem.Content.OtherEquipment.Category.ContentItemIds.ToString(Formatting.None);
                        }

                        string brandId = null;
                        if (contentItem.Content.OtherEquipment.Brand.ContentItemIds.Count > 0)
                        {
                            brandId = contentItem.Content.OtherEquipment.Brand.ContentItemIds[0];
                        }

                        string regionId = null;
                        if (contentItem.Content.OtherEquipment.Region.ContentItemIds.Count > 0)
                        {
                            regionId = contentItem.Content.OtherEquipment.Region.ContentItemIds[0];
                        }

                        string advertiserId = null;
                        if (contentItem.Content.OtherEquipment.Advertiser.ContentItemIds.Count > 0)
                        {
                            advertiserId = contentItem.Content.OtherEquipment.Advertiser.ContentItemIds[0];
                        }

                        int? year = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.OtherEquipment.Year.Value.ToString()))
                        {
                            year = Convert.ToInt32(contentItem.Content.OtherEquipment.Year.Value.ToString());
                        }

                        decimal? price = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.OtherEquipment.Price.Value.ToString()))
                        {
                            price = Convert.ToDecimal(contentItem.Content.OtherEquipment.Price.Value.ToString());
                        }

                        return new FarmMachineryIndex
                        {
                            CategoryId = categoryId,
                            BrandId = brandId,
                            RegionId = regionId,
                            AdvertiserId = advertiserId,
                            Condition = contentItem.Content.OtherEquipment.Condition.Value ?? false,
                            Year = year,
                            Price = price,
                            Published = contentItem.Published,
                            Description = contentItem.Content.OtherEquipment.AdditionalDescription.Text,
                            CreatedUtc = contentItem.CreatedUtc ?? DateTime.UtcNow
                        };
                    }

                    return null;
                });
        }
    }
}
