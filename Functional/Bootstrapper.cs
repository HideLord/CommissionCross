using Autofac;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross
{
    public static class Bootstrapper
    {
        public static IContainer Container { get; set; }

        public static void Init()
        {
            var builder = new ContainerBuilder();

            //string servicesAssemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Cross.Services.dll");
            //Assembly assembly = Assembly.LoadFile(servicesAssemblyPath);

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            //builder.RegisterAssemblyTypes(assembly)
                //.AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf();

            Container = builder.Build();
        }
    }
}
