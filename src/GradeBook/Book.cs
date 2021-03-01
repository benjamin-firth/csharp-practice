using System;
using System.Collections.Generic;
using System.IO;

namespace GradeBook
{
    public delegate void GradeAddedDelegate(object sender, EventArgs args);

    public class NamedObject
    {
        public NamedObject(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }
    }

    public interface IBook 
    {
        void AddGrade(double grade);
        Stats GetStats();
        string Name { get; }
        event GradeAddedDelegate GradeAdded;
    }
    public abstract class Book : NamedObject, IBook
    {
        public Book(string name) : base(name)
        {
        }

        public abstract event GradeAddedDelegate GradeAdded;
        public abstract void AddGrade(double grade);
        public abstract Stats GetStats();
    }

    public class DiskBook : Book
    {
        public DiskBook(string name) : base(name)
        {
        }

        public override event GradeAddedDelegate GradeAdded;

        public override void AddGrade(double grade)
        {
            using(var writer = File.AppendText($"{Name}.txt"))
            {
                writer.WriteLine(grade);
                if(GradeAdded != null)
                {
                    GradeAdded(this, new EventArgs());
                }
            }
        }

        public override Stats GetStats()
        {
            throw new NotImplementedException();
        }
    }

    public class InMemoryBook : Book
    {
        public InMemoryBook(string name) : base(name)
        {
            grades = new List<double>();
            Name = name;
        }

        public void AddLetterGrade(char letter)
        {
            switch(letter)
            {
                case 'A':
                    AddGrade(90);
                    break;

                case 'B':
                    AddGrade(80);
                    break;

                case 'C':
                    AddGrade(70);
                    break;

                case 'D':
                    AddGrade(60);
                    break;

                default:
                    AddGrade(0);
                    break;
            }
        }

        public override void AddGrade(double grade)
        {
            if(grade <= 100 && grade >= 0)
            {
                grades.Add(grade);
                if(GradeAdded != null)
                {
                    GradeAdded(this, new EventArgs());
                }
            }
            else
            {
                throw new ArgumentException($"Invalid {nameof(grade)}");
            }
        }

        public override event GradeAddedDelegate GradeAdded;

        public override Stats GetStats()
        {
            var total = new Stats();
            total.Average = 0.0;
            total.High = double.MinValue;
            total.Low = double.MaxValue;

            foreach(var num in grades)
            {
                if(num > total.High)
                {
                    total.High = num;
                }
                if(num < total.Low)
                {
                    total.Low = num;
                }
                total.Average += num;
            }

            total.Average /= grades.Count;

            switch(total.Average)
            {
                case var d when d >= 90.0:
                    total.Letter = 'A';
                    break;

                case var d when d >= 80.0:
                    total.Letter = 'B';
                    break;

                case var d when d >= 70.0:
                    total.Letter = 'C';
                    break;

                case var d when d >= 60.0:
                    total.Letter = 'D';
                    break;

                default:
                    total.Letter = 'F';
                    break;
            }

            return total;
        }
        private List<double> grades;
    }
}