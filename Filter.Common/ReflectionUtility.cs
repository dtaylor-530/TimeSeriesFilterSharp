using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FilterSharp.Common
{
    public class TypeHelper
    {
        public static T GetInstance<T>(Type type, params double[] a)
        {
            object[] x = a.Select(_ => (object)_).ToArray();
            return (T)Activator.CreateInstance(type, x);

        }


        public static IEnumerable<Type> GetInheritingTypes(Type type)
        {
            var x = AssemblyHelper.GetSolutionAssemblies();

            var sf = x.SelectMany(sd => sd.GetExportedTypes());

            var v = sf.Where(p => p.GetInterfaces().Any(t => t == type));

            return v;

        }

    }

    public static class AttributeHelper
    {


        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetDescription(this Type value)
        {


            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])value.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        public static Type[] GetTypesInNamespace(this Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }


        //        Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "MyNamespace");
        //for (int i = 0; i<typelist.Length; i++)
        //{
        //    Console.WriteLine(typelist[i].Name);
        //}

        public static IEnumerable<Type> FilterSharpByCategoryAttribute(this IEnumerable<Type> types, string category)
        {

            return types.Where(_ =>
            {
                var ca = _.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault();
                return ca == null ? false :
                ((CategoryAttribute)ca).Category.Equals(category, StringComparison.OrdinalIgnoreCase);
            });


        }


        public static IEnumerable<KeyValuePair<string, Type>> ToKeyValuePairs(IEnumerable<Type> types)
        {

            return types.Select(_ => new KeyValuePair<string, Type>(_.ToString(), _));

        }




    }

    public static class AssemblyHelper
    {

        public static IEnumerable<Assembly> GetSolutionAssemblies()
        {
            var list = new HashSet<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            while (stack.Count > 0)
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!reference.FullName.Contains("Deedle"))
                        if (list.Add(reference.FullName))
                            stack.Push(Assembly.Load(reference));



            }


        }

    }
}
