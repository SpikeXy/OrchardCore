using OrchardCore.ContentManagement;

namespace OrchardCore.Forms.Models
{
    public class ValidationRulePart : ContentPart
    {
        public string ContentItemId { get; set; }
        public string Type { get; set; }
        public string Option { get; set; }
        public string ErrorMessage { get; set; }
    }
}
