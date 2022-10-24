namespace ThreeThings.Utils.Common;

public sealed record VoidObject
{
    private static readonly Lazy<VoidObject> Lazy = new();

    public static VoidObject Instance => Lazy.Value;

    private VoidObject() { }
}
