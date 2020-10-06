using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataRegister
{
    public class Person
    {
        public static List<Person> people = new List<Person>();
        public string Id { get; }
        public string Name { get; }
        public string LastName { get; }
        public string FullName => $"{Name} {LastName}";
        public double Savings;
        public int Age => (Data >> 4);
        public Gender Gender => (Gender)(Data & 0b1000);
        public MaritalStatus MaritalStatus => (MaritalStatus)(Data & 0b100);
        public AcademicDegree AcademicDegree => (AcademicDegree)(Data & 0b11);
        public string Password { get; set; }
        private int Data = 0;

        private Person(in string id) {
            Id = id;
        }

        public Person(in string id, in string name, in string lastname, in double savings, in string pass, in int age, in Gender gender, in MaritalStatus maritalS, in AcademicDegree academicD)
        {
            Id = id;
            Name = name;
            LastName = lastname;
            Savings = savings;
            Password = pass;
            Data = (age << 4) | (int)gender | (int)maritalS | (int)academicD;
        }

        public override string ToString()
            => $"{GetType().Name}({nameof(Id)}: {Id}; {nameof(FullName)}: {FullName}; {nameof(Savings)}: {Savings}; {nameof(Password)}: {Password}; {nameof(Age)}: {Age}; {nameof(Gender)}: {Gender}; {nameof(MaritalStatus)}: {MaritalStatus}; {nameof(AcademicDegree)}: {AcademicDegree})";

        public override bool Equals(object obj)
        {
            if (obj is Person other)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        internal static Person FromCsvLine(string line)
        {
            string[] tokens = line.Split(",");

            (string id, string name, string lastname, double savings, string pass, int packedData) = (tokens[0], tokens[1], tokens[2], double.Parse(tokens[3]), tokens[4], int.Parse(tokens[5]));
            int age = (packedData >> 4);
            Gender gender = (Gender)(packedData & 0b1000);
            MaritalStatus maritalS = (MaritalStatus)(packedData & 0b100);
            AcademicDegree academicD = (AcademicDegree)(packedData & 0b11);

            return new Person (id, name, lastname, savings, pass, age, gender, maritalS, academicD);
        }

        internal static Person FromConsole(string record)
        {
            var tokens = record.Split(',');
            (string id, string name, string lastname, double savings, string password, int age, Gender gender, MaritalStatus status, AcademicDegree academicGrade) = (tokens[0], tokens[1], tokens[2], double.Parse(tokens[3]), tokens[4], int.Parse(tokens[5]), (Gender)int.Parse(tokens[6]), (MaritalStatus)int.Parse(tokens[7]), (AcademicDegree)int.Parse(tokens[8]));

            return new Person(id, name, lastname, savings, password, age, gender, status, academicGrade);
        }

        internal static Person GetOnePerson(string id)
        {
            Person search = new Person(id);
            return people?.Where(a => search.Equals(a)).SingleOrDefault();
        }

        internal string Insert()
        {
            try
            {
                if (people.Contains(this))
                    return "The id has been already registered, please try again";
                else
                {
                    people.Add(this);
                    return "Person created correctly!";
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("An error has ocurred: ", e);
            }
        }

        internal string Update()
        {
            try
            {
                var pos = people.FindIndex(a => a.Equals(this));
                people[pos] = this;
                return "Field modified!";
            }
            catch (Exception e)
            {
                throw new ApplicationException("An error has ocurred: ", e);
            }
        }

        internal string Delete(string id)
        {
            try
            {
                Person search = new Person(id);
                people.RemoveAll(x => search.Equals(x));
                return "Field deleted!";
            }
            catch (Exception e)
            {
                throw new ApplicationException("An error has ocurred: ", e);
            }
        }

        internal static void SaveToCsv()
        {
            if(people.Count() > 0)
            {
                File.WriteAllText(Program.dataArchive, "ID,Name,LastName,Savings,Password,Data");
                foreach (var p in Person.people)
                {
                    File.AppendAllText(Program.dataArchive, $"{Environment.NewLine}{p.Id},{p.Name},{p.LastName},{p.Savings},{p.Password},{p.Data}");
                }
            }
        }
    }
}

public enum Gender
{
  Male = 0,
  Female = 8
}

public enum MaritalStatus
{
  Single = 0,
  Married = 4
}

public enum AcademicDegree
{
  Initial = 0,
  Bachelor = 1,
  Grade = 2,
  Master = 3
}