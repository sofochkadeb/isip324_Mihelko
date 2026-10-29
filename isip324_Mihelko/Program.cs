using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    enum EventType
    {
        Chest, //сундук
        Enemy, //враг
        Boss //босс
    }
    enum EnemyType
    {
        Goblin, //гоблин
        Skeleton, //скелет
        Mage, //маг
        VVG, //ВВГ (гоблин)
        Kovalsky, //Ковальский (скелет)
        ArchimageCpp, //архимаг С++ (маг)
        PestovC //пестов С-- (скелет)
    }
    enum ItemType
    {
        Potion, //лечебное зелье
        Weapon, //оружие
        Armor //доспех
    }
    enum PlayerAction
    {
        Attack, //атака
        Defend //защита
    }
    internal class Program
    {
        public abstract class Item
        {
            public string Name { get; protected set; }
            public ItemType ItemType { get; protected set; }
            protected Item(string name, ItemType itemType)
            {
                Name = name;
                ItemType = itemType;
            }
            public override string ToString()
            {
                return $"[{ItemType}] {Name}";
            }
                       
        }
        static void Main(string[] args)
        {
        }
    }
}
