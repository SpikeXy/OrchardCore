using System.Collections.Generic;
using OrchardCore.Security.Permissions;

namespace AffairesExtra.Migration
{
    // todo
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageAffairesExtraMigration = new Permission("ManageAffairesExtraMigration", "Manage the Affaires Extra migration");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] { ManageAffairesExtraMigration };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new[] { ManageAffairesExtraMigration }
                }
            };
        }
    }
}