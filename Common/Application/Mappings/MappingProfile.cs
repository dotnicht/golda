using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace Binebase.Exchange.Common.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
            => AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic)
                .ToList()
                .ForEach(ApplyMappingsFromAssembly);

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            ApplyMapping(assembly, typeof(IMapFrom<>), "MappingFrom");
            ApplyMapping(assembly, typeof(IMapTo<>), "MappingTo");
        }

        private void ApplyMapping(Assembly assembly, Type type, string method)
        {
            var types = assembly
                .GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type))
                .ToList();

            foreach (var t in types)
            {
                var instance = Activator.CreateInstance(t);
                var methodInfo = t.GetMethod(method) ?? t.GetInterface(type.Name).GetMethod(method);
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}