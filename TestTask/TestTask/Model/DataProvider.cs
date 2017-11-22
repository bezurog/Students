using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.Model
{
    /// <summary>
    /// Абстрактный класс контекста данных
    /// </summary>
    public abstract class DataProvider
    {
        private string _connString;
        /// <summary>
        /// Возвращает строку подключения провайдера
        /// </summary>
        protected string ConnString
        {
            get
            {
                return _connString;
            }
        }
        
        /// <summary>
        /// Инициализирует провайдер строкой подключения
        /// </summary>
        /// <param name="connString"></param>
        public DataProvider(string connString)
        {
            _connString = connString;
        }

        /// <summary>
        /// Коллекция объектов Student
        /// </summary>
        public IList<Student> Students { get; set; }
        
        /// <summary>
        /// Добавление студента в коллекцию
        /// </summary>
        /// <param name="st"></param>
        public virtual void AddStudent(Student student)
        {
            Students.Add(student);
        }

        /// <summary>
        /// Редактирование студента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student">экземпляр с новыми значениями свойств</param>
        public virtual void EditStudent(int id, Student student)
        {
            var stud = Students.FirstOrDefault(st => st.Id == id);
            if (stud != null)
                stud.Copy(student);
        }

        /// <summary>
        /// Удаление студента по идентификатору
        /// </summary>
        /// <param name="id"></param>
        public virtual void RemoveStudent(int id)
        {
            var stud = Students.FirstOrDefault(st => st.Id == id);
            if (stud != null)
                Students.Remove(stud);
        }
      
        /// <summary>
        /// Сохраняет контекст данных
        /// </summary>
        public abstract void CommitChanges();
    }

    public class DataException : Exception
    {
        public DataException(string message, string source) 
            : base(string.Format($"Ошибка данных, источник {source}: {message}"))
        { }
    }
}
