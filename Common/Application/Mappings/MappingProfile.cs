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
            ApplyMapping(assembly, typeof(IMapFrom<>));
            ApplyMapping(assembly, typeof(IMapTo<>));
        }

        private void ApplyMapping(Assembly assembly, Type type)
        {
            var method = type.GetMethods().Single().Name;

            var types = assembly
                .GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type))
                .ToList();

            foreach (var profile in types)
            {
                var instance = Activator.CreateInstance(profile);
                profile.GetMethods()
                    .Where(x => x.Name == method)
                    .Union(profile.GetInterfaces().Where(x => x.Name == type.Name).SelectMany(x => x.GetMethods().Where(y => y.Name == method)))
                    .ToList()
                    .ForEach(x => x.Invoke(instance, new object[] { this }));
            }
        }
    }
}