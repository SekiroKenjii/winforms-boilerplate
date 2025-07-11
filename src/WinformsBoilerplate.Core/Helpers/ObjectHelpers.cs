using System.Reflection;

namespace WinformsBoilerplate.Core.Helpers;

public static class ObjectHelpers
{
    public static bool Compare<T>(T e1, T e2) where T : class
    {
        foreach (PropertyInfo propObj1 in e1.GetType().GetProperties())
        {
            PropertyInfo? propObj2 = e2.GetType().GetProperty(propObj1.Name);

            if (propObj1.PropertyType.Name.Equals("List`1"))
            {
                dynamic? objList1 = propObj1.GetValue(e1, null);
                dynamic? objList2 = propObj2?.GetValue(e2, null);

                if (objList1?.Count != objList2?.Count)
                {
                    return false;
                }

                for (int i = 0; i < objList1?.Count; i++)
                {
                    if (!Compare(objList1[i], objList2?[i]))
                    {
                        return false;
                    }
                }
            }
            else if (propObj1.GetValue(e1, null)?.Equals(propObj2?.GetValue(e2, null)) != true)
            {
                return false;
            }
        }

        return true;
    }
}
