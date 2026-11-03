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
            public Player(string name,int maxHP, int attack, int defense,  Weapon weapon = null, Armor armor = null) 
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

        public class VVG : Goblin //1 босс
        {
            public VVG(string name, int maxHP, int attack, int defense) 
                : base(name, maxHP, attack, defense)  
            {
                CriticalChance = 30;
            }
        }

        public class Kovalsky : Skeleton //2 босс
        {
            public Kovalsky(string name, int maxHP, int attack, int defense)
                : base(name, maxHP, attack, defense) { }
        }

        public class ArchimageCpp : Mag //3 босс
        {
            public ArchimageCpp(string name, int maxHP, int attack, int defense)
                : base(name, maxHP, attack, defense)
            {
                FreezeChance = 30;
            }
        }

        public class PestovC : Skeleton //4 босс
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

        public class Game
        {
            //приватные поля класса
            private Player _player; //сам игрок
            private Random _random; //единый генератор рандома
            private int _turnCounter; //счетчик ходов
            private bool _isFrozen; //флаг заморозки
            private bool _isDefending = false; //флаг защиты

            public Game(Player player, Random random, int turnCounter, bool isFrozen)
            {
                _player = player;
                _random = random;
                _turnCounter = turnCounter;
                _isFrozen = isFrozen;
            }

            public void Run()
            {
                Console.WriteLine("--- Добро пожаловать в игру! ---");
                Console.WriteLine($"Ваш герой: {_player.Name}");
                Console.WriteLine($"HP: {_player.CurrentHP}/{_player.MaxHP} | Атака: {_player.Attack} | Защита: {_player.Defense}");
                Console.WriteLine("---------------------------------");
                while (_player.IsAlive())
                {
                    Console.WriteLine($"\n--- Ход {_turnCounter + 1} ---");
                    Console.WriteLine($"Ваше HP: {_player.CurrentHP}/{_player.MaxHP}");

                    NextTurn();
                    if (!_player.IsAlive())
                    {
                        break;
                    }
                }
                Console.WriteLine("\n---------------------------------");
                Console.WriteLine(" Вы погибли! Игра окончена.");
                Console.WriteLine($"Вы продержались {_turnCounter} ходов.");
                Console.WriteLine("-----------------------------------");
            }
            private void NextTurn()
            {
                _turnCounter++;
                if (_turnCounter % 10 == 0)
                {
                    StartBattle(CreateRandomEnemy(true));
                }
                else if (_random.Next(1, 101) <= 50)
                {
                    OpenChest();
                }
                else
                {
                    StartBattle(CreateRandomEnemy(false));
                }
            }

            private Enemy CreateRandomEnemy(bool isBoss)
            {
                int choice = _random.Next(1, isBoss ? 5 : 4);
                if (!isBoss)
                {
                    switch (choice)
                    {
                        case 1:
                            return new Goblin("Гоблин", 30, 8, 2);
                        case 2:
                            return new Skeleton("Скелет", 25, 10, 4);
                        default:
                            return new Mag("Маг", 20, 12, 1);
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case 1:
                            return new VVG("ВВГ", (int)(30 * 2.0), (int)(8 * 1.5), (int)(2 * 1.2));
                        case 2:
                            return new Kovalsky("Ковальский", (int)(25 * 2.5), (int)(10 * 1.3), (int)(4 * 1.4));
                        case 3:
                            return new ArchimageCpp("Архимаг C++", (int)(20 * 1.8), (int)(12 * 1.6), (int)(1 * 1.1));
                        default:
                            return new PestovC("Пестов С-- ", (int)(25 * 1.3), (int)(10 * 1.8), (int)(4 * 0.6));
                    }
                }
            }
            private void StartBattle(Enemy enemy)
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Вы встретили {enemy.Name}!");
                Console.WriteLine($"HP: {enemy.CurrentHP}/{enemy.MaxHP} | Атака: {enemy.Attack} | Защита: {enemy.Defense}");
                Console.WriteLine("---------------------------------");

                while (_player.IsAlive() && enemy.IsAlive()) //главный цикл боя
                {
                    if (_isFrozen == true) //проверка заморозки игрока
                    {
                        Console.WriteLine("Вы заморожены и пропускаете ход! ");
                        _isFrozen = false;
                    }

                    //ход игрока
                    else
                    {

                        int choice;
                        bool validInput = false;
                        do
                        {
                            Console.WriteLine("1. Атаковать | 2. Защищаться ");
                            validInput = int.TryParse(Console.ReadLine(), out choice)
                                && (choice == 1 || choice == 2);
                            if (!validInput) Console.WriteLine("Введите 1 или 2!");
                        } while (!validInput);

                        if (choice == 1) //если выбрана атака
                        {
                            int damage = _player.CalculateDamage(enemy);
                            enemy.TakeDamage(damage);
                            Console.WriteLine($"Вы нанесли {damage} урона!");
                            _isDefending = false;
                        }
                        else if (choice == 2) //если выбрана защита
                        {
                            _isDefending = true;
                            Console.WriteLine("Вы встали в защиту. ");
                        }
                    }
                    if (!enemy.IsAlive()) //проверка смерти врага
                    {
                        Console.WriteLine($"Вы победили {enemy.Name}!");
                        break;
                    }

                    //ход врага
                    AttackResult result = enemy.PerformAttack(_player); //получить результат атаки
                    if (_isDefending == true) //если игрок в защите
                    {
                        int rand = _random.Next(1, 101);
                        if (rand <= 40)
                        {
                            Console.WriteLine("Вы уклонились! Урон равен 0.");
                            result.Damage = 0;
                        }
                        else
                        {
                            //срабатывает блок, 70-100% от защиты доспеха
                            int armorDefense = 0;
                            if (_player.Armor != null)
                            {
                                armorDefense = _player.Armor.DefenseBonus;
                            }
                            //генерация процента блока от 70 до 100
                            int blockPercent = _random.Next(70, 101);

                            //сколько урона поглотил доспех
                            int blockedAmount = (int)(armorDefense * (blockPercent / 100.0));
                            //вычитаем блок из урона врага
                            result.Damage -= blockedAmount;

                            if (result.Damage < 0) result.Damage = 0;
                            Console.WriteLine($"Вы заблокировали {blockedAmount} урона доспехом!");
                        }
                        _isDefending = false;
                    }

                    _player.TakeDamage(result.Damage);
                    Console.WriteLine($"Враг нанес вам {result.Damage} урона.");

                    if (result.IsCritical)
                    {
                        Console.WriteLine("КРИТИЧЕСКИЙ УДАР!!!");
                    }

                    if (result.IsFrozen)
                    {
                        _isFrozen = true;
                        Console.WriteLine("Вас заморозили! Пропуск хода...");
                    }

                    //ВЫВОД СОСТОЯНИЯ
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"Итоговое состояние: ");
                    Console.WriteLine($"HP игрока: {_player.CurrentHP}/{_player.MaxHP} | HP врага: {enemy.CurrentHP}/{enemy.MaxHP}");
                    Console.WriteLine("---------------------------------");
                }
                if (!_player.IsAlive())
                {
                    Console.WriteLine("Вы пали смертью храбрых...");
                }
                Console.WriteLine("-------------------------------");
            }

            private void OpenChest()
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Вы нашли сундук!");

                int itemType = _random.Next(1, 4);
                if (itemType == 1)
                {
                    Potion potion = new Potion("Зелье");
                    _player.Heal();
                    Console.WriteLine("Вы нашли зелье и полностью восстановили здоровье!");
                }
                if (itemType == 2)
                {
                    Weapon newWeapon = new Weapon(_random.Next(5, 20), "Стальной меч");
                    Console.WriteLine($"Вы нашли оружие: {newWeapon}");
                    if (_player.Weapon != null)
                    {
                        Console.WriteLine($"Ваше текущее оружие: {_player.Weapon}");
                    }
                    else
                    {
                        Console.WriteLine("У вас нет оружия.");
                    }

                    Console.WriteLine("1. Взять новое оружие");
                    Console.WriteLine("2. Выбросить его");

                    int choice;
                    bool valid = false;
                    do
                    {
                        valid = int.TryParse(Console.ReadLine(), out choice) && (choice == 1 || choice == 2);
                        if (!valid) Console.WriteLine("Введите 1 или 2!");
                    } while (!valid);

                    if (choice == 1)
                    {
                        _player.EquipWeapon(newWeapon);
                        Console.WriteLine("Вы взяли новое оружие!");
                    }
                    else
                    {
                        Console.WriteLine("Вы выбросили оружие.");
                    }
                }
                if (itemType == 3)
                {
                    Armor newArmor = new Armor(_random.Next(3, 15), "Стальные доспехи");
                    Console.WriteLine($"Вы нашли доспехи: {newArmor}");
                    if (_player.Armor != null)
                    {
                        Console.WriteLine($"Ваши текущие доспехи: {_player.Armor}");
                    }
                    else
                    {
                        Console.WriteLine("У вас нет доспехов.");
                    }

                    Console.WriteLine("1. Взять новые доспехи");
                    Console.WriteLine("2. Выбросить их");

                    int choice;
                    bool valid = false;
                    do
                    {
                        valid = int.TryParse(Console.ReadLine(), out choice) && (choice == 1 || choice == 2);
                        if (!valid) Console.WriteLine("Введите 1 или 2!");
                    } while (!valid);

                    if (choice == 1)
                    {
                        _player.EquipArmor(newArmor);
                        Console.WriteLine("Вы взяли новые доспехи!");
                    }
                    else
                    {
                        Console.WriteLine("Вы выбросили доспехи.");
                    }
                }
                Console.ReadLine();
            }

        }

        static void Main(string[] args)
        {
            //создаем игрока
            Player player = new Player("Герой", 100, 10, 5, null, null);
            //создаем генератор случайных чисел
            Random random = new Random();
            //создаем игру, передавая игрока, рандом, счетчик ходов и флаг заморозки
            Game game = new Game(player, random, 0, false);
            //запускаем игру
            game.Run();

            //чтобы консоль не закрылась сразу после окончания игры
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();
        }
    }
}
