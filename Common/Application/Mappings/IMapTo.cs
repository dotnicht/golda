using AutoMapper;

namespace Binebase.Exchange.Common.Application.Mappings
{
    public interface IMapTo<T>
    {
        void MappingTo(Profile profile) => profile.CreateMap(GetType(), typeof(T));
    }
}
