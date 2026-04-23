using TourCrm.Application.DTOs.Dictionaries.Hotels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class HotelService(IHotelRepository repo) : IHotelService
{
    public async Task<IReadOnlyList<HotelDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new HotelDto(x.Id, x.Name, x.CityId, x.Stars)).ToList();

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new Hotel { Name = dto.Name.Trim(), CityId = dto.CityId, Stars = dto.Stars }, ct);
        return new HotelDto(entity.Id, entity.Name, entity.CityId, entity.Stars);
    }

    public async Task UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new Hotel { Id = id, Name = dto.Name.Trim(), CityId = dto.CityId, Stars = dto.Stars }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}