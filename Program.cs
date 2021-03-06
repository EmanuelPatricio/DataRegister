﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataRegister
{
    class Program
    {
        public static string dataArchive;
        private static PersonSet PersonSet = new PersonSet(27, 10);
        static void Main(string[] args)
        {
            int op = 0;
            if (args.Length == 1)
            {
                dataArchive = $@"{args[0]}";

                if (!File.Exists(dataArchive))
                {
                    File.Create(dataArchive).Close();
                    File.WriteAllText(dataArchive,"ID,Name,LastName,Savings,Password,Data");
                }
                else
                {
                    var list = GetAllData();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i > 0)
                        {
                            list[i] += $",{i}";
                            var person = Person.FromCsvLine(list[i]);
                            PersonSet.Add(person);
                        }
                    }
                }

                do
                {
                    Console.Clear();
                    Console.WriteLine("----------MENU----------");
                    Console.WriteLine("\t[1] Create");
                    Console.WriteLine("\t[2] Read");
                    Console.WriteLine("\t[3] Search");
                    Console.WriteLine("\t[4] Update");
                    Console.WriteLine("\t[5] Delete");
                    Console.WriteLine("\t[6] Save to CSV");
                    Console.WriteLine("\t[7] Exit");
                    Console.WriteLine("------------------------");
                    Console.Write("Insert your option: [ ]\b\b");
                    op = Convert.ToInt32(Console.ReadLine());

                    switch (op)
                    {
                        case 1:
                            Console.Clear();
                            var person = Person.FromConsole(CaptureData());
                            if (PersonSet.Add(person))
                                Console.WriteLine("Person created correctly!");
                            else
                                Console.WriteLine("The id has been already registered, please try again");
                            Console.ReadKey();
                            break;

                        case 2:
                            Console.Clear();
                            foreach (var obj in PersonSet.ToSortedArray())
                            {
                                Console.WriteLine(obj.ToString());
                            }
                            Console.ReadKey();
                            break;

                        case 3:
                            Console.Clear();
                            Console.Write("Enter the id: ");
                            string id = NumbersOnly(11);
                            Console.WriteLine("\n");
                            var person1 = new Person(id);
                            var record = PersonSet.Get(person1);
                            if (record == null)
                                Console.WriteLine("Couldn't found the id inserted");
                            else
                                Console.WriteLine(record.ToString());
                            Console.ReadKey();
                            break;

                        case 4:
                            UpdatePerson();
                            Console.ReadKey();
                            break;

                        case 5:
                            DeletePerson();
                            Console.ReadKey();
                            break;
                        
                        case 6:
                            Person.SaveToCsv(PersonSet.ToSortedArray());
                            break;

                        default:
                            break;
                    }
                } while (op != 7);

                Console.Clear();
            }
            else
            {
                Console.WriteLine("The database route is missing!");
            }
        }
        public static List<string> GetAllData()
        {
            string[] data = File.ReadAllLines(dataArchive);
            List<string> list = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                list.Add(data[i]);
            }
            return list;
        }

        public static string CaptureData(string ident = null)
        {
            bool confirm = false;
            string id;

            if (ident == null)
            {
                Console.Write("ID number: ");
                id = NumbersOnly(11);
                Console.WriteLine();
            }
            else
                id = ident;
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Savings: ");
            double savings = double.Parse(ReadDouble());

            string pass, cpass;
            do
            {
                Console.Write("Password: ");
                pass = ReadPassword();
                Console.Write("Confirm password: ");
                cpass = ReadPassword();
            } while (string.IsNullOrEmpty(pass) || !(pass.Equals(cpass)));

            int age;
            do
            {
                Console.Write("Age: ");
                age = int.Parse(NumbersOnly(3));
                Console.WriteLine();
            } while (age < 7 || age > 120);
            
            int gender = 0;
            do
            {
                confirm = true;
                Console.Write("Gender (M/W): ");
                switch (Console.ReadLine().ToUpper())
                {
                    case "W":
                        gender = 8;
                        break;
                    case "M":
                        gender = 0;
                        break;
                    default:
                        confirm = false;
                        break;
                }
            } while (!confirm);
            
            int maritals = 0;
            do
            {
                confirm = true;
                Console.Write("Marital Status (S/M): ");
                switch (Console.ReadLine().ToUpper())
                {
                    case "M":
                        maritals = 4;
                        break;
                    case "S":
                        maritals = 0;
                        break;
                    default:
                        confirm = false;
                        break;
                }
            } while (!confirm);

            int academicd = 0;
            do
            {
                confirm = true;
                Console.Write("Academic Degree (I/B/G/M): ");
                switch (Console.ReadLine().ToUpper())
                {
                    case "M":
                        maritals = 3;
                        break;
                    case "G":
                        maritals = 2;
                        break;
                    case "B":
                        maritals = 1;
                        break;
                    case "I":
                        maritals = 0;
                        break;
                    default:
                        confirm = false;
                        break;
                }
            } while (!confirm);
            Console.WriteLine();

            return $"{id},{name},{lastName},{savings},{pass},{age},{gender},{maritals},{academicd}";
        }
        
        public static void UpdatePerson()
        {
            Console.Clear();
            Console.Write("Enter the id: ");
            string id = NumbersOnly(11);
            Console.WriteLine();
            var oldPerson = new Person(id);
            if (!(PersonSet.Contains(oldPerson)))
                Console.WriteLine("Couldn't found the id inserted");
            else
            {
                oldPerson = PersonSet.Get(oldPerson);
                Console.WriteLine(oldPerson.ToString());
                var newPerson = Person.FromCsvLine(CaptureData(id));
                PersonSet.Replace(oldPerson, newPerson);
            }
        }

        public static void DeletePerson()
        {
            string del;

            Console.Clear();
            Console.Write("Enter the id: ");
            string id = NumbersOnly(11);
            Console.WriteLine();
            var person = new Person(id);
            if (!(PersonSet.Contains(person)))
                Console.WriteLine("Couldn't found the id inserted");
            else
            {
                person = PersonSet.Get(person);
                Console.WriteLine(person.ToString());
                Console.Write("\nAre you sure you want to delete this record? (Y/N): ");

                del = Console.ReadLine();
                if (del == "Y")
                    PersonSet.Remove(person);
            }
        }
        
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }

        static string NumbersOnly(int limit)
        {
            string value = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && value.Length < limit)
                {
                    double val = 0;
                    bool _x = double.TryParse(key.KeyChar.ToString(), out val);
                    if (_x)
                    {
                        value += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && value.Length > 0)
                    {
                        value = value.Substring(0, (value.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);

            return value;
        }

        static string ReadDouble()
        {
            string doubleValue = "";
            int dotCount = 0;
            ConsoleKeyInfo key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key != ConsoleKey.Backspace)
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.D0:
                            Console.Write("0");
                            doubleValue += key.KeyChar;                             
                            break;
                        case ConsoleKey.D1:
                            Console.Write("1");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D2:
                            Console.Write("2");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D3:
                            Console.Write("3");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D4:
                            Console.Write("4");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D5:
                            Console.Write("5");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D6:
                            Console.Write("6");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D7:
                            Console.Write("7");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D8:
                            Console.Write("8");
                            doubleValue += key.KeyChar;
                            break;
                        case ConsoleKey.D9:
                            Console.Write("9");
                            doubleValue += key.KeyChar;
                            break;
                
                        case ConsoleKey.OemPeriod:
                            if(dotCount == 0)
                            {
                                dotCount++;
                                Console.Write(".");
                                doubleValue += key.KeyChar;
                            }
                            break;
                    }
                }
                else
                {
                    if (doubleValue.Length > 0)
                    {
                        char character = doubleValue.Last();
                        if (character == '.')
                            dotCount = 0;
                        doubleValue = doubleValue.Remove(doubleValue.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                key = Console.ReadKey(true);
            }
            Console.WriteLine();
            return doubleValue;
        }
    }
}
