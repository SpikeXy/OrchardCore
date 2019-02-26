
using OrchardCore.ContentManagement;

namespace OrchardCore.ContentLocalization.Model
{
    public class LocalizationPart : ContentPart
    {
        public string LocalizationSet { get; set; }

        public string Culture { get; set; }
    }
}
