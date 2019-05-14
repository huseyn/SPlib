using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using SPlib.Core.CustomAttributes;

namespace SPlib.Core.Helper
{
    public class EntityModifier
    {
        public static IEnumerable<PropertyInfo> GetInsertProperties(Type t)
        {
            return t.GetProperties().Where(p => p.Name != "Id");
        }

        public static int GetIdValue<T>(T t)
        {
            Type type = typeof(T);
            int id = 0;
            PropertyInfo[] properties = type.GetProperties();

            PropertyInfo idProp = properties.SingleOrDefault(p => p.GetCustomAttribute<PrimaryKey>() != null);
            if (idProp != null)
                id = (int)idProp.GetValue(t);
            else
            {
                foreach (var property in properties)
                {
                    if (property.Name.Equals("Id"))
                        id = (int)property.GetValue(t, null);
                    else if (property.Name.Equals(type.Name + "Id"))
                        id = (int)property.GetValue(t, null);
                }
            }

            if (id != 0)
                return id;
            else throw new ArgumentException();
        }
    }
}
