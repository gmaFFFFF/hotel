namespace gmafffff.starterKit.Messaging.Crud;

public abstract record CreateBusinessCommand<TDto>(TDto New) : BusinessCommand;