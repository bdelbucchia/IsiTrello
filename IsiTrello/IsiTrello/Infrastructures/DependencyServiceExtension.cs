using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsiTrello.Infrastructures
{
    public class DependencyServiceExtension : IMarkupExtension
    {
        public DependencyFetchTarget FetchTarget { get; set; }
        public Type Type { get; set; }


        public DependencyServiceExtension()
        {
            FetchTarget = DependencyFetchTarget.GlobalInstance;
        }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Type == null)
                throw new InvalidOperationException("Type argument mandatory for DependencyService extension");

            // DependencyService.Get<T>();
            var mi = typeof(DependencyService).GetTypeInfo().GetDeclaredMethod("Get");
            var cmi = mi.MakeGenericMethod(Type);
            return cmi.Invoke(null, new object[] { FetchTarget });
        }
    }
}
