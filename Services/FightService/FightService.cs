using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coreAPI.Data;
using coreAPI.Dtos.Fight;
using coreAPI.Models;
using coreAPI.Services.CharacterService;
using Microsoft.EntityFrameworkCore;

namespace coreAPI.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _dbContext;

        public FightService(DataContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };
            try
            {
                List<Character> characters =
                    await _dbContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        Character opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                            damage = DoSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                _dbContext.Characters.UpdateRange(characters);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _dbContext.Characters
           .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
           .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                Character opponent = await _dbContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                // CharacterSkill characterSkill = await _dbContext.CharacterSkills.FirstOrDefaultAsync(
                //                                     cs => cs.CharacterId == request.AttackerId &&
                //                                     cs.SkillId == request.SkillId);

                CharacterSkill characterSkill = attacker.CharacterSkills.FirstOrDefault(cs => cs.SkillId == request.SkillId);

                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill.";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, characterSkill);

                if (damage > 0)
                    opponent.HitPoints -= (int)damage;

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                _dbContext.Characters.Update(opponent);
                await _dbContext.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Exception : {ex.Message} and Inner Exception : {ex.InnerException}";
            }
            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);
            if (damage > 0)
                opponent.HitPoints -= (int)damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _dbContext.Characters.
                                                        Include(c => c.Weapon).
                                                        FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                Character opponent = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                //int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));

                int damage = DoWeaponAttack(attacker, opponent);
                damage -= new Random().Next(opponent.Defense);
                if (damage > 0)
                    opponent.HitPoints -= (int)damage;
                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                _dbContext.Characters.Update(opponent);
                await _dbContext.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occured. Error: {ex.Message} Inner Exception : {ex.InnerException}";
            }
            return response;

        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);
            if (damage > 0)
                opponent.HitPoints -= (int)damage;
            return damage;
        }
    }
}