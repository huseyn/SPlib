using SPlib.Core;
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
            //    Name = "Huseyn",
            //    Surname = "Mikayil",
            //    IsAdmin = false
            //};
            //Person p2 = new Person
            //{
            //    Name = "Sanan",
            //    Surname = "Fataliyev",
            //    IsAdmin = false
            //};
            //using (NonProcedural nonProcedural = new NonProcedural(cs))
            //{
            //    nonProcedural.BeginTransaction();
            //    nonProcedural.Insert(p);
            //    nonProcedural.Insert(p2);
            //    nonProcedural.CommitTransaction();
            //}

            Person p = new Person
            {
                Id = 435,
                Name = "Baloglan",
                Surname = "Mikayil",
                IsAdmin = false
            };
            using (NonProcedural nonProcedural = new NonProcedural(cs))
            {
                //nonProcedural.BeginTransaction();
                nonProcedural.Select<Person>(new List<int>() { 434, 435, 436 });
                //nonProcedural.CommitTransaction();
            }

            int id = 0;
            int affectedRows = 0;
            using (Procedural context = new Procedural(cs))
            {
                id = context.ExecuteScalar("AddPerson", new Person
                {
                    Name = "Sanan",
                    Surname = "Fataliyev",
                    IsAdmin = true
                });

                affectedRows = context.Execute("UpdatePerson", new Person
                {
                    Id = 350,
                    Name = "Huseyn",
                    Surname = "Mikayil",
                    IsAdmin = false
                });

                var d = context.DeleteById("DeletePerson", id);
            }
        }
    }

    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsAdmin { get; set; }
    }
}
