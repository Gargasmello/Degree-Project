using System;

namespace Pointo.Unit
{
    /// <summary>
    /// Unit Race Type makes it better to make different behaviours for each <see cref="Pointo.Unit.Unit"/>
    /// script. One can add/remove more types depending on your game.
    /// </summary>
    [Serializable]
    public enum UnitRaceType
    {
        None,
        Human,
        Orc,
        Elf,
        Wizards
    }
    
    /// <summary>
    /// Unit Type makes it better to make different behaviours for each <see cref="Pointo.Unit.Unit"/>
    /// script. One can add/remove more types depending on your game.
    /// </summary>
    [Serializable]
    public enum UnitType
    {
        Peon,
        Knight,
        Archer,
        Catapult
    }
}