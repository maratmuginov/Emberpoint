using static Emberpoint.Core.GameObjects.Map.EmberCell;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface ILightable
    {
        LightEngineProperties LightProperties { get; set; }
    }
}
