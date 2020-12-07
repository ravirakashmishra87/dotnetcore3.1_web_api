using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using coreAPI.Data;
using coreAPI.Dtos.Character;
using coreAPI.Dtos.Weapon;
using coreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace coreAPI.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WeaponService(DataContext dataContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._dataContext = dataContext;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _dataContext.Characters.
                                            FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                                                                c.User.Id == int.Parse(
                                                                    _httpContextAccessor.HttpContext.User.
                                                                    FindFirstValue(ClaimTypes.NameIdentifier)));

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }
                else
                {

                    Weapon weapon = new Weapon
                    {
                        Name = newWeapon.Name,
                        Damage = newWeapon.Damage,
                        Character = character
                    };
                    await _dataContext.Weapons.AddAsync(weapon);
                    await _dataContext.SaveChangesAsync();
                    response.Data = _mapper.Map<GetCharacterDto>(character);
                    response.Message="Character weapon added successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

            }
            return response;
        }
    }
}