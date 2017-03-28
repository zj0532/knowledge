using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IS
{
    //public class EnumItem
    //{
    //    public int Index { get; internal set; }
    //    public string Name { get; internal set; }
    //    public string Description { get; internal set; }
    //    public int Value { get; internal set; }

    //    public override string ToString()
    //    {
    //        return string.Concat("{",
    //            string.Format("Name='{0}',Value={1},Description='{2}',Index={3}",Name,Value,Description,Index)
    //            ,"}");
    //    }
    //}

    //public class EnumService
    //{
    //    private static readonly Dictionary<Type, EnumItem[]> Map = new Dictionary<Type, EnumItem[]>();

    //    public static EnumItem[] GetItems(Type enumType)
    //    {
    //        Guard.NotNull(enumType, "enumType");
    //        if (!enumType.IsEnum)
    //            throw new InvalidOperationException("Invalid enum type:" + enumType.Name);

    //        EnumItem[] items;

    //        if (!Map.TryGetValue(enumType, out items))
    //        {
    //            items = (from v in Enum.GetValues(enumType).Cast<object>()
    //                     let n = Enum.GetName(enumType, v)
    //                     let a = enumType.GetField(n).GetAttribute<EnumDescriptionAttribute>(false)
    //                     select new EnumItem
    //                     {
    //                         Name = n,
    //                         Value = Convert.ToInt32(v),
    //                         Description = a != null ? string.IsNullOrEmpty(a.Description) ? a.DefaultDescription : a.Description : null,
    //                         Index = a != null ? a.Rank : 0,
    //                     }
    //                     )
    //                     .OrderBy(p=>p.Index)
    //                     .ToArray();
    //            lock (Map)
    //                Map[enumType] = items;
    //        }

    //        return items;
    //    }

    //    public static string GetDescription<TEnum>(TEnum value)
    //    {
    //        return GetItems(typeof(TEnum)).FirstOrDefault(p => p.Value == (int)(object)value).Description;
    //    }
    //}
}
