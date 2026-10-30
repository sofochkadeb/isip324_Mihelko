using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static isip324_Mihelko.Program;
using static System.Net.Mime.MediaTypeNames;

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
        Mag, //маг
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
        //ИЕРАРХИЯ ПРЕДМЕТОВ!!!
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


        //ИЕРАРХИЯ ПЕРСОНАЖЕЙ
        public abstract class Character //абстрактный класс персонаж
        {
            //свойства
            public string Name { get; protected set; } 
            public int MaxHP {  get; protected set; }
            public int CurrentHP {  get; protected set; }
            public int Attack {  get; protected set; } //базовая атака
            public int Defense { get; protected set; } //базовая защита
            
            //конструктор
            protected Character(string name, int maxHP, int attack, int defense)
            {
                Name = name ;
                MaxHP = maxHP ;
                CurrentHP = maxHP ; //текущее здоровье автоматически максимальное
                Attack = attack ;
                Defense = defense ;
            }

            //методы
            public void TakeDamage(int damage) //полученный урон
            {
                CurrentHP -= damage ; //из текущего здоровья отнимается урон
                if (CurrentHP < 0) //проверка на отриц здоровье
                {
                    CurrentHP = 0;
                }
            }
            public bool IsAlive() //жив ли персонаж, тру если здоровье больше 0
            {
                return CurrentHP > 0;
            }
            public void Heal() //для зелья, здоровье до максимума
            {
                CurrentHP = MaxHP; 
            }
        }


        public class Player : Character //класс игрок, наследуется от персонажа
        {
            //свойства
            public Weapon Weapon { get; protected set; } //текущее оружие
            public Armor Armor { get; protected set; } //текущие доспехи

            //конструктор
            public Player(string name,int maxHP, int attack, int defense,  Weapon weapon, Armor armor) 
                : base(name, maxHP, attack, defense)
            {
                Weapon = weapon ;
                Armor = armor ;
            }

            //методы
            public void EquipWeapon(Weapon newWeapon) //замена тек оружия на новое
            {
                Weapon = newWeapon ;
            }
            public void EquipArmor(Armor newArmor) //замена тек доспехов на новые
            {
                Armor = newArmor;
            }
            public int CalculateDamage(Enemy enemy)
            {
                int totalDamage = this.Attack; //базовая атака игрока
                if (this.Weapon != null) //если есть оружие, прибавляем бонус (к оружию)
                {
                    totalDamage += this.Weapon.AttackBonus;
                }
                totalDamage -= enemy.Defense; //вычитаем защиту у врага
                if (totalDamage < 0) //если вдруг урон отриц, то по сути мы нанесем 0 урона
                {
                    totalDamage = 0;
                }
                return totalDamage;
            }
            public bool Defend() //метод с защитой игрока при нападении врага
            { 
                //рандомное число от 1 до 100
                Random rand = new Random();
                int chance = rand.Next(1, 101);

                if(chance <= 40) //если 40 и меньше, то игрок уклонился
                {
                    return true;
                }
                return false; //уклонение провалено (блок)
            }
        }

        public class Enemy : Character //общий класс врага, наследуется от персонажа
        {
            public EnemyType Type { get; protected set; }
            public Enemy(EnemyType type, string name, int maxHP, int attack, int defense)
                : base(name, maxHP, attack, defense)
            {
                Type = type;
            }
            public virtual AttackResult PerformAttack(Player target)
            {
                int attack = this.Attack; //берется базовая атака врага

                int playerDefense = target.Defense; //ОБЩАЯ ЗАЩИТА ИГРОКА (базовая + доспехи)

                if (target.Armor != null) //если есть доспех, прибавляем бонус (к защите)
                {
                    playerDefense += target.Armor.DefenseBonus;
                }

                attack -= playerDefense; //вычитаем защиту игрока из атаки врага

                if (attack < 0) //если вдруг урон отриц, то по сути мы нанесем 0 урона
                {
                    attack = 0;
                }
                return new AttackResult(attack, false, false) ; //без критов и заморозки

            }
        }

        public class Goblin : Enemy
        {
            public int CriticalChance { get; protected set; } = 20; //шанс крит урона
            public Goblin(string name, int maxHP, int attack, int defense) 
                : base(EnemyType.Goblin, name, maxHP, attack, defense) { }
            public override AttackResult PerformAttack(Player target)
            {
                AttackResult result = base.PerformAttack(target);
                Random random = new Random();
                int chance = random.Next(1, 101);
                if (chance <= CriticalChance)
                {
                    result.Damage = (int)(result.Damage* 1.5);
                    result.IsCritical = true;
                }
                return result ;
            }
        }

        public class Skeleton : Enemy
        {
            public Skeleton(string name, int maxHP, int attack, int defense) 
                : base(EnemyType.Skeleton, name, maxHP, attack, defense) { }
            public override AttackResult PerformAttack(Player target)
            {
                int attack = this.Attack; //берет атаку скелета
                //не вычитает защиту игрока
                if (attack < 0) //если вдруг урон отриц, то по сути мы нанесем 0 урона
                {
                    attack = 0;
                }
                return new AttackResult(attack, false, false); //без критов и заморозки
            }
        }

        public class Mag : Enemy
        {
            public int FreezeChance { get; protected set; } = 20;
            public Mag(string name, int maxHP, int attack, int defense) 
                : base(EnemyType.Mag, name, maxHP, attack, defense) { }
            public override AttackResult PerformAttack(Player target)
            {
                AttackResult result = base.PerformAttack(target);
                Random random = new Random();
                int chance = random.Next(1, 101);
                if (chance <= FreezeChance)
                {
                    result.IsFrozen = true;
                }
                return result;
            }
        }

        public class VVG : Goblin
        {
            public VVG(string name, int maxHP, int attack, int defense) 
                : base(name, maxHP, attack, defense) 
            {
                CriticalChance = 30;
            }
        }

        public class Kovalsky : Skeleton
        {
            public Kovalsky(string name, int maxHP, int attack, int defense)
                : base(name, maxHP, attack, defense) { }
        }

        public class ArchimageCpp : Mag
        {
            public ArchimageCpp(string name, int maxHP, int attack, int defense)
                : base(name, maxHP, attack, defense)
            {
                FreezeChance = 30;
            }
        }

        public class PestovC : Skeleton
        {
            public int FreezeChance { get; protected set; } = 15;
            public PestovC(string name, int maxHP, int attack, int defense)
                : base(name, maxHP, attack, defense) { }
            public override AttackResult PerformAttack(Player target)
            {
                AttackResult result = base.PerformAttack(target);
                Random random = new Random();
                int chance = random.Next(1, 101);
                if (chance <= FreezeChance)
                {
                    result.IsFrozen = true;
                }
                return result;
            }
        }

        public class AttackResult //результат атаки игроком врага
        {
            public int Damage { get; set; } //сколько урона
            public bool IsCritical { get; set; } //был ли крит
            public bool IsFrozen { get; set; } //была ли заморозка
            public AttackResult(int damage, bool isCritical, bool isFrozen)
            {
                Damage = damage;
                IsCritical = isCritical;
                IsFrozen = isFrozen;
            }
        }



        static void Main(string[] args)
        {
        }
    }
}
