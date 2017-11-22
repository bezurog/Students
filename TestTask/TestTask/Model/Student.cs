using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;

namespace TestTask.Model
{
    /// <summary>
    /// Тип "Пол студента
    /// </summary>
    public enum Gender { Male, Female };

    /// <summary>
    /// Тип студент
    /// </summary>
    public class Student : ObservableObject
    {
        public const int MINAGE = 16;
        public const int MAXAGE = 100;
        public static int DEFAULTAGE = 17;

        private int _id;
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { Set<int>("Id", ref _id, value); }
        }

        private string _firstName;
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set { Set<string>("FirstName", ref _firstName, value); }
        }

        private string _last;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Last
        {
            get { return _last; }
            set { Set<string>("Last", ref _last, value); }
        }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName
        {
            get { return $"{FirstName} {Last}"; }
        }

        private int _age;
        /// <summary>
        /// Возраст
        /// </summary>
        public int Age
        {
            get { return _age; }
            set
            {
                int age = value < MINAGE || value > MAXAGE ? DEFAULTAGE : value;
                Set<int>("Age", ref _age, age);
            }
        }


        public string AgeString
        {
            get
            {
                if (Age == 0)
                    return "Возраст не указан";

                if (Age >= 10 && Age <= 20)
                    return $"{Age} лет";

                if (Age % 10 == 1)
                    return $"{Age} год";

                if (Age % 10 >= 2 && Age % 10 <= 4)
                    return $"{Age} года";

                return $"{Age} лет";
            }
        }

        private Gender _gender;
        /// <summary>
        /// Пол
        /// </summary>
        public Gender Gender
        {
            get { return _gender; }
            set { Set<Gender>("Gender", ref _gender, value); }
        }


        /// <summary>
        /// Копирует данные из другого экземпляра
        /// </summary>
        /// <param name="student">источник</param>
        public void Copy(Student student)
        {
            this.FirstName = student.FirstName;
            this.Last = student.Last;
            this.Age = student.Age;
            this.Gender = student.Gender;
        }

        /// <summary>
        /// Возвращает новый идентификатор
        /// </summary>
        /// <param name="students">коллекция студентов</param>
        /// <returns></returns>
        public static int GetNextId(IList<Student> students)
        {
            return !students.Any() ? 0 : students.Max(s => s.Id) + 1;
        }
    }
}
