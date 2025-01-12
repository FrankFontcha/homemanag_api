using System.Security.Claims;
using API.Commons;
using API.Data;
using API.DTOs.Roles;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Policy = "IsUser")]
    [Route("/api/roles")]
    public class RolesController : BaseApiController
    {
        private readonly DataContext _context;
        private IMapper _mapper;
        private RolesCommon _rolesCommon;
        private BusinessCommon _businessCommon;

        public RolesController(DataContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
            this._rolesCommon = new RolesCommon(context);
            this._businessCommon = new BusinessCommon(context);
        }

        [HttpPost("add")]
        public async Task<ActionResult<RoleResultDtos>> CreateRoles(CreateRolesDto data)
        {
            try
            {
                if (await this._rolesCommon.RoleExist(data.Title, data.BusinessId, 0)) return BadRequest("Roles Name already in used");

                if(!await this._businessCommon.BusinessIdExist(data.BusinessId)) return NotFound("Please Provide a Valid Business ID");

                var _role = this._mapper.Map<AppRole>(data);

                _role.Code = Math.Abs(data.Title.GetHashCode())+"$"+data.BusinessId;
                _role.Business = this._businessCommon.GetBusinessById(data.BusinessId);
                _role.Status = (int)StatusEnum.enable;

                this._context.Roles.Add(_role);

                await this._context.SaveChangesAsync();

                var result = this._mapper.Map<RoleResultDtos>(_role);

                return result;
            }
            catch (System.Exception)
            {
                return BadRequest("An error occured. Role can't be created");
            }
        }

        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<RoleResultDtos>>> GetAllRoles(int skip = 0, int limit = 0, string sort = "asc")
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var userId = Int32.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

                var userBusiness = this._businessCommon.GetUserBusiness(userId);

                var query = this._context.Roles.Where(x => x.Status != (int)StatusEnum.delete && userBusiness.Contains(x.BusinessId));

                if (sort == "desc")
                {
                    var _result = await query.OrderByDescending(x => x.CreatedAt).Skip(skip).Take(limit).ToListAsync();
                    var result = this._mapper.Map<IEnumerable<RoleResultDtos>>(_result);
                    return Ok(result);
                }
                else
                {
                    var _result = await query.OrderBy(x => x.CreatedAt).Skip(skip).Take(limit).ToListAsync();
                    var result = this._mapper.Map<IEnumerable<RoleResultDtos>>(_result);
                    return Ok(result);
                }
            }
            catch (System.Exception)
            {
                return BadRequest("An error occured. Please retry later");
            }
        }
        
        [HttpGet("getall/{businessid}")]
        public async Task<ActionResult<IEnumerable<RoleResultDtos>>> GetAllRoles(int businessid)
        {
            try
            {
                var _role = await this._context.Roles.Where(x => x.Status != (int)StatusEnum.delete && x.BusinessId == businessid)
                .Include(p => p.Business).ToListAsync();

                if(_role == null) return NotFound("Role Not Found or Invalid Business ID");

                var result = this._mapper.Map<IEnumerable<RoleResultDtos>>(_role);

                return Ok(result);
            }
            catch (System.Exception)
            {
                return BadRequest("An error occured or role not found");
            }
        }

        [HttpGet("{roleid}")]
        public async Task<ActionResult<RoleResultDtos>> GetOneRoles(int roleid)
        {
            try
            {
                var _role = await this._context.Roles.Where(x => x.Id == roleid && x.Status == (int)StatusEnum.enable)
                .Include(p => p.Business).FirstOrDefaultAsync();
                
                if(_role == null) return NotFound("Please Provide a Valid role ID");

                var result = this._mapper.Map<RoleResultDtos>(_role);
                return result;
            }
            catch (System.Exception)
            {
                return BadRequest("An error occured or role not found");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<DeleteRoleResultDto>> DeleteRoles(int id)
        {
            try
            {
                var _access = await this._context.Roles.Where(x => x.Id == id && x.Status != (int)StatusEnum.delete).FirstOrDefaultAsync();
                _access.Status = (int)StatusEnum.delete;
                await this._context.SaveChangesAsync();

                return new DeleteRoleResultDto
                {
                    Status = true,
                    Message = "The giving Roles has been deleted"
                };
            }
            catch (System.Exception)
            {
                return new DeleteRoleResultDto
                {
                    Status = false,
                    Message = "The giving Roles counldn't been deleted"
                };
            }
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult<RoleResultDtos>> UpdateRoles(int id, UpdateRolesDto data)
        {
            try
            {
                var _role = await this._context.Roles.Where(x => x.Id == id && x.Status != (int)StatusEnum.delete).FirstOrDefaultAsync();

                _role.Title = (data.Title != null) ? data.Title : _role.Title;
                _role.BusinessId = data.BusinessId;
                _role.Description = (data.Description != null) ? data.Description : _role.Description;
                _role.Status = (int)StatusEnum.enable;

                if (await this._rolesCommon.RoleExist(_role.Title, 1, _role.Id)) return BadRequest("Roles Name already in used");

                await this._context.SaveChangesAsync();

                var result = this._mapper.Map<RoleResultDtos>(_role);

                return result;
            }
            catch (System.Exception ex)
            {
                return BadRequest("The giving role couldn't be edited" + ex);
            }
        }
    }
}