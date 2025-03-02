namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

[Flags]
public enum DbHandlerStatus {
    None = 0,
    Load = 2 << 0,
    Action = 2 << 1,
    BeforeSaving = 2 << 2,
    Save = 2 << 3,
    Pack = 2 << 4,
    All = Load | Action | BeforeSaving | Save | Pack,
    LoadFail = Load,
    ActionFail = Load | Action,
    WithoutSave = Load | Action | Pack
}