using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class UserService(IUnitOfWork uow) : IUserService
{
    public async Task<ServiceResult<List<UserDto>>> GetAllAsync(CancellationToken ct)
    {
        var users = await uow.Users.GetAllAsync(ct);
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            LastName = u.LastName,
            FirstName = u.FirstName,
            MiddleName = u.MiddleName,
            PhoneNumber = u.PhoneNumber,
            IsEmailConfirmed = u.IsEmailConfirmed,
            FullName = u.FullName
        }).ToList();

        return ServiceResult<List<UserDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(int id, CancellationToken ct)
    {
        var u = await uow.Users.GetByIdAsync(id, ct);
        if (u is null) return ServiceResult<UserDto>.Fail("Пользователь не найден");

        var dto = new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            LastName = u.LastName,
            FirstName = u.FirstName,
            MiddleName = u.MiddleName,
            PhoneNumber = u.PhoneNumber,
            IsEmailConfirmed = u.IsEmailConfirmed,
            FullName = u.FullName
        };

        return ServiceResult<UserDto>.Ok(dto);
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        var existingByEmail = await uow.Users.GetByEmailAsync(dto.Email, ct);
        if (existingByEmail is not null) return ServiceResult<int>.Fail("Email уже используется");

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            var existingByPhone = await uow.Users.GetByPhoneAsync(dto.PhoneNumber!);
            if (existingByPhone is not null) return ServiceResult<int>.Fail("Телефон уже используется");
        }

        var user = new User
        {
            Email = dto.Email.Trim(),
            LastName = dto.LastName!,
            FirstName = dto.FirstName!,
            MiddleName = dto.MiddleName,
            PhoneNumber = dto.PhoneNumber!
        };

        await uow.Users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        return ServiceResult<int>.Ok(user.Id, "Создано");
    }

    public async Task<ServiceResult<string>> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct)
    {
        var u = await uow.Users.GetByIdAsync(id, ct);
        if (u is null) return ServiceResult<string>.Fail("Пользователь не найден");

        if (dto.PhoneNumber != null && dto.PhoneNumber != u.PhoneNumber)
        {
            var exists = await uow.Users.GetByPhoneAsync(dto.PhoneNumber);
            if (exists is not null && exists.Id != id) return ServiceResult<string>.Fail("Телефон уже используется");
        }

        u.LastName = dto.LastName ?? u.LastName;
        u.FirstName = dto.FirstName ?? u.FirstName;
        u.MiddleName = dto.MiddleName ?? u.MiddleName;
        u.PhoneNumber = dto.PhoneNumber ?? u.PhoneNumber;
        if (dto.IsEmailConfirmed.HasValue) u.IsEmailConfirmed = dto.IsEmailConfirmed.Value;

        uow.Users.Update(u);
        await uow.SaveChangesAsync(ct);

        return ServiceResult<string>.Ok("Обновлено");
    }

    public async Task<ServiceResult<string>> DeleteAsync(int id, CancellationToken ct)
    {
        var u = await uow.Users.GetByIdAsync(id, ct);
        if (u is null) return ServiceResult<string>.Fail("Пользователь не найден");

        uow.Users.Delete(u);
        await uow.SaveChangesAsync(ct);

        return ServiceResult<string>.Ok("Удалено");
    }

    public async Task<ServiceResult<UserDto>> GetByEmailAsync(string email, CancellationToken ct)
    {
        var u = await uow.Users.GetByEmailAsync(email, ct);
        if (u is null) return ServiceResult<UserDto>.Fail("Не найдено");

        var dto = new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            LastName = u.LastName,
            FirstName = u.FirstName,
            MiddleName = u.MiddleName,
            PhoneNumber = u.PhoneNumber,
            IsEmailConfirmed = u.IsEmailConfirmed,
            FullName = u.FullName
        };

        return ServiceResult<UserDto>.Ok(dto);
    }

    public async Task<ServiceResult<UserDto>> GetByPhoneAsync(string phone, CancellationToken ct)
    {
        var u = await uow.Users.GetByPhoneAsync(phone);
        if (u is null) return ServiceResult<UserDto>.Fail("Не найдено");

        var dto = new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            LastName = u.LastName,
            FirstName = u.FirstName,
            MiddleName = u.MiddleName,
            PhoneNumber = u.PhoneNumber,
            IsEmailConfirmed = u.IsEmailConfirmed,
            FullName = u.FullName
        };

        return ServiceResult<UserDto>.Ok(dto);
    }
}