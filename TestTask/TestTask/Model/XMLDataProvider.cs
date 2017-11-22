using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.Model
{
    /// <summary>
    /// Провайдер данных XML
    /// </summary>
    public class XMLDataProvider : DataProvider
    {
        private XDocument _doc;

        /// <summary>
        /// Инициализирует XMLDataProvider провайдер
        /// </summary>
        /// <param name="fileName">имя XML файла</param>
        public XMLDataProvider(string fileName) : base(fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("XML файл не найден", fileName);

            var doc = XDocument.Load(ConnString);
            if (doc == null)
                throw new DataException("Файл не является XML файлом", fileName);

            var studentElements = doc.Root.Elements("Student");
            if (studentElements == null || !studentElements.Any())
                throw new DataException("Файл не содержит списка студентов", fileName);

            _doc = doc;
            Students = new List<Student>();
            FillStudents();
        }

        /// <summary>
        /// Заполнение списка студентов
        /// </summary>
        private void FillStudents()
        {
            var studentElements = _doc.Root.Elements("Student").ToList();
            foreach (var stElement in studentElements)
            {
                int id = 0;
                string firstName = string.Empty;
                string last = string.Empty;
                int age = Student.DEFAULTAGE;
                Gender gender = Gender.Male;
                int genderBuf = 0;

                var firstNameEl = stElement.Element("FirstName");
                if (firstName != null)
                    firstName = firstNameEl.Value;

                var lastEl = stElement.Element("Last");
                if (lastEl != null)
                    last = lastEl.Value;

                var ageEl = stElement.Element("Age");
                if (ageEl != null)
                    Int32.TryParse(ageEl.Value, out age);

                var genderEl = stElement.Element("Gender");
                if (genderEl != null)
                {
                    Int32.TryParse(genderEl.Value, out genderBuf);
                    gender = genderBuf < 0 || genderBuf > 1 ? Gender.Male : (Gender)genderBuf;
                }

                var idAttr = stElement.Attribute("Id");
                if (idAttr != null)
                    Int32.TryParse(idAttr.Value, out id);

                Students.Add(new Student { Id = id, FirstName = firstName, Last = last, Age = age, Gender = gender });
             }
        }

        /// <summary>
        /// Добавление студента в коллекцию
        /// </summary>
        /// <param name="st"></param>
        public override void AddStudent(Student student)
        {
            base.AddStudent(student);
            var studElement = new XElement("Student", new XAttribute("Id", student.Id),
                                            new XElement("FirstName", student.FirstName),
                                            new XElement("Last", student.Last),
                                            new XElement("Age", student.Age),
                                            new XElement("Gender", student.Gender));
            _doc.Root.Add(studElement);
        }

        /// <summary>
        /// Редактирование студента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student">экземпляр с новыми значениями свойств</param>
        public override void EditStudent(int id, Student student)
        {
            base.EditStudent(id, student);
            var editingEl = _doc.Root.Elements("Student").FirstOrDefault(
               el => el.Attribute("Id") != null && el.Attribute("Id").Value == id.ToString());
            if (editingEl == null)
                return;

            var firstNameEl = editingEl.Element("FirstName");
            if (firstNameEl == null)
            {
                firstNameEl = new XElement("FirstName");
                editingEl.Add(firstNameEl);
            }
            firstNameEl.Value = student.FirstName;

            var lastEl = editingEl.Element("Last");
            if (lastEl == null)
            {
                lastEl = new XElement("Last");
                editingEl.Add(lastEl);
            }
            lastEl.Value = student.Last;

            var ageEl = editingEl.Element("Age");
            if (ageEl == null)
            {
                ageEl = new XElement("Age");
                editingEl.Add(ageEl);
            }
            ageEl.Value = student.Age.ToString();

            var genderEl = editingEl.Element("Gender");
            if (genderEl == null)
            {
                genderEl = new XElement("Gender");
                editingEl.Add(genderEl);
            }
            genderEl.Value = student.Gender.ToString();
        }

        /// <summary>
        /// Удаление студента по идентификатору
        /// </summary>
        /// <param name="id"></param>
        public override void RemoveStudent(int id)
        {
            base.RemoveStudent(id);
            var removingEl = _doc.Root.Elements("Student").FirstOrDefault(
                el =>  el.Attribute("Id") != null && el.Attribute("Id").Value == id.ToString());
            if (removingEl != null)
                removingEl.Remove();
        }

        public override void CommitChanges()
        {
            _doc.Save(ConnString);
        }
    }
}
