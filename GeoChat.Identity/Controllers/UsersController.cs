using AutoMapper;
using GeoChat.Identity.Api.Dtos;
using GeoChat.Identity.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoChat.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public UsersController(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _userManager.FindByIdAsync(id);
        if (result == null) 
        {
            return NotFound();
        }
        var mappedResult = _mapper.Map<UserResponseDto>(result);
        return Ok(mappedResult);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var results = await _userManager.Users.ToListAsync();
        var mappedResults = _mapper.Map<IEnumerable<UserResponseDto>>(results);
        return Ok(mappedResults);
    }

}
