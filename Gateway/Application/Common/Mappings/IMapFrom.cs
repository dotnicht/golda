using AutoMapper;

namespace Binebase.Exchange.Gateway.Application.Common.Mappings
{
    public interface IMapFrom<T>
    {   
        void MappingFrom(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
