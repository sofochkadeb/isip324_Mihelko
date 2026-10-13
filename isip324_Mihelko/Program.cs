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
        static void Main(string[] args)
        {
        }
    }
}
