using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TestTask.Model;

namespace TestTask.ViewModel
{

    /// <summary>
    /// ������ ������������� �������� �����
    /// </summary>
    public class MainViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _fileName = @"Data\Students.xml";
        private DataProvider _context;
        /// <summary>
        /// ������������� ���������� ���-������
        /// </summary>
        public MainViewModel()
        {
            AddCommand = new RelayCommand(OnAddStudent, CanAddStudent);
            EditCommand = new RelayCommand(OnEditStudent, CanEditStudent);
            DeleteCommand = new RelayCommand(OnDeleteStudent, CanDeleteStudent);

            GenderTypes = new List<string>
            {
                Model.Gender.Male.ToString(),
                Model.Gender.Female.ToString()
            };
            try
            {
                _context = new XMLDataProvider(_fileName);
                Students = new ObservableCollection<Student>(_context.Students);
                SelectedStudents = new ObservableCollection<Student>();
                FirstName = string.Empty;
                Last = string.Empty;
            }
            catch(Exception ex)
            {
                ErrorText = ex.Message;
            }



        }

        #region ��������
        private Student _selectedStudent;
        /// <summary>
        /// ���������� ������� � DataGrid
        /// </summary>
        public Student SelectedStudent
        {
            get { return _selectedStudent; }
            set
            {
                _selectedStudent = value;
                if (value != null)
                {
                    
                    Last = value.Last;
                    FirstName = value.FirstName;
                    Age = value.Age;
                    Gender = (int)value.Gender;
                }
                else
                {
                    Last = string.Empty;
                    FirstName = string.Empty;
                    Age = Student.DEFAULTAGE;
                    Gender = (int)Model.Gender.Male;
                }
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedStudents");
                RaisePropertyChanged("SelectedStudent");
            }
        }

        private string _last;
        /// <summary>
        /// ������� �������� ��������
        /// </summary>
        public string Last
        {
            get { return _last; }
            set
            {
                _last = value;
                 AddCommand.RaiseCanExecuteChanged();
                 RaisePropertyChanged("Last");
            }
        }

        private string _firstName;
        /// <summary>
        /// ��� �������� ��������
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                AddCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("FirstName");
            }
        }

        private int _age;
        /// <summary>
        /// ������� �������� ��������
        /// </summary>
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                AddCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("Age");
            }
        }

        private int _gender;
        /// <summary>
        /// ��� �������� ��������
        /// </summary>
        public int Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                RaisePropertyChanged("Gender");
            }
        }

        private string _errorText;
        /// <summary>
        /// ����� ��� ����������� ������ ������ �����
        /// </summary>
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                RaisePropertyChanged("ErrorText");
            }
        }

        #endregion

        #region ���������
        private List<string> _genderTypes;
        /// <summary>
        /// ������ �����
        /// </summary>
        public List<string> GenderTypes
        {
            get { return _genderTypes; }
            set { _genderTypes = value; }
        }

        private ObservableCollection<Student> _students;
        /// <summary>
        /// ��������� ���������
        /// </summary>
        public ObservableCollection<Student> Students
        {
            get { return _students; }
            set
            {
                _students = value;
                this.RaisePropertyChanged("Students");
            }
        }

        private ObservableCollection<Student> _selectedStudents;
        /// <summary>
        /// ��������� ���������� ���������
        /// </summary>
        public ObservableCollection<Student> SelectedStudents
        {
            get { return _selectedStudents; }
            set
            {
                _selectedStudents = value;
                
                DeleteCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("SelectedStudents");
            }
        }
        #endregion

        #region ��������
        /// <summary>
        /// ������� ���������� ��������
        /// </summary>
        public RelayCommand AddCommand { get; private set; }
        /// <summary>
        /// ������� �������������� ��������
        /// </summary>
        public RelayCommand EditCommand { get; private set; }
        /// <summary>
        /// ������� �������� ��������
        /// </summary>
        public RelayCommand DeleteCommand { get; private set; }

        /// <summary>
        /// ���������� ������� ���������� ��������
        /// </summary>
        public void OnAddStudent()
        {
            Student st = new Student
            {
                Id = Student.GetNextId(_context.Students),
                Last = this.Last,
                FirstName = this.FirstName,
                Age = this.Age,
                Gender = (Gender)this.Gender
            };
            Students.Add(st);
            _context.AddStudent(st);
            _context.CommitChanges();
            SelectedStudent = st;
            this.RaisePropertyChanged("Students");
        }

        public bool CanAddStudent()
        {
            return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(Last) && Age >= Student.MINAGE && Age <= Student.MAXAGE;
        }

        /// <summary>
        /// ���������� ������� �������������� ��������
        /// </summary>
        public void OnEditStudent()
        {
            Student st = new Student
            {
                Last = this.Last,
                FirstName = this.FirstName,
                Age = this.Age,
                Gender = (Gender)this.Gender
            };
            _context.EditStudent(SelectedStudent.Id, st);
            _context.CommitChanges();
            Students = new ObservableCollection<Student>(_context.Students);
            this.RaisePropertyChanged("SelectedStudent");
            this.RaisePropertyChanged("Students");
        }

        public bool CanEditStudent()
        {
            return SelectedStudent != null;
        }

        /// <summary>
        /// ���������� ������� �������� ���������
        /// </summary>
        public void OnDeleteStudent()
        {
            if (MessageBox.Show("������� ������?", "�������", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var student in SelectedStudents)
                    _context.RemoveStudent(student.Id);
                _context.CommitChanges();

                SelectedStudent = null;
                Students = new ObservableCollection<Student>(_context.Students);
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanDeleteStudent()
        {
            return SelectedStudents != null && SelectedStudents.Any();
        }
        #endregion

        #region IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "FirstName":
                        if (string.IsNullOrEmpty(FirstName))
                            error = "��� �� ����� ���� ������!";
                        break;
                    case "Last":
                        if (string.IsNullOrEmpty(Last))
                            error = "������� �� ����� ���� ������!";
                        break;
                    case "Age":
                        if (Age < Student.MINAGE || Age > Student.MAXAGE)
                            error = $"������� ������ ���� � ��������� �� {Student.MINAGE} �� {Student.MAXAGE}!";
                        break;
                    case "Gender":
                        if (Gender != (int) Model.Gender.Male && Gender != (int)Model.Gender.Female)
                            error = $"�������� ���!";
                        break;
                }
                return error;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

    }
}