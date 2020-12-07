
using System.Linq;
using AutoMapper;
using coreAPI.Dtos.Character;
using coreAPI.Dtos.Skill;
using coreAPI.Dtos.Weapon;
using coreAPI.Models;

namespace coreAPI
{

    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill,GetSkillDto>();
            CreateMap<Character, GetCharacterDto>().ForMember(dest => dest.Skills, 
                                                              opt => opt.MapFrom(
                                                              source => source.CharacterSkills.Select(cs => cs.Skill)));
        }
    }
}