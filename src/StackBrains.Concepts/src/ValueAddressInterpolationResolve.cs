using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ResolverState = FrogStack.Brightpearl.State<string, string>;
using Result = StackBrains.Essentials.Result<string, string>;
using StackBrains.Essentials;
using Newtonsoft.Json.Linq;

namespace FrogStack.Brightpearl
{
    public class ValueAddressInterpolationResolve
    {
        private readonly IValueProvider<EntityAddress, Task<JObject>> provider;

        public ValueAddressInterpolationResolve(IValueProvider<EntityAddress, Task<JObject>> provider)
        {
            this.provider = provider;
        }

        private async Task<Result<string, string>> TryResolveAsync(
            string input,
            ValueAddress address
        )
        {
            try
            {
                var entity = await provider
                    .Get(address.EntityAddress);

                if (entity == null)
                    return new Result(error: $"Entity not found: {address.EntityAddress}");

                string GetEntityPropertyValue(IEnumerable<string> path)
                {
                    var _path = string.Join(".", path);
                    var value = entity.SelectToken(_path);
                    if (value == null) throw new Exception($"No value at path: {_path}");
                    return value.ToString();
                }

                var output = input.Replace(
                    oldValue: address.ToInterpolationString(),
                    newValue: GetEntityPropertyValue(address.PropertyPath),
                    comparisonType: StringComparison.OrdinalIgnoreCase
                );

                return new Result(ok: output);
            }
            catch (Exception ex)
            {
                return new Result(error: ex.Message);
            }
        }

        public Task<ResolverState> TryResolveAsync(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException($"'{nameof(input)}' cannot be null or empty.", nameof(input));

            ResolverState NextState(ResolverState currentState, Result result) =>
                result.Map(
                    value => currentState.WithValue(value),
                    error => currentState.WithErrors(error)
                );

            Task<ResolverState> ResolveNextAsync(ResolverState state, ValueAddress address) =>
                TryResolveAsync(state.Value, address).MapAsync(res => NextState(state, res));

            return ValueAddressInterpolation
                .Search(input)
                .AggregateAsync(
                    seed: new ResolverState(input),
                    reducer: ResolveNextAsync
                );
        }
    }
}