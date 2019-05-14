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
            
            Person p = new Person
            {
                Id = 3391500,
                Name = "Baloglan",
                Surname = "Mikayil",
                IsAdmin = false
            };
            using (NonProcedural nonProcedural = new NonProcedural(cs))
            {
                //nonProcedural.BeginTransaction();
                nonProcedural.Delete(p);
                //nonProcedural.CommitTransaction();
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
