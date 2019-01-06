using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffairesExtra.Migration.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly MigrationService _migrationService;
        private readonly IContentManager _contentManager;

        public AdminController(
            MigrationService migrationService,
            IAuthorizationService authorizationService,
            IContentManager contentManager)
        {
            _migrationService = migrationService;
            _authorizationService = authorizationService;
            _contentManager = contentManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpPost, ActionName("Migrate")]
        public async Task<IActionResult> Migrate()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAffairesExtraMigration))
            {
                return Unauthorized();
            }

            var documents = await _migrationService.Migrate();

            foreach (dynamic product in documents)
            {

                var test = product.ProductId;

                var contentItem = await _contentManager.NewAsync("FarmMachinery");


            }

            return View();
        }
    }
}
