using System.Threading.Tasks;
using coreAPI.Dtos.Character;
using coreAPI.Dtos.Weapon;
using coreAPI.Models;

namespace coreAPI.Services.WeaponService
{
    public interface IWeaponService
    {
         Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}