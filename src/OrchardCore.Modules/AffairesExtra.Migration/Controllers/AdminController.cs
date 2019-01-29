using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AffairesExtra.Migration.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.Users;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using YesSql;

namespace AffairesExtra.Migration.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly MigrationService _migrationService;
        private readonly IContentManager _contentManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly ISession _session;
        private readonly IDisplayManager<User> _userDisplayManager;
        private readonly UserManager<IUser> _userManager;
        private readonly IUserService _userService;
        private static string tenantName = "Default";
        private readonly string mainImportPicturePath = "C:\\Users\\jasmi\\OneDrive\\Bureau\\Affaires Extra\\import-principale\\";
        private readonly string webImportPicturePath = "C:\\Users\\jasmi\\OneDrive\\Bureau\\Affaires Extra\\import-siteweb\\";
        private readonly string FarmMachineryMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\machinerie agricole\\principale\\";
        private readonly string FarmMachineryWebPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\machinerie agricole\\site web\\";
        private readonly string AdvertiserMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\annonceur\\";
        private readonly string FarmMachineryAccessoryMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\machinerie agricole (accessoire)\\principale\\";
        private readonly string FarmMachineryAccessoryWebPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\machinerie agricole (accessoire)\\site web\\";
        private readonly string OtherEquipmentMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\autre équipement\\principale\\";
        private readonly string OtherEquipmentWebPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\autre équipement\\site web\\";
        private readonly string RealEstateMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\immobilier\\principale\\";
        private readonly string RealEstateWebPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\immobilier\\site web\\";
        private readonly string ClassifiedAdMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\annonce classée\\principale\\";
        private readonly string ClassifiedAdWebPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\annonce classée\\site web\\";
        private readonly string ServiceMainPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\service\\principale\\";
        private readonly string ServiceWebPicturePath = "E:\\Repositories\\AffairesExtra - OrchardCore\\src\\OrchardCore.Cms.Web\\App_Data\\Sites\\" + tenantName + "\\Media\\service\\site web\\";

        public AdminController(
            MigrationService migrationService,
            IContentItemDisplayManager contentItemDisplayManager,
            IAuthorizationService authorizationService,
            ISession session,
            IContentManager contentManager,
            IDisplayManager<User> userDisplayManager,
            UserManager<IUser> userManager,
            IUserService userService)
        {
            _migrationService = migrationService;
            _authorizationService = authorizationService;
            _contentManager = contentManager;
            _contentItemDisplayManager = contentItemDisplayManager;
            _session = session;
            _userDisplayManager = userDisplayManager;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            return View();
        }

        private static void CleanFolder(string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllContentItemsFromTypeAsync(string contentType)
        {
            var query = _session.Query<ContentItem, ContentItemIndex>();
            query = query.With<ContentItemIndex>(x => x.ContentType == contentType).With<ContentItemIndex>(x => x.Latest);

            return await query.ListAsync();
        }


        public string Slugify(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var sb = new StringBuilder();

            var stFormKD = text.ToString().Trim().ToLower().Normalize(NormalizationForm.FormKD);
            foreach (var t in stFormKD)
            {
                // Allowed symbols
                if (t == '-' || t == '_' || t == '~')
                {
                    sb.Append(t);
                    continue;
                }

                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                switch (uc)
                {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.OtherLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        // Keep letters and digits
                        sb.Append(t);
                        break;
                    case UnicodeCategory.NonSpacingMark:
                        // Remove diacritics
                        break;
                    default:
                        // Replace all other chars with dash
                        sb.Append('-');
                        break;
                }
            }

            var slug = sb.ToString().Normalize(NormalizationForm.FormC);

            // Simplifies dash groups
            for (var i = 0; i < slug.Length - 1; i++)
            {
                if (slug[i] == '-')
                {
                    var j = 0;
                    while (i + j + 1 < slug.Length && slug[i + j + 1] == '-')
                    {
                        j++;
                    }
                    if (j > 0)
                    {
                        slug = slug.Remove(i + 1, j);
                    }
                }
            }

            if (slug.Length > 1000)
            {
                slug = slug.Substring(0, 1000);
            }

            slug = slug.Trim('-', '_', '.');

            return slug;
        }

        [HttpPost, ActionName("ImportAdvertisers")]
        public async Task<ActionResult> ImportAdvertisers()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(AdvertiserMainPicturePath);

            var products = await _migrationService.GetImportProductsData();

            //create advertisers
            var advertisersList = products.Where(x => x.IsAdvertiser == "Oui");
            foreach (var advertiser in advertisersList)
            {
                var contentItem = await _contentManager.NewAsync("Advertiser");

                await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                contentItem.DisplayText = advertiser.Name.ToString().Trim();
                //contentItem.Content.TitlePart.Title = advertiser.Name.ToString().Trim();
                contentItem.Content.AutoroutePart.Path = "annonceur/" + Slugify(advertiser.Name);
                contentItem.Content.Advertiser.ClientNo.Text = advertiser.ClientNo;
                contentItem.Content.Advertiser.Email.Text = advertiser.AdvertiserEmailContact;

                string image = advertiser.PrincipalImage.ToString();

                if (image != "")
                {
                    if (System.IO.File.Exists(mainImportPicturePath + image.Split('/').Last()) && !System.IO.File.Exists(AdvertiserMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + image.Split('/').Last()).ToLower()))
                    {
                        System.IO.File.Copy(mainImportPicturePath + image.Split('/').Last(), AdvertiserMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + image.Split('/').Last()).ToLower());
                        contentItem.Content.Advertiser.Logo.Paths.Add("annonceur/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + image.Split('/').Last()).ToLower());
                    }
                }

                await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                // creates a published item
                await _contentManager.PublishAsync(contentItem);
            }

            return View();
        }

        [HttpPost, ActionName("ImportUsers")]
        public async Task<ActionResult> ImportUsers()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            var products = await _migrationService.GetImportProductsData();

            //create advertisers
            var advertisersList = products.Where(x => x.IsAdvertiser == "Oui" && x.AdvertiserEmailContact != "");
            foreach (var advertiser in advertisersList)
            {

                var user = new User();
                user.Email = advertiser.AdvertiserEmailContact;
                user.UserName = advertiser.AdvertiserEmailContact;
                user.RoleNames = new string[] { "Advertiser" };

                var shape = await _userDisplayManager.UpdateEditorAsync(user, updater: this, isNew: true);

                //if (!ModelState.IsValid)
                //{
                //    return View(shape);
                //}

                await _userService.CreateUserAsync(user, null, ModelState.AddModelError);

                //if (!ModelState.IsValid)
                //{
                //    return View(shape);
                //}

            }

            return View();
        }

        [HttpPost, ActionName("ImportBrands")]
        public async Task<ActionResult> ImportBrands()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            var products = await _migrationService.GetImportProductsData();

            //create product brands
            var brandsList = products.Where(x => x.Brand != "").Select(x => x.Brand).Distinct();
            foreach (var brand in brandsList)
            {
                var contentItem = await _contentManager.NewAsync("FarmMachineryBrand");

                await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                contentItem.DisplayText = brand;
                //contentItem.Content.TitlePart.Title = brand;
                contentItem.Content.AutoroutePart.Path = "machinerie-agricole/" + Slugify(brand);

                await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                // creates a published item
                await _contentManager.PublishAsync(contentItem);
            }

            return View();
        }

        [HttpPost, ActionName("ImportFarmMachinery")]
        public async Task<IActionResult> ImportFarmMachinery()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(FarmMachineryMainPicturePath);
            CleanFolder(FarmMachineryWebPicturePath);

            var products = await _migrationService.GetImportProductsData();

            var images = await _migrationService.GetImportImagesData();
            var farmMachineryBrands = await GetAllContentItemsFromTypeAsync("FarmMachineryBrand");
            var farmMachineryTypes = await GetAllContentItemsFromTypeAsync("FarmMachineryType");
            var farmMachineryCategories = await GetAllContentItemsFromTypeAsync("FarmMachineryCategory");
            var advertisers = await GetAllContentItemsFromTypeAsync("Advertiser");
            var regions = await GetAllContentItemsFromTypeAsync("Region");

            foreach (dynamic product in products)
            {
                if (product.IsMotorEquipment == "Oui")
                {
                    var contentItem = await _contentManager.NewAsync("FarmMachinery");

                    await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                    contentItem.Content.FarmMachinery.TypeOfFuel.Text = product.FuelType;
                    contentItem.Content.FarmMachinery.Model.Text = product.Model;
                    contentItem.Content.FarmMachinery.Year.Value = product.Year;
                    contentItem.Content.FarmMachinery.Hours.Value = product.Hours;
                    contentItem.Content.FarmMachinery.Condition.Value = product.Condition == "Neuf" ? true : false;
                    contentItem.Content.FarmMachinery.Sold.Value = product.Sold == "VRAI" ? true : false;

                    //if (product.Concessionnaire.ToString().Trim() == "Équipements de ferme usagés Jutras")
                    //{
                    //    var test = "asd";
                    //    test = test.ToString().Trim();
                    //}

                    //if we find an advertiser we assign it
                    var advertiser = advertisers.Where(x => x.DisplayText.ToString().Trim() == product.Concessionnaire.ToString().Trim()).FirstOrDefault();
                    if (advertiser != null)
                    {
                        contentItem.Content.FarmMachinery.Advertiser.ContentItemIds.Add(advertiser.ContentItemId);
                        contentItem.Owner = advertiser.Content.Advertiser.Email.Text;
                    }

                    //if we find a brand we assign it
                    if (farmMachineryBrands.Where(x => x.DisplayText == product.Brand).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachinery.Brand.ContentItemIds.Add(farmMachineryBrands.Where(x => x.DisplayText == product.Brand).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery type we assign it
                    if (farmMachineryTypes.Where(x => x.DisplayText.ToString().Trim() == product.Title1.ToString().Trim()).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachinery.Type.ContentItemIds.Add(farmMachineryTypes.Where(x => x.DisplayText.ToString().Trim() == product.Title1.ToString().Trim()).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery category we assign it #1
                    if (farmMachineryCategories.Where(x => x.DisplayText == product.Category).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachinery.Category.ContentItemIds.Add(farmMachineryCategories.Where(x => x.DisplayText == product.Category).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery category we assign it #2
                    if (farmMachineryCategories.Where(x => x.DisplayText == product.Category2).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachinery.Category.ContentItemIds.Add(farmMachineryCategories.Where(x => x.DisplayText == product.Category2).FirstOrDefault().ContentItemId);
                    }

                    //if we find a region we assign it
                    if (regions.Where(x => x.DisplayText == product.Region).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachinery.Region.ContentItemIds.Add(regions.Where(x => x.DisplayText == product.Region).FirstOrDefault().ContentItemId);
                    }

                    contentItem.Content.FarmMachinery.MotorHP.Value = product.MotorHP;
                    contentItem.Content.FarmMachinery.PTOForce.Value = product.ForcePTO;

                    //main picture
                    string imagePrincipale = product.PrincipalImage.ToString();
                    if (!String.IsNullOrEmpty(imagePrincipale))
                    {
                        if (System.IO.File.Exists(mainImportPicturePath + imagePrincipale.Split('/').Last()) && !System.IO.File.Exists(FarmMachineryMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower()))
                        {
                            System.IO.File.Copy(mainImportPicturePath + imagePrincipale.Split('/').Last(), FarmMachineryMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                            contentItem.Content.FarmMachinery.MainPicture.Paths.Add("machinerie agricole/principale/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        }
                        else {
                            Console.WriteLine("Could not import principal image {" + product.PrincipalImage.ToString() + "}: " + product.ImageUrl.ToString());
                        }
                    }

                    //website pictures
                    var websiteImages = images.Where(x => x.ProductId == product.ProductId).OrderBy(x => x.Order);
                    foreach (var image in websiteImages)
                    {
                        string imageSiteWeb = image.Image;
                        if (!String.IsNullOrEmpty(imageSiteWeb))
                        {
                            if (System.IO.File.Exists(webImportPicturePath + imageSiteWeb) && !System.IO.File.Exists(FarmMachineryWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower()))
                            {
                                System.IO.File.Copy(webImportPicturePath + imageSiteWeb, FarmMachineryWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                                contentItem.Content.FarmMachinery.WebsitePictures.Paths.Add("machinerie agricole/site web/" + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            }
                            else
                            {
                                Console.WriteLine("Could not import website image {" + image.Image.ToString() + "}: " + image.ImageUrl.ToString());
                            }
                        }
                    }

                    //contentItem.Content.FarmMachinery.YoutubeVideo.EmbeddedAddress = product.YoutubeLink;
                    contentItem.Content.FarmMachinery.YoutubeVideo.RawAddress = product.YoutubeLink;
                    contentItem.Content.FarmMachinery.AdditionalDescription.Text = product.Description;
                    //contentItem.Content.FarmMachinery.AdministratorNote.Text = product.TypeOfFuel;
                    //contentItem.Content.FarmMachinery.Price.Value = product.TypeOfFuel;

                    contentItem.Content.FarmMachinery.PromoTag.Text = product.PromoFlag;
                    contentItem.Content.FarmMachinery.InventoryNo.Text = product.InventoryNo;
                    contentItem.Content.FarmMachinery.WheelDrive.Text = product.WheelDrive;
                    contentItem.Content.FarmMachinery.DistanceTraveled.Value = product.Mileage;
                    contentItem.CreatedUtc = DateTime.Parse(product.CreationDate);

                    await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                    // creates a drafted item
                    //await _contentManager.CreateAsync(contentItem);

                    // creates a published item
                    await _contentManager.PublishAsync(contentItem);
                }
            }

            return View();
        }

        [HttpPost, ActionName("ImportFarmMachineryAccessory")]
        public async Task<IActionResult> ImportFarmMachineryAccessory()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(FarmMachineryAccessoryMainPicturePath);
            CleanFolder(FarmMachineryAccessoryWebPicturePath);

            var products = await _migrationService.GetImportProductsData();

            var images = await _migrationService.GetImportImagesData();
            var farmMachineryBrands = await GetAllContentItemsFromTypeAsync("FarmMachineryBrand");
            var farmMachineryCategories = await GetAllContentItemsFromTypeAsync("FarmMachineryCategory");
            var farmMachineryAccessoryTypes = await GetAllContentItemsFromTypeAsync("FarmMachineryAccessoryType");
            var advertisers = await GetAllContentItemsFromTypeAsync("Advertiser");
            var regions = await GetAllContentItemsFromTypeAsync("Region");


            foreach (dynamic product in products)
            {
                if (product.IsAccessory == "Oui")
                {
                    var contentItem = await _contentManager.NewAsync("FarmMachineryAccessory");

                    await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                    contentItem.Content.FarmMachineryAccessory.Model.Text = product.Model;
                    contentItem.Content.FarmMachineryAccessory.Year.Value = product.Year;
                    contentItem.Content.FarmMachineryAccessory.Condition.Value = product.Condition == "Neuf" ? true : false;
                    contentItem.Content.FarmMachineryAccessory.Sold.Value = product.Sold == "VRAI" ? true : false;

                    //if we find an advertiser we assign it
                    var advertiser = advertisers.Where(x => x.DisplayText.ToString().Trim() == product.Concessionnaire.ToString().Trim()).FirstOrDefault();
                    if (advertiser != null)
                    {
                        contentItem.Content.FarmMachineryAccessory.Advertiser.ContentItemIds.Add(advertiser.ContentItemId);
                        contentItem.Owner = advertiser.Content.Advertiser.Email.Text;
                    }

                    //if we find a brand we assign it
                    if (farmMachineryBrands.Where(x => x.DisplayText == product.Brand).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachineryAccessory.Brand.ContentItemIds.Add(farmMachineryBrands.Where(x => x.DisplayText == product.Brand).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery type we assign it
                    if (farmMachineryAccessoryTypes.Where(x => x.DisplayText.ToString().Trim() == product.Title1.ToString().Trim()).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachineryAccessory.Type.ContentItemIds.Add(farmMachineryAccessoryTypes.Where(x => x.DisplayText.ToString().Trim() == product.Title1.ToString().Trim()).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery category we assign it #1
                    if (farmMachineryCategories.Where(x => x.DisplayText == product.Category).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachineryAccessory.Category.ContentItemIds.Add(farmMachineryCategories.Where(x => x.DisplayText == product.Category).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery category we assign it #2
                    if (farmMachineryCategories.Where(x => x.DisplayText == product.Category2).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachineryAccessory.Category.ContentItemIds.Add(farmMachineryCategories.Where(x => x.DisplayText == product.Category2).FirstOrDefault().ContentItemId);
                    }
                    else
                    {
                        //TODO create machinery category if not found.
                    }

                    //contentItem.Content.FarmMachineryAccessory.MotorHP.Value = product.MotorHP;
                    contentItem.Content.FarmMachineryAccessory.PTOForce.Value = product.ForcePTO;

                    //main picture
                    string imagePrincipale = product.PrincipalImage.ToString();
                    if (!String.IsNullOrEmpty(imagePrincipale))
                    {
                        if (System.IO.File.Exists(mainImportPicturePath + imagePrincipale.Split('/').Last()) && !System.IO.File.Exists(FarmMachineryAccessoryMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower()))
                        {
                            System.IO.File.Copy(mainImportPicturePath + imagePrincipale.Split('/').Last(), FarmMachineryAccessoryMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                            contentItem.Content.FarmMachineryAccessory.MainPicture.Paths.Add("machinerie agricole (accessoire)/principale/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        }
                        else
                        {
                            Console.WriteLine("Could not import principal image {" + product.PrincipalImage.ToString() + "}: " + product.ImageUrl.ToString());
                        }
                    }

                    //website pictures
                    var websiteImages = images.Where(x => x.ProductId == product.ProductId).OrderBy(x => x.Order);
                    foreach (var image in websiteImages)
                    {
                        string imageSiteWeb = image.Image;
                        if (!String.IsNullOrEmpty(imageSiteWeb))
                        {
                            if (System.IO.File.Exists(webImportPicturePath + imageSiteWeb) && !System.IO.File.Exists(FarmMachineryAccessoryWebPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower()))
                            {
                                System.IO.File.Copy(webImportPicturePath + imageSiteWeb, FarmMachineryAccessoryWebPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                                contentItem.Content.FarmMachineryAccessory.WebsitePictures.Paths.Add("machinerie agricole (accessoire)/site web/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not import website image {" + image.Image.ToString() + "}: " + image.ImageUrl.ToString());
                        }
                    }

                    contentItem.Content.FarmMachineryAccessory.YoutubeVideo.RawAddress = product.YoutubeLink;
                    contentItem.Content.FarmMachineryAccessory.AdditionalDescription.Text = product.Description;

                    //if we find a region we assign it
                    if (regions.Where(x => x.DisplayText == product.Region).FirstOrDefault() != null)
                    {
                        contentItem.Content.FarmMachineryAccessory.Region.ContentItemIds.Add(regions.Where(x => x.DisplayText == product.Region).FirstOrDefault().ContentItemId);
                    }

                    contentItem.Content.FarmMachineryAccessory.PromoTag.Text = product.PromoFlag;
                    contentItem.Content.FarmMachineryAccessory.InventoryNo.Text = product.InventoryNo;
                    contentItem.Content.FarmMachineryAccessory.WheelDrive.Text = product.WheelDrive;
                    contentItem.CreatedUtc = DateTime.Parse(product.CreationDate);

                    await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                    // creates a drafted item
                    //await _contentManager.CreateAsync(contentItem);

                    // creates a published item
                    await _contentManager.PublishAsync(contentItem);
                }
            }

            return View();
        }

        [HttpPost, ActionName("CleanOtherEquipment")]
        public async Task<IActionResult> CleanOtherEquipment()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(OtherEquipmentMainPicturePath);
            CleanFolder(OtherEquipmentWebPicturePath);

            return View();
        }

        [HttpPost, ActionName("ImportOtherEquipement")]
        public async Task<IActionResult> ImportOtherEquipement(string id)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }



            var products = await _migrationService.GetImportProductsData(id);

            var images = await _migrationService.GetImportImagesData();
            var farmMachineryBrands = await GetAllContentItemsFromTypeAsync("FarmMachineryBrand");
            var farmMachineryTypes = await GetAllContentItemsFromTypeAsync("FarmMachineryType");
            var farmMachineryCategories = await GetAllContentItemsFromTypeAsync("FarmMachineryCategory");
            var otherEquipmentTypes = await GetAllContentItemsFromTypeAsync("OtherEquipmentType");
            var advertisers = await GetAllContentItemsFromTypeAsync("Advertiser");
            var regions = await GetAllContentItemsFromTypeAsync("Region");


            foreach (dynamic product in products)
            {
                if (product.IsOtherEquipment == "Oui")
                {
                    var contentItem = await _contentManager.NewAsync("OtherEquipment");

                    await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                    contentItem.Content.OtherEquipment.Model.Text = product.Model;
                    contentItem.Content.OtherEquipment.Condition.Value = product.Condition == "Neuf" ? true : false;
                    contentItem.Content.OtherEquipment.Sold.Value = product.Sold == "VRAI" ? true : false;

                    //if we find an advertiser we assign it
                    var advertiser = advertisers.Where(x => x.DisplayText.ToString().Trim() == product.Concessionnaire.ToString().Trim()).FirstOrDefault();
                    if (advertiser != null)
                    {
                        contentItem.Content.OtherEquipment.Advertiser.ContentItemIds.Add(advertiser.ContentItemId);
                        contentItem.Owner = advertiser.Content.Advertiser.Email.Text;
                    }

                    //if we find a machinery type we assign it
                    if (otherEquipmentTypes.Where(x => x.DisplayText.ToString().Trim() == product.Title1.ToString().Trim()).FirstOrDefault() != null)
                    {
                        contentItem.Content.OtherEquipment.Type.ContentItemIds.Add(otherEquipmentTypes.Where(x => x.DisplayText.ToString().Trim() == product.Title1.ToString().Trim()).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery category we assign it #1
                    if (farmMachineryCategories.Where(x => x.DisplayText == product.Category).FirstOrDefault() != null)
                    {
                        contentItem.Content.OtherEquipment.Category.ContentItemIds.Add(farmMachineryCategories.Where(x => x.DisplayText == product.Category).FirstOrDefault().ContentItemId);
                    }

                    //if we find a machinery category we assign it #2
                    if (farmMachineryCategories.Where(x => x.DisplayText == product.Category2).FirstOrDefault() != null)
                    {
                        contentItem.Content.OtherEquipment.Category.ContentItemIds.Add(farmMachineryCategories.Where(x => x.DisplayText == product.Category2).FirstOrDefault().ContentItemId);
                    }

                    //if we find a brand we assign it
                    if (farmMachineryBrands.Where(x => x.DisplayText == product.Brand).FirstOrDefault() != null)
                    {
                        contentItem.Content.OtherEquipment.Brand.ContentItemIds.Add(farmMachineryBrands.Where(x => x.DisplayText == product.Brand).FirstOrDefault().ContentItemId);
                    }

                    //main picture
                    string imagePrincipale = product.PrincipalImage.ToString();
                    if (!String.IsNullOrEmpty(imagePrincipale))
                    {
                        if (System.IO.File.Exists(mainImportPicturePath + imagePrincipale.Split('/').Last()) && !System.IO.File.Exists(OtherEquipmentMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower()))
                        {
                            System.IO.File.Copy(mainImportPicturePath + imagePrincipale.Split('/').Last(), OtherEquipmentMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                            contentItem.Content.OtherEquipment.MainPicture.Paths.Add("autre équipement/principale/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        }
                        else
                        {
                            Console.WriteLine("Could not import principal image {" + product.PrincipalImage.ToString() + "}: " + product.ImageUrl.ToString());
                        }
                    }

                    //website pictures
                    var websiteImages = images.Where(x => x.ProductId == product.ProductId).OrderBy(x => x.Order);
                    foreach (var image in websiteImages)
                    {
                        string imageSiteWeb = image.Image;
                        if (!String.IsNullOrEmpty(imageSiteWeb))
                        {
                            if (System.IO.File.Exists(webImportPicturePath + imageSiteWeb) && !System.IO.File.Exists(OtherEquipmentWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower()))
                            {
                                System.IO.File.Copy(webImportPicturePath + imageSiteWeb, OtherEquipmentWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                                contentItem.Content.OtherEquipment.WebsitePictures.Paths.Add("autre équipement/site web/" + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not import website image {" + image.Image.ToString() + "}: " + image.ImageUrl.ToString());
                        }
                    }

                    //contentItem.Content.OtherEquipment.YoutubeVideo.EmbeddedAddress = product.YoutubeLink;
                    contentItem.Content.OtherEquipment.YoutubeVideo.RawAddress = product.YoutubeLink;
                    contentItem.Content.OtherEquipment.AdditionalDescription.Text = product.Description;

                    //if we find a region we assign it
                    if (regions.Where(x => x.DisplayText == product.Region).FirstOrDefault() != null)
                    {
                        contentItem.Content.OtherEquipment.Region.ContentItemIds.Add(regions.Where(x => x.DisplayText == product.Region).FirstOrDefault().ContentItemId);
                    }

                    contentItem.Content.OtherEquipment.PromoTag.Text = product.PromoFlag;
                    contentItem.Content.OtherEquipment.InventoryNo.Text = product.InventoryNo;
                    contentItem.Content.OtherEquipment.Year.Value = product.Year;
                    contentItem.CreatedUtc = DateTime.Parse(product.CreationDate);

                    await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                    // creates a drafted item
                    //await _contentManager.CreateAsync(contentItem);

                    // creates a published item
                    await _contentManager.PublishAsync(contentItem);
                }
            }

            return View(new OtherEquipmentViewModel { Id = id });
        }

        [HttpPost, ActionName("ImportRealEstate")]
        public async Task<IActionResult> ImportRealEstate()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(RealEstateMainPicturePath);
            CleanFolder(RealEstateWebPicturePath);

            var products = await _migrationService.GetImportProductsData();

            var images = await _migrationService.GetImportImagesData();
            var advertisers = await GetAllContentItemsFromTypeAsync("Advertiser");
            var realEstateTypes = await GetAllContentItemsFromTypeAsync("RealEstateType");
            var realEstateActivityTypes = await GetAllContentItemsFromTypeAsync("RealEstateActivityType");
            var realEstateStatus = await GetAllContentItemsFromTypeAsync("RealEstateStatus");
            var regions = await GetAllContentItemsFromTypeAsync("Region");

            foreach (dynamic product in products)
            {
                if (product.IsRealEstate == "Oui")
                {
                    var contentItem = await _contentManager.NewAsync("RealEstate");

                    await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                    //main picture
                    string imagePrincipale = product.PrincipalImage.ToString();
                    if (!String.IsNullOrEmpty(imagePrincipale))
                    {
                        if (System.IO.File.Exists(mainImportPicturePath + imagePrincipale.Split('/').Last()) && !System.IO.File.Exists(RealEstateMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower()))
                        {
                            System.IO.File.Copy(mainImportPicturePath + imagePrincipale.Split('/').Last(), RealEstateMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                            contentItem.Content.RealEstate.MainPicture.Paths.Add("immobilier/principale/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        }
                        else
                        {
                            Console.WriteLine("Could not import principal image {" + product.PrincipalImage.ToString() + "}: " + product.ImageUrl.ToString());
                        }
                    }

                    //website pictures
                    var websiteImages = images.Where(x => x.ProductId == product.ProductId).OrderBy(x => x.Order);
                    foreach (var image in websiteImages)
                    {
                        string imageSiteWeb = image.Image;
                        if (!String.IsNullOrEmpty(imageSiteWeb))
                        {
                            if (System.IO.File.Exists(webImportPicturePath + imageSiteWeb) && !System.IO.File.Exists(RealEstateWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower()))
                            {
                                System.IO.File.Copy(webImportPicturePath + imageSiteWeb, RealEstateWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                                contentItem.Content.RealEstate.WebsitePictures.Paths.Add("immobilier/site web/" + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not import website image {" + image.Image.ToString() + "}: " + image.ImageUrl.ToString());
                        }
                    }

                    //if we find an advertiser we assign it
                    var advertiser = advertisers.Where(x => x.DisplayText.ToString().Trim() == product.Concessionnaire.ToString().Trim()).FirstOrDefault();
                    if (advertiser != null)
                    {
                        contentItem.Content.RealEstate.Advertiser.ContentItemIds.Add(advertiser.ContentItemId);
                        contentItem.Owner = advertiser.Content.Advertiser.Email.Text;
                    }

                    //if we find a RealEstateType we assign it
                    var realEstateType = realEstateTypes.Where(x => x.DisplayText.ToString().Trim() == product.RealEstateType.ToString().Trim()).FirstOrDefault();
                    if (realEstateType != null)
                    {
                        contentItem.Content.RealEstate.Type.ContentItemIds.Add(realEstateType.ContentItemId);
                    }

                    //if we find a RealEstateActivityType we assign it
                    var realEstateActivityType = realEstateActivityTypes.Where(x => x.DisplayText.ToString().Trim() == product.ActivityType.ToString().Trim()).FirstOrDefault();
                    if (realEstateActivityType != null)
                    {
                        contentItem.Content.RealEstate.ActivityType.ContentItemIds.Add(realEstateActivityType.ContentItemId);
                    }

                    contentItem.Content.RealEstate.Type.Text = product.RealEstateType;
                    contentItem.Content.RealEstate.ActivityType.Text = product.ActivityType;

                    contentItem.Content.RealEstate.YoutubeVideo.RawAddress = product.YoutubeLink;
                    //contentItem.Content.RealEstate.Price = "";
                    contentItem.Content.RealEstate.Sold.Value = product.Sold == "VRAI" ? true : false;

                    //if we find a region we assign it
                    if (regions.Where(x => x.DisplayText == product.Region).FirstOrDefault() != null)
                    {
                        contentItem.Content.RealEstate.Region.ContentItemIds.Add(regions.Where(x => x.DisplayText == product.Region).FirstOrDefault().ContentItemId);
                    }

                    //if we find a status we assign it
                    if (realEstateStatus.Where(x => x.DisplayText == product.RealEstateStatus).FirstOrDefault() != null)
                    {
                        contentItem.Content.RealEstate.Status.ContentItemIds.Add(realEstateStatus.Where(x => x.DisplayText == product.RealEstateStatus).FirstOrDefault().ContentItemId);
                    }

                    contentItem.Content.RealEstate.NumberOfRooms.Value = product.NumberOfRooms;
                    contentItem.Content.RealEstate.Area.Value = product.Area;
                    contentItem.CreatedUtc = DateTime.Parse(product.CreationDate);

                    await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                    // creates a drafted item
                    //await _contentManager.CreateAsync(contentItem);

                    // creates a published item
                    await _contentManager.PublishAsync(contentItem);
                }
            }

            return View();
        }

        [HttpPost, ActionName("ImportClassifiedAd")]
        public async Task<IActionResult> ImportClassifiedAd()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(ClassifiedAdMainPicturePath);
            CleanFolder(ClassifiedAdWebPicturePath);

            var products = await _migrationService.GetImportProductsData();
            var images = await _migrationService.GetImportImagesData();
            //var advertisers = await GetAllContentItemsFromTypeAsync("Advertiser");
            var regions = await GetAllContentItemsFromTypeAsync("Region");


            foreach (dynamic product in products)
            {
                if (product.IsClassifiedAd == "Oui")
                {
                    var contentItem = await _contentManager.NewAsync("ClassifiedAd");

                    await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                    //contentItem.Content.ClassifiedAd.Condition.Value = product.Condition == "Neuf" ? true : false;
                    contentItem.Content.ClassifiedAd.Sold.Value = product.Sold == "VRAI" ? true : false;

                    //main picture
                    string imagePrincipale = product.PrincipalImage.ToString();

                    if (!String.IsNullOrEmpty(imagePrincipale))
                    {
                        contentItem.Content.ClassifiedAd.MainPicture.Paths.Add("annonce classée/principale/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        if (System.IO.File.Exists(mainImportPicturePath + imagePrincipale.Split('/').Last()) && !System.IO.File.Exists(ClassifiedAdMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower()))
                        {
                            System.IO.File.Copy(mainImportPicturePath + imagePrincipale.Split('/').Last(), ClassifiedAdMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        }
                        else
                        {
                            Console.WriteLine("Could not import principal image {" + product.PrincipalImage.ToString() + "}: " + product.ImageUrl.ToString());
                        }
                    }

                    //website pictures
                    var websiteImages = images.Where(x => x.ProductId == product.ProductId).OrderBy(x => x.Order);
                    foreach (var image in websiteImages)
                    {
                        string imageSiteWeb = image.Image;
                        if (!String.IsNullOrEmpty(imageSiteWeb))
                        {
                            contentItem.Content.ClassifiedAd.WebsitePictures.Paths.Add("annonce classée/site web/" + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            if (System.IO.File.Exists(webImportPicturePath + imageSiteWeb) && !System.IO.File.Exists(ClassifiedAdWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower()))
                            {
                                System.IO.File.Copy(webImportPicturePath + imageSiteWeb, ClassifiedAdWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            }
                            else
                            {
                                Console.WriteLine("Could not import website image {" + image.Image.ToString() + "}: " + image.ImageUrl.ToString());
                            }
                        }
                    }

                    //if we find a region we assign it
                    if (regions.Where(x => x.DisplayText == product.Region).FirstOrDefault() != null)
                    {
                        contentItem.Content.ClassifiedAd.Region.ContentItemIds.Add(regions.Where(x => x.DisplayText == product.Region).FirstOrDefault().ContentItemId);
                    }

                    contentItem.Content.ClassifiedAd.Description.Text = product.Description;
                    contentItem.Content.ClassifiedAd.PostalCode.Text = product.ZipCode;
                    contentItem.Content.ClassifiedAd.Town.Text = product.TownAds;
                    contentItem.Content.ClassifiedAd.Company.Text = product.Concessionnaire;
                    contentItem.Content.ClassifiedAd.ContactPerson.Text = product.ResourcePerson;
                    contentItem.Content.ClassifiedAd.Email.Text = product.AdEmailContact;
                    contentItem.DisplayText = product.Name.ToString().Trim();
                    //contentItem.Content.TitlePart.Title = product.Name.ToString().Trim();
                    contentItem.CreatedUtc = DateTime.Parse(product.CreationDate);

                    await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                    // creates a drafted item
                    //await _contentManager.CreateAsync(contentItem);

                    // creates a published item
                    await _contentManager.PublishAsync(contentItem);
                }
            }

            return View();
        }

        [HttpPost, ActionName("ImportService")]
        public async Task<IActionResult> ImportService()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            CleanFolder(ServiceMainPicturePath);
            CleanFolder(ServiceWebPicturePath);

            var products = await _migrationService.GetImportProductsData();
            var images = await _migrationService.GetImportImagesData();
            var advertisers = await GetAllContentItemsFromTypeAsync("Advertiser");
            var regions = await GetAllContentItemsFromTypeAsync("Region");
            var taxonomies = await GetAllContentItemsFromTypeAsync("Taxonomy");
            var taxonomy = taxonomies.Where(x => x.DisplayText == "Catégories de service").FirstOrDefault();

            foreach (dynamic product in products)
            {
                if (product.IsService == "Oui")
                {
                    var contentItem = await _contentManager.NewAsync("Service");

                    await _contentItemDisplayManager.UpdateEditorAsync(contentItem, this, true);

                    //main picture
                    string imagePrincipale = product.PrincipalImage.ToString();
                    if (!String.IsNullOrEmpty(imagePrincipale))
                    {
                        if (System.IO.File.Exists(mainImportPicturePath + imagePrincipale.Split('/').Last()) && !System.IO.File.Exists(ServiceMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower()))
                        {
                            System.IO.File.Copy(mainImportPicturePath + imagePrincipale.Split('/').Last(), ServiceMainPicturePath + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                            contentItem.Content.Service.MainPicture.Paths.Add("service/principale/" + contentItem.ContentItemId + System.IO.Path.GetExtension(mainImportPicturePath + imagePrincipale.Split('/').Last()).ToLower());
                        }
                        else
                        {
                            Console.WriteLine("Could not import principal image {" + product.PrincipalImage.ToString() + "}: " + product.ImageUrl.ToString());
                        }
                    }

                    //website pictures
                    var websiteImages = images.Where(x => x.ProductId == product.ProductId).OrderBy(x => x.Order);
                    foreach (var image in websiteImages)
                    {
                        string imageSiteWeb = image.Image;
                        if (!String.IsNullOrEmpty(imageSiteWeb))
                        {
                            if (System.IO.File.Exists(webImportPicturePath + imageSiteWeb) && !System.IO.File.Exists(ServiceWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower()))
                            {
                                System.IO.File.Copy(webImportPicturePath + imageSiteWeb, ServiceWebPicturePath + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                                contentItem.Content.Service.WebsitePictures.Paths.Add("service/site web/" + contentItem.ContentItemId + '-' + image.LibraryItemId + System.IO.Path.GetExtension(mainImportPicturePath + imageSiteWeb).ToLower());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not import website image {" + image.Image.ToString() + "}: " + image.ImageUrl.ToString());
                        }
                    }

                    //if we find a region we assign it
                    if (regions.Where(x => x.DisplayText == product.Region).FirstOrDefault() != null)
                    {
                        contentItem.Content.Service.Region.ContentItemIds.Add(regions.Where(x => x.DisplayText == product.Region).FirstOrDefault().ContentItemId);
                    }

                    contentItem.Content.Service.YoutubeVideo.RawAddress = product.YoutubeLink;
                    contentItem.Content.Service.AdditionalDescription.Text = product.Description;

                    var taxonomyId = "";
                    foreach (var item in taxonomy.Content.TaxonomyPart.Terms)
                    {
                        if (item.DisplayText == product.ServiceCategory.ToString().Trim())
                        {
                            taxonomyId = item.ContentItemId;
                        }

                        if (item.Terms != null)
                        {
                            foreach (var subitem in item.Terms)
                            {
                                if (subitem.DisplayText == product.ServiceSubCategory.ToString().Trim())
                                {
                                    taxonomyId = subitem.ContentItemId;
                                }
                            }
                        }
                    }

                    if (taxonomyId != "")
                    {
                        contentItem.Content.Service.Category.TermContentItemIds.Add(taxonomyId);
                    }

                    //contentItem.Content.Service.Category.Text = product.ServiceCategory.ToString().Trim();
                    //contentItem.Content.Service.SubCategory.Text = product.ServiceSubCategory.ToString().Trim();
                    contentItem.DisplayText = product.Name.ToString().Trim();
                    //contentItem.Content.TitlePart.Title = product.Name.ToString().Trim();
                    contentItem.CreatedUtc = DateTime.Parse(product.CreationDate);

                    await _contentManager.CreateAsync(contentItem, VersionOptions.DraftRequired);

                    // creates a drafted item
                    //await _contentManager.CreateAsync(contentItem);

                    // creates a published item
                    await _contentManager.PublishAsync(contentItem);
                }
            }

            return View();
        }

        private static string urlDecode(string url)
        {
            var result = url;
            result = result.Replace("%20", " ");

            return result;
        }
    }
}
