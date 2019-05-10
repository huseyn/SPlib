using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SPlib.Core.Helper
{
    public class EntityModifier
    {
        public static IEnumerable<PropertyInfo> GetInsertProperties(Type t)
        {
            return t.GetProperties().Where(p => p.Name != "Id");
        }
    }
}
