

using System;
using System.Collections.Generic;

namespace FrogStack.Brightpearl
{
    public class ValueAddress
    {
        public ValueAddress(
            EntityAddress entityAddress,
            params string[] propertyPath
        )
        {
            EntityAddress = entityAddress ?? throw new ArgumentNullException(nameof(entityAddress));
            PropertyPath = propertyPath ?? throw new ArgumentNullException(nameof(propertyPath));
        }

        public EntityAddress EntityAddress { get; }

        public IEnumerable<string> PropertyPath { get; }
    }
}