using SPlib.Core;
using SPlib.Core.CustomAttributes;
using System;
using System.Collections.Generic;

namespace SPlib.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            string cs = @"data source = .\SQLEXPRESS; initial catalog = Person;integrated security = SSPI;";

            //Person p = new Person
            //{
            //    Id = 3391493,
            //    Name = "Elesger",
            //    Surname = "asdfgh",
            //    IsAdmin = true
            //};
            //using (Database database = new Database(cs))
            //{
            //    //nonProcedural.BeginTransaction();
            //    database.Update(p);
            //    //nonProcedural.CommitTransaction();
            //}

            using (Query query = new Query(cs))
            {
                List<Person> persons = query.Select<Person>("SELECT * FROM Persons WHERE Id>@maxId AND name LIKE @name", new { maxId = 1000, name ="Ələsgər"  });
            }
        }
    }

    class Person
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsAdmin { get; set; }
    }
}
