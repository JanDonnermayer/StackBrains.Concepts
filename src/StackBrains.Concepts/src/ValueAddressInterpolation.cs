using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FrogStack.Brightpearl
{
    public static class ValueAddressInterpolation
    {
        private const string interpolationPattern = @"\${(?<entityType>.*?)\[(?<entityId>.*?)\](?<propertyPath>.*?)}";

        public static IEnumerable<ValueAddress> Search(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException($"'{nameof(input)}' cannot be null or empty.", nameof(input));

            static ValueAddress? GetValueAddress(GroupCollection groups)
            {
                var success = groups.TryGetValue("entityType", out var entityType);
                    success &= groups.TryGetValue("entityId", out var entityId);
                    success &= groups.TryGetValue("propertyPath", out var propertyPath);

                if (!success) return null;

                var entityAddress = new EntityAddress(
                    entityType: entityType!.Value,
                    entityId: entityId!.Value
                );

                return new ValueAddress(
                    entityAddress: entityAddress,
                    propertyPath: propertyPath!.Value.Split(".")
                        .Where(s => !string.IsNullOrEmpty(s)).ToArray()
                );
            }

            return new Regex(interpolationPattern)
                .Matches(input)
                .Select(m => GetValueAddress(m.Groups))
                .Choose();
        }

        public static string ToInterpolationString(this ValueAddress location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            string propertyPath = string.Join(".", location.PropertyPath.Prepend(""));
            return $"${{{location.EntityAddress.EntityType}[{location.EntityAddress.EntityId}]{propertyPath}}}";
        }
    }
}