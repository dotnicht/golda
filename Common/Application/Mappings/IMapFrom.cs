using AutoMapper;

namespace Binebase.Exchange.Common.Application.Mappings
{
    public interface IMapFrom<T>
    {   
        void MappingFrom(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
