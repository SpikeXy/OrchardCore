using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace AffairesExtra.Migration
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            // Configuration and settings menus for the AdminTree module
            builder.Add(T["Configuration"], cfg => cfg
                    .Add(T["Affaires Extra Migration"], "1.5", admt => admt
                        .Permission(Permissions.ManageAffairesExtraMigration)
                        .Action("Index", "Admin", new { area = "AffairesExtra.Migration" })
                        .LocalNav()
                    ));

            return Task.CompletedTask;
        }
    }
}
