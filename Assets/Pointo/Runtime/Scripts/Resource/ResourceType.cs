using System;

namespace Pointo.Resource
{
    /// <summary>
    /// Resource Type makes it better to make different behaviours for each <see cref="Pointo.Resource.Resource"/>
    /// script. One can add/remove more types depending on your game.
    /// </summary>
    [Serializable]
    public enum ResourceType
    {
        Wood,
        Fish,
        Coal,
        Iron,
        Gold
    }
}