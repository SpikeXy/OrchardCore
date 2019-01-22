using Fluid;
using Fluid.Values;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Liquid.Filters
{
    public class ToInt32Filter : ILiquidFilter
    {
        public Task<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var text = input.ToStringValue();

            return Task.FromResult<FluidValue>(new NumberValue(Int32.Parse(text)));
        }
    }
}
