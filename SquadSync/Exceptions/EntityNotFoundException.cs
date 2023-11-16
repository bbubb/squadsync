﻿using System;
using System.Runtime.Serialization;

namespace SquadSync.Exceptions
{
    // Custom generic exception for when an entity is not found in the database.
    // Example usage: throw new EntityNotFoundException("UserRepository", nameof(User), user.Guid);
    // Results in the following exception message: "[UserRepository] Entity of type 'User' with key '00000000-0000-0000-0000-000000000000' was not found."

    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public string Context { get; }
        public string EntityType { get; }
        public object EntityKey { get; }
        public string Context { get; }

        // EntityNotFoundException("UserRepository", nameof(User), user.Guid); 
        // Includes EntityKey
        public EntityNotFoundException(string context, string entityType, object entityKey)
            : base($"[{context}] Entity of type '{entityType}' with key '{entityKey}' was not found.")
        {
            Context = context;
            EntityType = entityType;
            EntityKey = entityKey;
        }

        // EntityNotFoundException("UserRepository", nameof(User));
        // Does not include EntityKey, which could be sensitive information
        public EntityNotFoundException(string context, string entityType)
            : base($"[{context}] Entity of type '{entityType}' was not found.")
        {
            Context = context;
            EntityType = entityType;
        }

        // EntityNotFoundException("UserRepository", nameof(User), user.Guid, innerException);
        // Includes EntityKey and innerException
        public EntityNotFoundException(string context, string entityType, object entityKey, Exception innerException)
            : base($"[{context}] Entity of type '{entityType}' with key '{entityKey}' was not found.", innerException)
        {
            Context = context;
            EntityType = entityType;
            EntityKey = entityKey;
        }

        // EntityNotFoundException("UserRepository", nameof(User), innerException);
        // Does not include EntityKey, which could be sensitive information
        public EntityNotFoundException(string context, string entityType, Exception innerException)
            : base($"[{context}] Entity of type '{entityType}' was not found.", innerException)
        {
            Context = context;
            EntityType = entityType;
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Context = info.GetString("Context");
            EntityType = info.GetString("EntityType");
            EntityKey = info.GetValue("EntityKey", typeof(object));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Context", Context);
            info.AddValue("EntityType", EntityType);
            info.AddValue("EntityKey", EntityKey);
        }
    }
}