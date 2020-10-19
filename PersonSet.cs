using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRegister
{
    public class PersonSet : IPerson
    {
        private readonly int numberOfBuckets;
        private readonly int bucketSize;
        private Person[][] Persons;

        public PersonSet(in int bucketCount, in int sizeOfBuckets)
        {
            numberOfBuckets = bucketCount;
            bucketSize = sizeOfBuckets;
            Persons = new Person[bucketCount][];
            for (int i = 0; i < Persons.Length; i++)
			{
                Persons[i] = new Person[bucketSize];
			}
        }

        private string BinarySearch(Person person)
        {
            for (int i = 0; i < numberOfBuckets; i++)
			{
                var bucket = Persons[i];
                int max = bucket.Length - 1;
                int min = 0;
                while (min <= max)
	            {
                    int medium = (min + max) / 2;
                    if (bucket[medium] != null)
	                {
                        if (bucket[medium].GetHashCode() == person.GetHashCode())
	                        return $"{i},{medium}";
                        else if (person.GetHashCode() < bucket[medium].GetHashCode())
                            max = medium - 1;
                        else
                            min = medium + 1;
	                }
                    else
	                {
                        max = medium - 1;
	                }
	            }
			}
            return null;
        }

        public bool Remove(Person p)
        {
            if (Contains(p))
	        {
                string[] tokens = BinarySearch(p).Split(',');
                int bucketPosition = int.Parse(tokens[0]);
                int positionInBucket = int.Parse(tokens[1]);
                var tmp = CloneBucketArray(Persons[bucketPosition], positionInBucket + 1);
                Persons[bucketPosition][positionInBucket] = null;
                Persons[bucketPosition] = MergeBucketArray(tmp, Persons[bucketPosition], positionInBucket);
                return true;
	        }
            return false;
        }

        public bool Contains(Person person)
        {
            if (BinarySearch(person) != null)
	            return true;
            return false;
        }

        public bool Replace(Person pOld, Person pNew)
        {
            if (Contains(pOld))
            {
                string[] tokens = BinarySearch(pOld).Split(',');
                Persons[int.Parse(tokens[0])][int.Parse(tokens[1])] = pNew;
                return true;
	        }
            return false;
        }

        public Person Get(Person p)
        {
            if (Contains(p)) 
                return GetPersonFromBinarySearch(p); 
            return null;
        }

        private Person GetPersonFromBinarySearch(Person p)
        {
            string[] tokens = BinarySearch(p).Split(',');
            return Persons[int.Parse(tokens[0])][int.Parse(tokens[1])];
        }

        public Person[] ToSortedArray()
        {
            Person[][] buckets = new Person[Persons.Length][];
            Person[] newPersonArray = new Person[bucketSize * numberOfBuckets];
            for (int i = 0; i < buckets.Length; i++)
			{
                buckets[i] = CloneBucketArray(Persons[i], 0);
                var shrinkFrom = buckets[i].Length - GetNullValuesOrElementsToRightInBucket(Persons[i]);
                if (buckets[i].Length != shrinkFrom)
                    if (shrinkFrom == 0)
                        buckets[i] = null;
                    else
                        buckets[i] = ShrinkBucket(Persons[i], shrinkFrom);
                newPersonArray = MergeBucketArray(buckets[i], newPersonArray);
			}
            return ShrinkBucket(newPersonArray);
        }

        public bool Add(Person person)
        {
            bool isSaved = false;
            if (Contains(person))
                return isSaved;
            int bucketPosition = person.GetHashCode() % numberOfBuckets;
            int position = 0;
            bool isPositionNull = false, isArrayEmpty = true, isArrayFull = false;
            Person[] tmp = new Person[bucketSize];
            Person[] bucket = Persons[bucketPosition];
            for (int i = 0; i < bucketSize; i++)
			{
                if (bucket[i] != null)
	            {
                    isArrayEmpty = false;
                    if (person.CompareTo(Persons[bucketPosition][i]) < 0)
	                {
                        position = i;
                        isArrayFull = false;
                        break;
	                }
                    else if (person.CompareTo(Persons[bucketPosition][i]) > 0  && ((bucket.Length - 1) >= i + 1 && Persons[bucketPosition][i + 1] == null))
                    {
                        position = i + 1;
                        isPositionNull = true;
                        isArrayFull = false;
                        break;
                    }
                    else if (person.CompareTo(Persons[bucketPosition][i]) > 0 && ((bucket.Length - 1) >= i + 1 && Persons[bucketPosition][i + 1] != null))
                    {
                        isArrayFull = true;
                    }
	            }
			}
            if ((!isPositionNull) && (!isArrayEmpty))
            {
                tmp = CloneBucketArray(bucket, position);
                bucket[position] = person;
                bucket = MergeBucketArray(tmp, bucket, position + 1);
                Persons[bucketPosition] = bucket;
            }
            else if (isArrayFull)
            {
                var oldLength = bucket.Length;
                bucket = ExtendBucket(bucket);
                bucket[oldLength] = person;
            }
            else
            {
                Persons[bucketPosition][position] = person;
            }
            isSaved = true;
            return isSaved;
        }

        public static Person[] ExtendBucket(Person[] people)
        {
            var oldBucketSize = people.Length;
            Person[] tmp = new Person[(int)(oldBucketSize * 1.5)];
            for (int i = 0; i < oldBucketSize; i++)
            {
                tmp[i] = people[i];
            }
            return tmp;
        }

        internal static Person[] ShrinkBucket(Person[] people, int? startShrinkIndex = null)
        {
            var newBucketSize = people.Length - GetNullValuesOrElementsToRightInBucket(people, startShrinkIndex);
            Person[] tmp = new Person[newBucketSize];
            for (int i = 0; i < newBucketSize; i++)
            {
                if (startShrinkIndex.HasValue)
                {
                    if (i > startShrinkIndex)
                        break;
                    tmp[i] = people[i];
                }
                else
                {
                    if (people[i] == null)
                        break;
                    tmp[i] = people[i];
                }
            }
            return tmp;
        }

        internal static int GetNullValuesOrElementsToRightInBucket(Person[] people, int? startPosition = null)
        {
            int availableSpaces = 0;
            for (int i = people.Length - 1; i >= 0; i--)
            {
                if (startPosition.HasValue)
                {
                    if (i >= startPosition)
                        availableSpaces++;
                }
                else
                {
                    if (people[i] == null)
                        availableSpaces++;
                }

            }
            return availableSpaces;
        }

        internal static Person[] CloneBucketArray(Person[] people, int startPosition)
        {
            Person[] tmpArray = new Person[people.Length];
            int cont = 0;
            for (int i = 0; i < people.Length; i++)
            {
                if (i >= startPosition && people[i] != null)
                {
                    tmpArray[cont] = people[i];
                    cont++;
                }
            }
            return tmpArray;
        }

        internal Person[] MergeBucketArray(Person[] fromArray, Person[] toArray, int? destinationIndex = null)
        {
            if (fromArray != null)
            {
                if (!destinationIndex.HasValue)
                    destinationIndex = toArray.Length - GetNullValuesOrElementsToRightInBucket(toArray, destinationIndex); 
                while (true)
                {
                    var availableSpaces = GetNullValuesOrElementsToRightInBucket(toArray, destinationIndex);
                    var personCount = fromArray.Length - GetNullValuesOrElementsToRightInBucket(fromArray);
                    if (availableSpaces < personCount)
                    {
                        toArray = ExtendBucket(toArray);
                    }
                    else
                        break;
                }
                int cont = 0;
                for (int i = destinationIndex.Value; i < toArray.Length; i++)
                {
                    if (cont < fromArray.Length)
                    {
                        toArray[i] = fromArray[cont];
                        cont++;
                    }
                }
            }
            return toArray;
        }
    }
}