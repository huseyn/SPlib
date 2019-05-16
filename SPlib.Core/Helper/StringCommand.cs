using SPlib.Core.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SPlib.Core.Helper
{
    public class StringCommand
    {
        public static string InsertSql(Type type)
        {
            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(p => p.Name != "Id");
            string namePart = string.Join(",", properties.Select(p => p.Name));
            string valuePart = string.Join(",", properties.Select(p => "@" + p.Name));
            return $"INSERT INTO {string.Concat(type.Name, "s")}({namePart}) " + $"OUTPUT INSERTED.ID VALUES({valuePart})";
        }

        public static string UpdateSql(Type type)
        {
            string columnName = string.Empty;
            PropertyInfo[] properties = type.GetProperties();
            PropertyInfo idProp = properties.SingleOrDefault(p => p.GetCustomAttribute<PrimaryKey>() != null);
            if (idProp != null)
                columnName = idProp.Name;
            else
            {
                foreach (var property in properties)
                {
                    if (property.Name.Equals("Id") || property.Name.Equals(type.Name + "Id"))
                        columnName = property.Name;
                }
            }
            string settingPart = string.Join(",", properties.Where(p => p.GetCustomAttribute<PrimaryKey>() == null).Select(p => p.Name + "=" + "@" + p.Name));
            string conditionPart = $"WHERE {columnName} = @{columnName}";
            return $"UPDATE {string.Concat(type.Name, "s")} SET {settingPart} {(columnName == null ? string.Empty : conditionPart)}";
        }

        internal static string DeleteSql(Type type)
        {
            string columnName = string.Empty;
            PropertyInfo[] properties = type.GetProperties();

            PropertyInfo idProp = properties.SingleOrDefault(p => p.GetCustomAttribute<PrimaryKey>() != null);
            if (idProp != null)
                columnName = idProp.Name;
            else
            {
                foreach (var property in properties)
                {
                    if (property.Name.Equals("Id") || property.Name.Equals(type.Name + "Id"))
                        columnName = property.Name;
                }
            }

            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException();


            string conditionPart = $"WHERE {columnName} = @{columnName}";
            return $"DELETE {string.Concat(type.Name, "s")} {conditionPart}";
        }

        internal static string SelectSql(Type type, List<int> ids = null)
        {
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            List<string> idsStr = new List<string>();
            if (ids != null)
                for (int i = 0; i < ids.Count; i++)
                {
                    idsStr.Add(string.Concat("@Id", i));
                }
            string selectPart = string.Join(",", properties.Select(p => p.Name));
            string columnName = type.GetProperty("Id").Name;
            string joinIds = string.Join(",", idsStr);
            string conditionPart = $"WHERE {columnName} IN ( {joinIds} )";
            return $"SELECT {selectPart} FROM {string.Concat(type.Name, "s")} {(ids == null || ids.Count == 0 ? string.Empty : conditionPart)}";
        }
    }
}
