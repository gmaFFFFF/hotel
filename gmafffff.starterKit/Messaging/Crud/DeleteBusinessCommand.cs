namespace gmafffff.starterKit.Messaging.Crud;

public abstract record DeleteBusinessCommand<TId>(params TId[] Ids) : BusinessCommand
    where TId : struct, IEquatable<TId>;