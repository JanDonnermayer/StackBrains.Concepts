using System;

namespace FrogStack.Brightpearl
{
    public class EntityAddress
    {
        public EntityAddress(
            string entityType,
            string entityId
        )
        {
            if (string.IsNullOrEmpty(entityType))
                throw new ArgumentException($"'{nameof(entityType)}' cannot be null or empty.", nameof(entityType));

            if (string.IsNullOrEmpty(entityId))
                throw new ArgumentException($"'{nameof(entityId)}' cannot be null or empty.", nameof(entityId));

            EntityType = entityType;
            EntityId = entityId;
        }

        public string EntityType { get; }

        public string EntityId { get; }


        public override bool Equals(object? obj)
        {
            return obj is EntityAddress address &&
                EntityType == address.EntityType &&
                EntityId == address.EntityId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EntityType, EntityId);
        }
    }
}