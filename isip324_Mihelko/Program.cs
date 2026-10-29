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
        public abstract class Item //класс предмет, включает в себя перечисление с 3 предметами
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
        public class Weapon : Item //класс оружие с бонусом атаки (наследуется от предмета)
        {
            public int AttackBonus { get; protected set; } //прибавка к урону
            public Weapon(int attackBonus, string name) : base(name, ItemType.Weapon)
            {
                AttackBonus = attackBonus;
            }
            public override string ToString() 
            {
                return $"[Оружие] \"{Name}\" | +{AttackBonus} к урону";
            }
        }
        public class Armor : Item //класс доспехи дает защиту вместо урона 
        {
            public int DefenseBonus {  get; protected set; } //прибавка к защите
            public Armor(int defenseBonus, string name) : base(name, ItemType.Armor)
            {
                DefenseBonus = defenseBonus;
            }
            public override string ToString()
            {
                return $"[Доспех] \"{Name}\" | + {DefenseBonus} ед. защиты";
            }
        }
        public class Potion : Item //пустой конструктор, тк нет доп свойств, принимаем только имя зелья
        {
            public Potion(string name) : base(name, ItemType.Potion) { }
            public override string ToString()
            {
                return $"[Зелье] \"{Name}\" | Восстанавливает здоровье полностью";
            }
        }
        static void Main(string[] args)
        {
        }
    }
}
