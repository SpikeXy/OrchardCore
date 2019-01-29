using System;

namespace AffairesExtra.Contents.Models
{
    /// <summary>
    /// Represents a FarmMachinery instance.
    /// </summary>
    public class FarmMachinery
    {
        public int Id { get; set; }

        /// <summary>
        /// A unique identifier for this FarmMachinery.
        /// </summary>
        public string FarmMachineryId { get; set; }

        /// <summary>
        /// A unique identifier for this FarmMachinery category.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// A unique identifier for this FarmMachinery brand.
        /// </summary>
        public string BrandId { get; set; }

        /// <summary>
        /// A unique identifier for this FarmMachinery region.
        /// </summary>
        public string Regiond { get; set; }

        /// <summary>
        /// A unique identifier for this FarmMachinery advertiser.
        /// </summary>
        public string AdvertiserId { get; set; }

        /// <summary>
        /// FarmMachinery make year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// FarmMachinery price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// FarmMachinery description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date that the content item has been created.
        /// </summary>
        public DateTime CreatedUtc { get; set; }
    }
}
