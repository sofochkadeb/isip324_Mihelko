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
        abstract class Person : IIdentifiable, IValidatable
        {
            private static int _counter = 0; //статический счетчик для генерации id людей
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
                        _firstName = value.Trim();
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
                        _lastName = value.Trim();
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
                    else
                    {
                        _phone = value.Trim();
                    }
                }
            }
            public int Age
            {
                get => _age;
                set
                {
                    if (value < 16 || value > 100)
                    {
                        throw new ValidationException("Возраст должен быть от 16 до 100 лет.");
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
                    else
                    {
                        _email = value.Trim();
                    }
                }
            }
            public string FullName //вычисляемое свойство без поля (считается каждый раз)
            {
                get { return FirstName + ' ' + LastName; }
            }
            public Gender Gender { get => _gender; set => _gender = value; }
            
            public Person(string firstName, string lastName, int age, string phone, string email, Gender gender)
            {
                _id = ++_counter; //уникальный id, генерируется автоматически
                FirstName = firstName; //через свойство - для проверки
                LastName = lastName;    
                Age = age;
                Email = email;
                Phone = phone;
                Gender = gender; 
            }
            public abstract string GetInfo(); //абстрактный метод, который должен быть у каждого наследника
            public virtual bool IsValid(out string error) //проверка валидности, срабатывает по запросу, а не только проверяет в момент присваивания
            {                                               //проверяет все поля сразу
                if (string.IsNullOrWhiteSpace(_firstName))
                {
                    error = "Имя пустое";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(_lastName))
                {
                    error = "Фамилия пустая";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(_email))
                { error = "Email пустой"; return false; }
                error = null;
                return true;
            }
            public override string ToString() => FullName; //для того чтобы печаталось ФИО, а не имя класса
        }
        class Student : Person
        {
            private string _studentId; //номер зачетки
            private int _enrollmentYear; //год поступления
            private StudentStatus _status;
            private List<Course> _enrolledCourses = new List<Course>(); //сколько курсов

            public string StudentId
            {
                get => _studentId;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ValidationException("Номер зачётки не может быть пустым.");
                    }
                    else
                    {
                        _studentId = value.Trim();
                    }
                }
            }

            public int EnrollmentYear
            {
                get => _enrollmentYear;
                set
                {
                    if (value < 2000 || value > DateTime.Now.Year + 1)
                        throw new ValidationException("Некорректный год поступления.");
                    _enrollmentYear = value;
                }
            }

            public StudentStatus Status //взят из enum
            {
                get => _status;
                set => _status = value; 
            }
            //публичное свойство типа список курсов только для чтения», которое возвращает приватное поле
            public IReadOnlyList<Course> EnrolledCourses => _enrolledCourses;

            public Student(string firstName, string lastName, int age, //конструктор класса от родителя
                           string email, string phone, Gender gender,
                           string studentId, int enrollmentYear)
                : base(firstName, lastName, age, email, phone, gender) 
            {
                StudentId = studentId;
                EnrollmentYear = enrollmentYear;
                _status = StudentStatus.Active; // по умолчанию учится
            }

            public override string GetInfo() //переопределил метод родителя
                => $"Id={Id}, Студент: {FullName}, зачётка: {StudentId}, " +
                   $"год поступления: {EnrollmentYear}, статус: {Status}, " +
                   $"курсов: {_enrolledCourses.Count}"; 

            // метод записать на курс (проверка на дубликат внутри)
            public void EnrollToCourse(Course course)
            {
                if (course == null)
                    throw new ValidationException("Курс не может быть null.");
                if (_enrolledCourses.Contains(course))
                    throw new DuplicateEntityException(
                        $"Студент {FullName} уже записан на курс {course.Title}.");
                else
                {
                    _enrolledCourses.Add(course);
                }
            }

            // метод отписаться от курса
            public bool UnenrollFromCourse(Course course)
                => _enrolledCourses.Remove(course);
            // метод для проверки перед записью, записан ли на курс, и узнать состоит ли на курсе
            public bool IsEnrolled(Course course)
                => _enrolledCourses.Contains(course);

            // дополняем проверку родителя своей (метод из родителя поэтому здесь override)
            public override bool IsValid(out string error)
            {
                if (!base.IsValid(out error)) return false;
                if (string.IsNullOrWhiteSpace(_studentId))
                { error = "Номер зачётки пустой"; return false; }
                error = null;
                return true;
            }
        }
        class Teacher : Person
        {
            private string _department; //кафедра
            private string _position; //должность
            private AcademicDegree _degree; //из enum
            private List<Course> _teachingCourses = new List<Course>();

            public string Department
            {
                get => _department;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ValidationException("Кафедра не может быть пустой.");
                    else
                    {
                        _department = value.Trim();
                    }
                }
            }

            public string Position
            {
                get => _position;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ValidationException("Должность не может быть пустой.");
                    else
                    {
                        _position = value.Trim();
                    }
                }
            }

            public AcademicDegree Degree //берется из enum
            {
                get => _degree;
                set => _degree = value;
            }
            //публичное свойство типа список курсов только для чтения», которое возвращает приватное поле (курсы преподавателя, там курсы студента)
            public IReadOnlyList<Course> TeachingCourses => _teachingCourses;

            public Teacher(string firstName, string lastName, int age, //конструктор класса от родителя
                           string email, string phone, Gender gender,
                           string department, string position, AcademicDegree degree)
                : base(firstName, lastName, age, email, phone, gender)
            {
                Department = department;
                Position = position;
                _degree = degree;
            }

            public override string GetInfo()
                => $"Id={Id},Преподаватель: {FullName}, кафедра: {Department}, " +
                   $"должность: {Position}, степень: {Degree}, " +
                   $"курсов ведёт: {_teachingCourses.Count}";
            //метод добавления курса, который ведет преподаватель
            public void AssignCourse(Course course)
            {
                if (course == null)
                    throw new ValidationException("Курс не может быть null.");
                if (_teachingCourses.Contains(course))
                    throw new DuplicateEntityException(
                        $"Преподаватель {FullName} уже ведёт курс {course.Title}.");
                _teachingCourses.Add(course);
            }
            //удалить курс у преподавателя
            public void RemoveCourse(Course course)
                => _teachingCourses.Remove(course);
        }

        class Course : IIdentifiable, IValidatable
        {
            private static int _counter = 0; //статический счетчик для генерации id курсов

            private int _id;
            private string _title; //название курса
            private string _description; //описание курса
            private int _credits; //сложность курса в единицах "веса"
            private int _maxStudents;
            private Teacher _teacher;                  // может быть null (пока не назначен)
            private List<Student> _students = new List<Student>();

            public int Id => _id;

            public string Title
            {
                get => _title;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ValidationException("Название курса не может быть пустым.");
                    else
                    {
                        _title = value.Trim();
                    }
                }
            }

            public string Description
            {
                get => _description;
                set
                {
                    if (value != null) //пришла строка, сохрани обрезанную
                    {
                        _description = value.Trim();
                    }
                    else //если пустая, сохрани пустую строку
                    {
                       _description = "";
                    }
                } 
            }

            public int Credits
            {
                get => _credits;
                set
                {
                    if (value < 1 || value > 10)
                        throw new ValidationException("Кредитов должно быть от 1 до 10.");
                    else
                    {
                        _credits = value;
                    }
                }
            }

            public int MaxStudents
            {
                get => _maxStudents;
                set
                {
                    if (value < 1 || value > 200)
                        throw new ValidationException("Максимум студентов — от 1 до 200.");
                    else
                    {
                        _maxStudents = value;
                    }
                }
            }

            public Teacher Teacher
            {
                get => _teacher;
                set => _teacher = value; // может быть null
            }

            ////публичное свойство типа список студентов только для чтения», которое возвращает приватное поле
            public IReadOnlyList<Student> Students => _students;

            public Course(string title, string description, int credits, int maxStudents) //конструктор класса
            {
                _id = ++_counter;
                Title = title;
                Description = description;
                Credits = credits;
                MaxStudents = maxStudents;
            }
            //метод для определения свободных мест (true/false), только геттер
            public bool HasFreeSlots() => _students.Count < _maxStudents;

            //добавить студента с проверками: есть ли место и не записан ли уже
            public bool AddStudent(Student s)
            {
                if (s == null) return false;
                if (!HasFreeSlots()) return false;
                if (_students.Contains(s)) return false;
                _students.Add(s);
                return true;
            }
            //метод удаления студента с курса
            public bool RemoveStudent(Student s) => _students.Remove(s);

            public string GetInfo() //метод переопределенный через родителя 
            {
                string teacherName;
                if (_teacher != null)
                    teacherName = _teacher.FullName;
                else
                    teacherName = "не назначен";

                return $"Id={Id}, Курс: {Title} ({Credits} кр.), преподаватель: {teacherName}, " +
                       $"студентов: {_students.Count}/{_maxStudents}";
            }
                

            public bool IsValid(out string error) //добавленная проверка переопределенная от родителя
            {
                if (string.IsNullOrWhiteSpace(_title))
                { error = "Название курса пустое"; return false; }
                if (_credits < 1)
                { error = "Некорректное число кредитов"; return false; }
                if (_maxStudents < 1)
                { error = "Некорректный максимум студентов"; return false; }
                else
                {
                    error = null;
                }
                return true;
            }
        }
        class University
        {
            private List<Student> _students = new List<Student>(); //список студентов
            private List<Teacher> _teachers = new List<Teacher>(); //список преподавателей
            private List<Course> _courses = new List<Course>(); //список курсов

            // ДОБАВЛЕНИЕ
            public Student AddStudent(string firstName, string lastName, int age,
                                      string email, string phone, Gender gender,
                                      string studentId, int enrollmentYear)
            {
                // проверка дубликата по email
                if (_students.Any(s => s.Email == email))
                    throw new DuplicateEntityException(
                        $"Студент с email {email} уже существует.");

                var student = new Student(firstName, lastName, age, email, phone,
                                          gender, studentId, enrollmentYear);
                _students.Add(student);
                return student;
            }

            public Teacher AddTeacher(string firstName, string lastName, int age,
                                      string email, string phone, Gender gender,
                                      string department, string position,
                                      AcademicDegree degree)
            {
                if (_teachers.Any(t => t.Email == email))
                    throw new DuplicateEntityException(
                        $"Преподаватель с email {email} уже существует.");

                var teacher = new Teacher(firstName, lastName, age, email, phone,
                                          gender, department, position, degree);
                _teachers.Add(teacher);
                return teacher;
            }

            public Course AddCourse(string title, string description,
                                    int credits, int maxStudents)
            {
                if (_courses.Any(c => c.Title == title))
                    throw new DuplicateEntityException(
                        $"Курс с названием {title} уже существует.");

                var course = new Course(title, description, credits, maxStudents);
                _courses.Add(course);
                return course;
            }

            // ЗАПИСЬ СТУДЕНТА НА КУРС
            public void EnrollStudent(int studentId, int courseId)
            {
                // ищем студента, если не нашли бросаем исключение
                Student student = FindStudentById(studentId);
                if (student == null)
                    throw new EntityNotFoundException($"Студент с Id {studentId} не найден.");

                // ищем курс, если не нашли бросаем исключение
                Course course = FindCourseById(courseId);
                if (course == null)
                    throw new EntityNotFoundException($"Курс с Id {courseId} не найден.");

                // проверка: есть ли свободные места
                if (!course.HasFreeSlots())
                    throw new CourseCapacityExceededException(
                        $"На курсе «{course.Title}» нет свободных мест.");

                // проверка: не записан ли уже
                if (student.IsEnrolled(course))
                    throw new DuplicateEntityException(
                        $"Студент уже записан на курс «{course.Title}».");

                // связываем обе стороны: у курса появляется студент, у студента — курс
                course.AddStudent(student);
                student.EnrollToCourse(course);
            }

            //НАЗНАЧЕНИЕ ПРЕПОДАВАТЕЛЯ
            public void AssignTeacher(int teacherId, int courseId)
            {
                // ищем преподавателя, если не нашли бросаем исключение
                Teacher teacher = FindTeacherById(teacherId);
                if (teacher == null)
                    throw new EntityNotFoundException($"Преподаватель с Id {teacherId} не найден.");

                // ищем курс, если не нашли бросаем исключение
                Course course = FindCourseById(courseId);
                if (course == null)
                    throw new EntityNotFoundException($"Курс с Id {courseId} не найден.");

                // назначаем препода на курс и добавляем курс преподу
                course.Teacher = teacher;
                teacher.AssignCourse(course);
            }

            // ПОИСК ЧЕРЕЗ LINQ
            public Student FindStudentById(int id)
                => _students.FirstOrDefault(s => s.Id == id);

            public Teacher FindTeacherById(int id)
                => _teachers.FirstOrDefault(t => t.Id == id);

            public Course FindCourseById(int id)
                => _courses.FirstOrDefault(c => c.Id == id);

            // ВЫБОРКИ ЧЕРЕЗ LINQ 1) ищем курсы опред студента 2) ищем студентов опред курса
            public List<Course> GetCoursesOfStudent(int studentId)
            {
                // ищем студента, если не нашли бросаем исключение
                Student s = FindStudentById(studentId);
                if (s == null)
                    throw new EntityNotFoundException($"Студент с Id {studentId} не найден.");

                // возвращаем список его курсов
                return s.EnrolledCourses.ToList();
            }
            public List<Student> GetStudentsOfCourse(int courseId)
            {
                // ищем курс, если не нашли бросаем исключение
                Course c = FindCourseById(courseId);
                if (c == null)
                    throw new EntityNotFoundException($"Курс с Id {courseId} не найден.");

                // возвращаем список студентов этого курса
                return c.Students.ToList();
            }


            // Полиморфизм: возвращаем студентов и преподавателей как общий тип Person
            public IEnumerable<Person> GetAllPeople()
                => _students.Cast<Person>().Concat(_teachers);

            // ВЫВОД ВСЕГО
            public void PrintAllStudents()
            {
                if (_students.Count == 0) { Console.WriteLine("Студентов нет."); return; }
                Console.WriteLine(" Все студенты ");
                foreach (var s in _students.OrderBy(s => s.LastName)) //LINQ
                    Console.WriteLine(s.GetInfo()); 
            }

            public void PrintAllTeachers()
            {
                if (_teachers.Count == 0) { Console.WriteLine("Преподавателей нет."); return; }
                Console.WriteLine(" Все преподаватели ");
                foreach (var t in _teachers.OrderBy(t => t.LastName)) //LINQ
                    Console.WriteLine(t.GetInfo()); 
            }

            public void PrintAllCourses()
            {
                if (_courses.Count == 0) { Console.WriteLine("Курсов нет."); return; }
                Console.WriteLine(" Все курсы ");
                foreach (var c in _courses.OrderBy(c => c.Title)) //LINQ
                    Console.WriteLine(c.GetInfo());
            }

            // Списки для меню (чтобы меню знало, из чего выбирать)
            public IReadOnlyList<Student> Students => _students;
            public IReadOnlyList<Teacher> Teachers => _teachers;
            public IReadOnlyList<Course> Courses => _courses;
        }

        static class InputHelper //БЕЗОПАСНЫЙ ВВОД БЕЗ ПАДЕНИЙ ПРОГИ
        {
            // Чтение int в диапазоне [min, max], пока не введут корректно
            public static int ReadInt(string prompt, int min, int max)
            {
                while (true)
                {
                    Console.Write(prompt);
                    if (int.TryParse(Console.ReadLine(), out int value)
                        && value >= min && value <= max)
                        return value;
                    Console.WriteLine($"Введите целое число от {min} до {max}.");
                }
            }

            // Непустая строка
            public static string ReadNonEmptyString(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    string s = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                    Console.WriteLine("Строка не может быть пустой.");
                }
            }

            // Email с проверкой через Regex
            public static string ReadEmail(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    string s = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(s))
                        return s.Trim();
                    Console.WriteLine("Неверный формат email. Попробуйте снова.");
                }
            }

            // Выбор значения enum по номеру
            // Выбор пола
            public static Gender ReadGender(string prompt)
            {
                while (true)
                {
                    Console.WriteLine(prompt);
                    Console.WriteLine("  1. Male");
                    Console.WriteLine("  2. Female");
                    Console.Write("Ваш выбор: ");

                    if (int.TryParse(Console.ReadLine(), out int n))
                    {
                        if (n == 1) return Gender.Male;
                        if (n == 2) return Gender.Female;
                    }
                    Console.WriteLine("Неверный выбор.");
                }
            }
            // Выбор учёной степени
            public static AcademicDegree ReadAcademicDegree(string prompt)
            {
                while (true)
                {
                    Console.WriteLine(prompt);
                    Console.WriteLine("  1. None");
                    Console.WriteLine("  2. Candidate");
                    Console.WriteLine("  3. Doctor");
                    Console.Write("Ваш выбор: ");

                    if (int.TryParse(Console.ReadLine(), out int n))
                    {
                        if (n == 1) return AcademicDegree.None;
                        if (n == 2) return AcademicDegree.Candidate;
                        if (n == 3) return AcademicDegree.Doctor;
                    }
                    Console.WriteLine("Неверный выбор.");
                }
            }
            // Выбор статуса студента
            public static StudentStatus ReadStudentStatus(string prompt)
            {
                while (true)
                {
                    Console.WriteLine(prompt);
                    Console.WriteLine("  1. Active");
                    Console.WriteLine("  2. OnLeave");
                    Console.WriteLine("  3. Expelled");
                    Console.WriteLine("  4. Graduated");
                    Console.Write("Ваш выбор: ");

                    if (int.TryParse(Console.ReadLine(), out int n))
                    {
                        if (n == 1) return StudentStatus.Active;
                        if (n == 2) return StudentStatus.OnLeave;
                        if (n == 3) return StudentStatus.Expelled;
                        if (n == 4) return StudentStatus.Graduated;
                    }
                    Console.WriteLine("Неверный выбор.");
                }
            }

            // Да/Нет
            // Спрашивает да/нет. Возвращает true для "д"/"да", false для "н"/"нет".
            public static bool Confirm(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt + " (д/н): ");
                    string s = Console.ReadLine();

                    // ReadLine может вернуть null 
                    if (s == null)
                    {
                        Console.WriteLine("Введите 'д' или 'н'.");
                        continue; // новый круг цикла
                    }

                    s = s.Trim(); // убираем пробелы по краям

                    // Проверяем «да»
                    if (s == "д" || s == "да")
                        return true;

                    // Проверяем «нет»
                    if (s == "н" || s == "нет")
                        return false;

                    // Ничего не подошло, просим ещё раз
                    Console.WriteLine("Введите 'д' или 'н'.");
                }
            }
        }

        class ConsoleMenu //ГЛАВНОЕ МЕНЮ
        {
            private University _university;

            public ConsoleMenu(University u) => _university = u; //Публичный конструктор ConsoleMenu, принимает один аргумент — объект University.
                                                                 //Сохраняет его в приватное поле _university
            public void Run()
            {
                while (true)
                {
                    ShowMainMenu();
                    int choice = InputHelper.ReadInt("Ваш выбор: ", 0, 8);

                    // try/catch вокруг switch
                    try
                    {
                        switch (choice)
                        {
                            case 1: AddStudentFlow(); break;
                            case 2: AddTeacherFlow(); break;
                            case 3: AddCourseFlow(); break;
                            case 4: EnrollStudentFlow(); break;
                            case 5: AssignTeacherFlow(); break;
                            case 6: ShowStudentCoursesFlow(); break;
                            case 7: ShowCourseStudentsFlow(); break;
                            case 8: ShowAllFlow(); break;
                            case 0: return; // выход
                        }
                    }
                    catch (ValidationException ex)
                    { Console.WriteLine($"Ошибка данных: {ex.Message}"); }
                    catch (EntityNotFoundException ex)
                    { Console.WriteLine($"Не найдено: {ex.Message}"); }
                    catch (DuplicateEntityException ex)
                    { Console.WriteLine($"Дубликат: {ex.Message}"); }
                    catch (CourseCapacityExceededException ex)
                    { Console.WriteLine($"Мест нет: {ex.Message}"); }
                }
            }

            private void ShowMainMenu()
            {
                Console.WriteLine("\n УПРАВЛЕНИЕ УНИВЕРСИТЕТОМ ");
                Console.WriteLine("1. Добавить студента");
                Console.WriteLine("2. Добавить преподавателя");
                Console.WriteLine("3. Добавить курс");
                Console.WriteLine("4. Записать студента на курс");
                Console.WriteLine("5. Назначить преподавателя на курс");
                Console.WriteLine("6. Показать курсы студента");
                Console.WriteLine("7. Показать студентов курса");
                Console.WriteLine("8. Показать все списки");
                Console.WriteLine("0. Выход");
            }

            private void AddStudentFlow()
            {
                string fn = InputHelper.ReadNonEmptyString("Имя: ");
                string ln = InputHelper.ReadNonEmptyString("Фамилия: ");
                int age = InputHelper.ReadInt("Возраст: ", 16, 100);
                string email = InputHelper.ReadEmail("Email: ");
                string phone = InputHelper.ReadNonEmptyString("Телефон: ");
                Gender g = InputHelper.ReadGender("Пол:");
                string sid = InputHelper.ReadNonEmptyString("Номер зачётки: ");
                int year = InputHelper.ReadInt("Год поступления: ", 2000, DateTime.Now.Year + 1);

                var s = _university.AddStudent(fn, ln, age, email, phone, g, sid, year);
                Console.WriteLine($"Добавлен: {s.GetInfo()}");
            }

            private void AddTeacherFlow()
            {
                string fn = InputHelper.ReadNonEmptyString("Имя: ");
                string ln = InputHelper.ReadNonEmptyString("Фамилия: ");
                int age = InputHelper.ReadInt("Возраст: ", 16, 100);
                string email = InputHelper.ReadEmail("Email: ");
                string phone = InputHelper.ReadNonEmptyString("Телефон: ");
                Gender g = InputHelper.ReadGender("Пол:");
                string dep = InputHelper.ReadNonEmptyString("Кафедра: ");
                string pos = InputHelper.ReadNonEmptyString("Должность: ");
                AcademicDegree deg = InputHelper.ReadAcademicDegree("Учёная степень:");

                var t = _university.AddTeacher(fn, ln, age, email, phone, g, dep, pos, deg);
                Console.WriteLine($"Добавлен: {t.GetInfo()}");
            }

            private void AddCourseFlow()
            {
                string title = InputHelper.ReadNonEmptyString("Название курса: ");
                string desc = InputHelper.ReadNonEmptyString("Описание: ");
                int credits = InputHelper.ReadInt("Кредитов (1–10): ", 1, 10);
                int max = InputHelper.ReadInt("Максимум студентов (1–200): ", 1, 200);

                var c = _university.AddCourse(title, desc, credits, max);
                Console.WriteLine($"Добавлен: {c.GetInfo()}");
            }

            private void EnrollStudentFlow()
            {
                _university.PrintAllStudents();
                int sid = InputHelper.ReadInt("Id студента: ", 1, int.MaxValue);
                _university.PrintAllCourses();
                int cid = InputHelper.ReadInt("Id курса: ", 1, int.MaxValue);

                _university.EnrollStudent(sid, cid);
                Console.WriteLine("Студент записан на курс.");
            }

            private void AssignTeacherFlow()
            {
                _university.PrintAllTeachers();
                int tid = InputHelper.ReadInt("Id преподавателя: ", 1, int.MaxValue);
                _university.PrintAllCourses();
                int cid = InputHelper.ReadInt("Id курса: ", 1, int.MaxValue);

                _university.AssignTeacher(tid, cid);
                Console.WriteLine("Преподаватель назначен на курс.");
            }

            private void ShowStudentCoursesFlow()
            {
                _university.PrintAllStudents();
                int sid = InputHelper.ReadInt("Id студента: ", 1, int.MaxValue);
                var courses = _university.GetCoursesOfStudent(sid);
                if (courses.Count == 0) { Console.WriteLine("Студент ни на что не записан."); return; }
                foreach (var c in courses)
                    Console.WriteLine(c.GetInfo());
            }

            private void ShowCourseStudentsFlow()
            {
                _university.PrintAllCourses();
                int cid = InputHelper.ReadInt("Id курса: ", 1, int.MaxValue);
                var students = _university.GetStudentsOfCourse(cid);
                if (students.Count == 0) { Console.WriteLine("На курс никто не записан."); return; }
                foreach (var s in students)
                    Console.WriteLine(s.GetInfo());
            }

            private void ShowAllFlow()
            {
                _university.PrintAllStudents();
                _university.PrintAllTeachers();
                _university.PrintAllCourses();
            }
        }

        static void Main(string[] args)
        {
                // создаём университет
                var university = new University();

                // ТЕСТОВЫЕ ДАННЫЕ
                try
                {
                    // добавляем преподавателя
                    var t = university.AddTeacher(
                        "Иван", "Петров", 45,
                        "petrov@mail.ru", "+7(999)123-45-67",
                        Gender.Male, "ФИТ", "Доцент", AcademicDegree.Candidate);

                    // добавляем курс
                    var c = university.AddCourse(
                        "Математика", "Основы математического анализа", 5, 30);

                    // назначаем преподавателя на курс
                    university.AssignTeacher(t.Id, c.Id);

                    // добавляем студента
                    var s = university.AddStudent(
                        "Анна", "Сидорова", 19,
                        "anna@mail.ru", "89991234567",
                        Gender.Female, "S-001", 2024);

                    // записываем студента на курс
                    university.EnrollStudent(s.Id, c.Id);
                }
                catch (Exception ex)
                {
                    // если что-то в тестовых данных пошло не так просто скажем,не упадем
                    Console.WriteLine($"Ошибка при загрузке тестовых данных: {ex.Message}");
                }

                // ЗАПУСК МЕНЮ
                var menu = new ConsoleMenu(university);
                menu.Run();
            }
        }
    }