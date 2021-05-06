using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrchardCore.Forms.Services;
using OrchardCore.Forms.Services.Models;

namespace OrchardCore.Forms.Providers
{
    public class ContainsProvider : IValidationRuleProvider
    {
        public int Index => 1;
        public string DisplayName => "Contains";
        public string Name => "Contains";
        public bool IsShowOption => true;
        public string OptionPlaceHolder => "string to compare with input";
        public bool IsShowErrorMessage => true;

        public Task<bool> ValidateInputByRuleAsync(ValidationRuleInput model)
        {
            return Task.FromResult(model.Input.Contains(model.Option, StringComparison.InvariantCulture));
        }
    }
}
