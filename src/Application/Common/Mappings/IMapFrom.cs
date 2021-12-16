using AutoMapper;

namespace Messaging.Application.Common.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);


        profile.CreateMap(typeof(T), GetType());
    }
}
