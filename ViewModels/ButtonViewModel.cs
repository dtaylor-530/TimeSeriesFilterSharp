using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Filter.Utility;
using System.Linq.Expressions;

namespace Filter.ViewModel
{



    public class ButtonDefinitionFactory
    {
        public static IEnumerable<ButtonDefinition> Build<T>(Action<T> tr, Type t, params object[] parameters)
        {
            foreach (var m in Helper.LoadMethods<T>(t, parameters))
            {
                yield return new ButtonDefinition
                {
                    Content = m.Key,
                    Command = new RelayCommand(() => tr(m.Value()))

                };
            }
        }
        //public static IEnumerable<ButtonDefinition> Build<T>(Action<T> tr, Type t, params object[] parameters)
        //{
        //    foreach (var m in Helper.LoadMethods<T>(t, parameters))
        //    {
        //        yield return new ButtonDefinition
        //        {
        //            Content = m.Key,
        //            Command = new RelayCommand<T>((a) => m.Value())

        //        };
        //    }
        //}

    }


    public class ButtonDefinition
    {
        public string Content { get; set; }
        public RelayCommand Command { get; set; }
    }

    //public class ButtonDefinition
    //{
    //    public string Content { get; set; }
    //    public ReactiveCommand Command { get; set; }
    //}




    public class Helper
    {

        public static IEnumerable<KeyValuePair<string, Func<T>>> LoadMethods<T>(Type t, object[] parameters)
        {
            return Assembly.GetAssembly(t)
                 //.GetType(nameof(Filter)+"."+nameof(Filter.ViewModel)+"."+nameof(Filter.ViewModel.VMFactory))
                 .GetType(t.FullName)
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                       .Select(_ => new KeyValuePair<string, Func<T>>(_.Name, () => (T)_.Invoke(null, parameters)));

        }



        public static Func<T, R> GetInstanceMethod<T, R>(MethodInfo method)
        {
            //ParameterExpression x = Expression.Parameter(typeof(T), "it");
            return Expression.Lambda<Func<T, R>>(
                Expression.Call(null, method), null).Compile();
        }

        public static IEnumerable<Type> GetInheritingTypes(Type type)
        {
            var x =GetSolutionAssemblies();

            var sf = x.SelectMany(sd =>  sd.GetExportedTypes() );

            var v = sf.Where(p => p.GetInterfaces().Any(t => t == type));

            return v;

            }


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
                    if(!reference.FullName.Contains("Deedle"))
                    if (list.Add(reference.FullName))
                            stack.Push(Assembly.Load(reference));



            }

        }
        //var type = typeof(IMyInterface);

    }

}
