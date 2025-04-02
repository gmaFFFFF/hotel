namespace gmafffff.starterKit.Messaging.Crud;

public abstract record UpdateBusinessCommand<TId, TDto>(TId Id, TDto Changed) : BusinessCommand
    where TId : struct, IEquatable<TId>;