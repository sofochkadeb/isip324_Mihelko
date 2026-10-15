using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace isip324_Mihelko
{
    internal class Program
    {
        //созданы перечисления по полу, по статусу студента, ученой степени преподавателя, пункты главного меню
        enum Gender { Male, Female }
        enum StudentStatus { Active, OnLeave, Expelled, Graduated } //учится, в академическом, отчислен, выпустился
        enum AcademicDegree { None, Candidate, Doctor }
        enum MenuOption { AddStudent = 1, AddTeacher, AddCourse } //добавить студента, добавить преподавателя, добавить курс

        class ValidationException : Exception //класс-исключение "данные не прошли проверку" наследуется от Exception
        {
            public ValidationException(string message) : base(message) { } //конструктор берет текст ошибки при throw new.. и передает его родителю
        }
        class EntityNotFoundException : Exception //класс-исключение "не нашли" наследуется от Exception
        {
            public EntityNotFoundException(string message) : base(message) { } 
        }
        class DuplicateEntityException : Exception //класс-исключение "уже есть" наследуется от Exception
        {
            public DuplicateEntityException(string message) : base(message) { }
        }
        class CourseCapacityExceededException : Exception //класс-исключение "мест нет" наследуется от Exception
        {
            public CourseCapacityExceededException(string message) : base(message) { }
        }

        interface IIdentifiable //у сущностей у всех будет Id, любой класс,
        {                           //который будет реализовывать интерфейс, должен иметь свойство Id
            int Id { get; }
        }
        interface IValidatable //класс, который реализует интерфейс, должен уметь
        {                           //сказать, корректен ли он, если нет, то объяснить, что не так
            bool IsValid(out string error);
            
        }
        abstract class Person
        {
            private int _id;
            private string _firstName;
            private string _lastName;
            private int _age;
            private string _email;
            private string _phone;
            private Gender _gender;
            public int Id
            {
                get => _id;
            }
            public string FirstName
            {
                get => _firstName;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ValidationException("Имя не может быть пустым.");
                    }
                    else
                    {
                        _firstName = value;
                    }
                }
            }
            public string LastName
            {
                get => _lastName;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ValidationException("Фамилия не может быть пустой.");
                    }
                    else
                    {
                        _lastName = value;
                    }
                }
            }
            public string Phone
            {
                get => _phone;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ValidationException("Номер телефона не может быть пустым. ");
                    }
                    bool isValid = true;
                    foreach (char symbol in value)
                    {
                        if (char.IsDigit(symbol))
                            isValid = true;
                        else if (symbol == '+' || symbol == '-' || symbol == ' ' || symbol == '(' || symbol == ')')
                            isValid = true;
                        else
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid == false)
                    {
                        throw new ValidationException("Номер телефона должен содержать разрешенные для него символы");
                    }  
                    _phone = value.Trim();   
                }
            }
            public int Age
            {
                get => _age;
                set
                {
                    if (value < 5 && value > 110)
                    {
                        throw new ValidationException("Возраст должен быть в пределах разумного.");
                    }
                    else
                    {
                        _age = value;
                    }
                }
            }
            public string Email
            {
                get => _email;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ValidationException("Почтовый адрес не может быть пустой.");
                    }
                    else if ( Regex.IsMatch(value,"^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$") == false)
                    {
                        throw new ValidationException("Почтовый адрес должен подходить под шаблон.");
                    }
                    else
                    {
                        _email = value;
                    }
                }
            }
            public string FullName
            {
                get { return FirstName + ' ' + LastName; }
            }
            public Gender Gender { get => _gender; set => _gender = value; }
            
            public Person(string firstName, string lastName, int age, string phone, string email, Gender gender)
            {
                FirstName = firstName;
                LastName = lastName;    
                Age = age;
                Email = email;
                Phone = phone;
                Gender = gender;
            }
            public abstract string GetInfo();
        }
        static void Main(string[] args)
        {
        }
    }
}
