using System;
using AffairesExtra.Contents.Models;
using OrchardCore.ContentManagement;
using YesSql.Indexes;
using System.Linq;
using Newtonsoft.Json;

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
                    if (contentItem.ContentType == "FarmMachinery")
                    {
                        string categoryId = null;
                        if (contentItem.Content.FarmMachinery.Category.ContentItemIds.Count > 0)
                        {
                            categoryId = contentItem.Content.FarmMachinery.Category.ContentItemIds.ToString(Formatting.None);
                        }

                        string brandId = null;
                        if (contentItem.Content.FarmMachinery.Brand.ContentItemIds.Count > 0)
                        {
                            brandId = contentItem.Content.FarmMachinery.Brand.ContentItemIds[0];
                        }

                        string regionId = null;
                        if (contentItem.Content.FarmMachinery.Region.ContentItemIds.Count > 0)
                        {
                            regionId = contentItem.Content.FarmMachinery.Region.ContentItemIds[0];
                        }

                        string advertiserId = null;
                        if (contentItem.Content.FarmMachinery.Advertiser.ContentItemIds.Count > 0)
                        {
                            advertiserId = contentItem.Content.FarmMachinery.Advertiser.ContentItemIds[0];
                        }

                        int? year = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.FarmMachinery.Year.Value.ToString()))
                        {
                            year = Convert.ToInt32(contentItem.Content.FarmMachinery.Year.Value.ToString());
                        }

                        decimal? price = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.FarmMachinery.Price.Value.ToString()))
                        {
                            price = Convert.ToDecimal(contentItem.Content.FarmMachinery.Price.Value.ToString());
                        }

                        return new FarmMachineryIndex
                        {
                            CategoryId = categoryId,
                            BrandId = brandId,
                            RegionId = regionId,
                            AdvertiserId = advertiserId,
                            Condition = contentItem.Content.FarmMachinery.Condition.Value ?? false,
                            Year = year,
                            Price = price,
                            Published = contentItem.Published,
                            Description = contentItem.Content.FarmMachinery.AdditionalDescription.Text,
                            CreatedUtc = contentItem.CreatedUtc ?? DateTime.UtcNow
                        };
                    }
                    else if(contentItem.ContentType == "FarmMachineryAccessory")
                    {
                        string categoryId = null;
                        if (contentItem.Content.FarmMachineryAccessory.Category.ContentItemIds.Count > 0)
                        {
                            categoryId = contentItem.Content.FarmMachineryAccessory.Category.ContentItemIds.ToString(Formatting.None);
                        }

                        string brandId = null;
                        if (contentItem.Content.FarmMachineryAccessory.Brand.ContentItemIds.Count > 0)
                        {
                            brandId = contentItem.Content.FarmMachineryAccessory.Brand.ContentItemIds[0];
                        }

                        string regionId = null;
                        if (contentItem.Content.FarmMachineryAccessory.Region.ContentItemIds.Count > 0)
                        {
                            regionId = contentItem.Content.FarmMachineryAccessory.Region.ContentItemIds[0];
                        }

                        string advertiserId = null;
                        if (contentItem.Content.FarmMachineryAccessory.Advertiser.ContentItemIds.Count > 0)
                        {
                            advertiserId = contentItem.Content.FarmMachineryAccessory.Advertiser.ContentItemIds[0];
                        }

                        int? year = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.FarmMachineryAccessory.Year.Value.ToString()))
                        {
                            year = Convert.ToInt32(contentItem.Content.FarmMachineryAccessory.Year.Value.ToString());
                        }

                        decimal? price = null;
                        if (!String.IsNullOrWhiteSpace(contentItem.Content.FarmMachineryAccessory.Price.Value.ToString()))
                        {
                            price = Convert.ToDecimal(contentItem.Content.FarmMachineryAccessory.Price.Value.ToString());
                        }

                        return new FarmMachineryIndex
                        {
                            CategoryId = categoryId,
                            BrandId = brandId,
                            RegionId = regionId,
                            AdvertiserId = advertiserId,
                            Condition = contentItem.Content.FarmMachineryAccessory.Condition.Value ?? false,
                            Year = year,
                            Price = price,
                            Published = contentItem.Published,
                            Description = contentItem.Content.FarmMachineryAccessory.AdditionalDescription.Text,
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
