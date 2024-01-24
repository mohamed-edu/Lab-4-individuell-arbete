using Lab_4_individuell_arbete.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_4_individuell_arbete
{
    internal class Program
    {
         static void Main(string[] args)
        {
            while (true) // förklaring till meny
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine("Choose a function:");
                Console.WriteLine("1. Get personnel");
                Console.WriteLine("2. Get all students");
                Console.WriteLine("3. Get all students in a specific class");
                Console.WriteLine("4. Get all student grades, which teacher graded and what date it graded");
                Console.WriteLine("5. Get a all activ courses ");
                Console.WriteLine("6. Add new students");
                Console.WriteLine("7. Add new personnel");
                Console.WriteLine("8. get all department(by what subject)");
                Console.WriteLine("9. Put New grade");
                Console.WriteLine("10. get all Each department Salary");
                Console.WriteLine("11. get avarage salary of each department");
                Console.WriteLine("12. Exit");
                Console.ForegroundColor = ConsoleColor.White;

                string choice = Console.ReadLine();

                switch (choice) //användarens meny
                {
                    
                    case "1":
                        GetPersonnel();
                       
                        break;
                    case "2":
                        GetAllStudents();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "3":
                        GetStudentsInClass();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "4":
                        GetGrades();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "5":
                        ShowActiveCourses();
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case "6":
                        AddNewStudent();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "7":
                        AddNewPersonnel();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "8":
                        GetEachSubjectTeacher();
                        break;
                    case "9":
                        PutNewGrade();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "10":
                        DepartmentSalary();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "11":
                        DepartmentAvarageSalary();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "12":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }



        static void GetPersonnel() // här kan användaren se alla anstälda eller i kategorier
        {
            using (var context = new TheSchoolContext())
            {
                Console.WriteLine("here you can choose which personel you want their name");
                Console.WriteLine("to see all teacher tab 1");
                Console.WriteLine("to see all Admin tab 2");
                Console.WriteLine("to see the principle tab 3");
                Console.WriteLine("to see the janitors tab 4");
                Console.WriteLine("to see all personel tab 5");
                Console.WriteLine("press key after seeing the info to get back to menu");

                int choosePersonnel = int.Parse(Console.ReadLine());

                var filteredPersonnel = context.People
                    .Include(p => p.EmploymentHistories)
                    .OrderBy(p => p.PersonId)
                    .Where(s =>
                        (choosePersonnel == 1 && s.Position == "Teacher") ||
                        (choosePersonnel == 2 && s.Position == "Admin") ||
                        (choosePersonnel == 3 && s.Position == "Principal") ||
                        (choosePersonnel == 4 && s.Position == "Janitor") ||
                        (choosePersonnel == 5 && (s.Position == "Teacher" || s.Position == "Admin" || s.Position == "Janitor" || s.Position == "Principal")))
                    .OrderBy(s => s.FirstName)
                    .ThenBy(s => s.LastName)
                    .ToList();

                foreach (var personnel in filteredPersonnel)
                {
                    var totalMonthsWorked = personnel.EmploymentHistories
                  .Select(e =>
                  {
                      if (e.EndDate.HasValue)
                          return ((e.EndDate.Value.Year - e.StartDate.Year) * 12) + e.EndDate.Value.Month - e.StartDate.Month;
                      else
                          return ((DateTime.Now.Year - e.StartDate.Year) * 12) + DateTime.Now.Month - e.StartDate.Month;
                  })
                  .DefaultIfEmpty(0)
                  .Sum();

                    var yearsWorked = totalMonthsWorked / 12;
                    var monthsWorked = totalMonthsWorked % 12;

                    Console.WriteLine($"Id: {personnel.PersonId} - Position: {personnel.Position} - Name: {personnel.FirstName} {personnel.LastName} - Worked: {yearsWorked} years and {monthsWorked} months");
                }
                Console.ReadKey();
                Console.Clear();
            }
        }
        static void GetEachSubjectTeacher()
        {
            using (var context = new TheSchoolContext())
            {
                var teachersByDepartment = context.Teachers
                  .Join(context.People,
                      teacher => teacher.FkPersonId,
                      person => person.PersonId,
                      (teacher, person) => new { Teacher = teacher, Person = person })
                  .GroupBy(t => t.Teacher.SubjectField)
                  .ToList();

                foreach (var group in teachersByDepartment)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Department: {group.Key}");
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var teacherPersonPair in group)
                    {
                        Console.WriteLine($" - {teacherPersonPair.Person.FirstName} {teacherPersonPair.Person.LastName}");
                    }
                }
                Console.ReadKey();
                Console.Clear();
            }

        }
        static void GetAllStudents() // här kan användaren se elever soreterade efter förnamn eller efternamn
        {
            using (var context1 = new TheSchoolContext())
            {
               
                    var students = context1.Students
                           .Include(s => s.FkPerson)
                           .OrderBy(s => s.FkPerson.LastName)
                           .ThenBy(s => s.FkPerson.FirstName)
                           .ToList();
                    foreach (var student in students)
                    {
                        Console.WriteLine($"{student.FkPerson.FirstName} {student.FkPerson.LastName} - {student.FkPerson.Position}:");
                    }
               
            }
        }

       

        static void GetGrades() // här kan användaren se betyg från sista månad
        {
            using (var context = new TheSchoolContext())
            {
                var gradesLastMonth = context.Grades
                        .Include(g => g.FkStudent.FkPerson)
                        .Include(g => g.FkCourse)
                        .Include(g=>g.FkTeacher.FkPerson)
                        .OrderBy(g => g.GradeDate)
                        .ToList();

                foreach (var grade in gradesLastMonth)
                {
                    Console.WriteLine($"Student name; {grade.FkStudent.FkPerson.FirstName} {grade.FkStudent.FkPerson.LastName} " +
                        $"- Course: {grade.FkCourse.CourseName}, Grade: {grade.Grade1} - teacher: {grade.FkTeacher.FkPerson.FirstName} {grade.FkTeacher.FkPerson.LastName} date: {grade.GradeDate}");
                }
            }
        }
        static void GetStudentsInClass() // här kan användaren se elever i en viss klass
        {
            using (var context = new TheSchoolContext())
            {
                var classes = context.Students.Select(s => s.Class).Distinct().ToList();
                Console.WriteLine("Choose a class:");
                foreach (var className in classes)
                {
                    Console.WriteLine(className);
                }

                string selectedClass = Console.ReadLine();

                var studentsInClass = context.Students
                    .Where(s => s.Class == selectedClass)
                    .OrderBy(s => s.StudentId)
                    .Include(s => s.FkPerson)
                    .OrderBy(s => s.FkPerson.FirstName)
                    .ThenBy(s => s.FkPerson.LastName)
                    .ToList();

                foreach (var student in studentsInClass)
                {
                    Console.WriteLine($"{student.FkPerson.FirstName} {student.FkPerson.LastName} - Class: {student.Class}");
                }
            }
        }
        static void ShowActiveCourses() // Visa en lista på alla (aktiva) kurser
        {
            using (var context = new TheSchoolContext())
            {
                var activeCourses = context.Courses
                    .Where(c => c.IsActive)
                    .ToList();

                Console.WriteLine("Active Courses:");
                foreach (var course in activeCourses)
                {
                    Console.WriteLine($"Course ID: {course.CourseId}, Course Name: {course.CourseName}");
                }
            }
           
        }
       
        static void DepartmentSalary()
        {
            using (var context = new TheSchoolContext())
            {
                var departmentSalaries = context.Teachers
                    .Join(context.People, t => t.FkPersonId, p => p.PersonId, (t, p) => new { Teacher = t, Person = p })
                    .GroupBy(tp => tp.Teacher.SubjectField)
                    .Select(group => new
                    {
                        Department = group.Key,
                        TotalSalary = group.Max(tp => tp.Teacher.Salary)
                    })
                    .ToList();

                foreach (var departmentSalary in departmentSalaries)
                {
                    Console.WriteLine($"Department: {(departmentSalary.Department)}, Total Salary: {departmentSalary.TotalSalary}");
                }
            }
        }

        static void DepartmentAvarageSalary()
        {
            using (var context = new TheSchoolContext())
            {
                var departmentAverageSalaries = context.Teachers
                    .Join(context.People, t => t.FkPersonId, p => p.PersonId, (t, p) => new { Teacher = t, Person = p })
                    .GroupBy(tp => tp.Teacher.SubjectField)
                    .Select(group => new
                    {
                        Department = group.Key,
                        AverageSalary = group.Average(tp => tp.Teacher.Salary)
                    })
                    .ToList();

                foreach (var departmentAverageSalary in departmentAverageSalaries)
                {
                    Console.WriteLine($"Department: {departmentAverageSalary.Department}, Average Salary: {departmentAverageSalary.AverageSalary}");
                }
            }

        }


        static void AddNewStudent() // här addas ny elev, användaren bestämmer ID, namn osv
            {
                using (var context = new TheSchoolContext())
                {
                    Console.WriteLine("Enter new student details:");

                    Console.Write("First Name: ");
                    string firstName = Console.ReadLine();

                    Console.Write("Last Name: ");
                    string lastName = Console.ReadLine();

                    Console.Write("Class: ");
                    string studentClass = Console.ReadLine();

                    Console.WriteLine("ID: ");
                    int StudentId = int.Parse(Console.ReadLine());
                    

                    var newStudent = new Student
                    {
                        FkPerson = new Person
                        {
                        
                            FirstName = firstName,
                            LastName = lastName,
                            PersonId = StudentId
                        },
                        Class = studentClass
                    };

                    context.Students.Add(newStudent);
                    context.SaveChanges();

                    Console.WriteLine("New student added successfully!");
               
                }
            }

        static void AddNewPersonnel() // här addas ny personal, användar bestämmer id, namn osv
        {
            using (var context = new TheSchoolContext())
            {
                Console.WriteLine("Enter new personnel details:");
                Console.WriteLine("ID: ");
                int PersonId = int.Parse(Console.ReadLine());

                Console.Write("First Name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();

                Console.Write("Position: ");
                string position = Console.ReadLine();

                var newPersonel = new Person
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Position = position,
                    PersonId = PersonId
                };

                context.People.Add(newPersonel);
                context.SaveChanges();

                Console.WriteLine("New personnel added successfully!");

            }

        }
        static void PutNewGrade() // här addas ny betyg och mer info 
        {
            using (var context = new TheSchoolContext())
            {
                Console.WriteLine("Enter new student grade details:");

                Console.Write("Course Id?: ");
                int courseId = int.Parse(Console.ReadLine());

                Console.Write("Teacher Id: ");
                int teacherId = int.Parse(Console.ReadLine());

                Console.Write("Grade (0-3): ");
                int newGrade = int.Parse(Console.ReadLine());

                Console.WriteLine("Grade date (yyyy-mm-dd: ");
                DateTime gradeDate = DateTime.Parse(Console.ReadLine());


                var theNewGrade = new Grade
                {

                    FkCourseId = courseId,
                    FkTeacherId = teacherId,
                    Grade1 = newGrade,
                    GradeDate = gradeDate

                   
                };

                context.Grades.Add(theNewGrade);
                context.SaveChanges();

                Console.WriteLine("New Grade added successfully!");

            }
        }
    }
}

