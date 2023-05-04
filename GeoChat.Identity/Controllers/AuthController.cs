using AutoMapper;
using GeoChat.Identity.Api.Dtos;
using GeoChat.Identity.Api.Entities;
using GeoChat.Identity.Api.EventBus;
using GeoChat.Identity.Api.EventBus.Events;
using GeoChat.Identity.Api.EventBus.Extensions;
using GeoChat.Identity.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeoChat.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenGenerator _tokenGenerator;
	private readonly IEventBus _eventBus;
    private readonly IConfiguration _configuration;

    public AuthController(
		IMapper mapper, SignInManager<AppUser> signInManager,
		ITokenGenerator tokenGenerator,
        IEventBus eventBus,
		IConfiguration configuration)
	{
		_mapper = mapper;
		_signInManager = signInManager;
		_tokenGenerator = tokenGenerator;
        _eventBus = eventBus;
		_configuration = configuration;
	}

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login(UserLoginDto userLogin)
	{
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }
		
		var user = await _signInManager.UserManager.FindByEmailAsync(userLogin.Email);
		if (user == null)
		{
			return BadRequest();
		}
		
		var signInResult = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, lockoutOnFailure: false);
		if (signInResult.Succeeded == false)
		{
			return BadRequest();
		}
     
		var token = await _tokenGenerator.GenerateTokenAsync(user);
		return Ok(token);	
	}


	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> Register(UserRegisterDto userRegister)
	{
		if (ModelState.IsValid == false)
		{
			return BadRequest(ModelState);
		}
		
		var user = _mapper.Map<AppUser>(userRegister);
		var result = await _signInManager.UserManager.CreateAsync(user, userRegister.Password);
		if (result.Succeeded == false) 
		{
			return BadRequest();
		}

		var mappedUser = _mapper.Map<UserResponseDto>(user);

		var newUserCreatedEvent = new NewAccountCreatedEvent()
		{
			UserId = user.Id,
			UserName = user.UserName
		};
		_eventBus.PublishNewAccountCreatedEvent(_configuration, newUserCreatedEvent);
		return Ok(mappedUser);
	}
}
