using System;
using System.Runtime.Serialization;

namespace SquadSync.Exceptions
{
    // Custom generic exception for when an entity is not found in the database.
    // Example usage: throw new EntityNotFoundException(nameof(User), user.Guid);
    // Results in the following exception message: "Entity of type 'User' with key '00000000-0000-0000-0000-000000000000' was not found."
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public string EntityType { get; }
        public object EntityKey { get; }

        public EntityNotFoundException(string entityType, object entityKey)
            : base($"Entity of type '{entityType}' with key '{entityKey}' was not found.")
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }

        public EntityNotFoundException(string entityType, object entityKey, Exception innerException)
            : base($"Entity of type '{entityType}' with key '{entityKey}' was not found.", innerException)
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EntityType = info.GetString("EntityType");
            EntityKey = info.GetValue("EntityKey", typeof(object));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EntityType", EntityType);
            info.AddValue("EntityKey", EntityKey);
        }
    }
}
