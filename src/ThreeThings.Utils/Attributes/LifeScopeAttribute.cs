using ThreeThings.Utils.Enums;

namespace ThreeThings.Utils.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class LifeScopeAttribute : Attribute
{
    public LifeScopeAttribute(LifeScope scope)
    {
        Scope = scope;
    }

    public LifeScope Scope { get; set; }
}
