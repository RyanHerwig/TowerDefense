public class ElementEffect
{
    public ElementName ElementName;
    public float Duration;

    public ElementEffect(ElementName effectName, float duration)
    {
        ElementName = effectName;
        Duration = duration;
    }
}

public struct ElementEffectData
{
    public ElementEffect Element;
    public Enemy Target;
    public TowerDamage Origin;

    public ElementEffectData(ElementEffect element, Enemy target, TowerDamage origin)
    {
        Element = element;
        Target = target;
        Origin = origin;
    }
}