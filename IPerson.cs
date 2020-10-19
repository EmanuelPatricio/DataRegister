using System;
using System.Collections.Generic;
using System.Text;

namespace DataRegister
{
    public interface IPerson
    {
        public bool Add(Person p);
        public bool Remove(Person p);
        public bool Contains(Person p);
        public bool Replace(Person pOld, Person pNew);
        public Person[] ToSortedArray();
        public Person Get(Person p);
    }
}