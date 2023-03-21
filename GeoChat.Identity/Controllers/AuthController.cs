using AutoMapper;
using GeoChat.Identity.Api.Dtos;
using GeoChat.Identity.Api.Entities;
using GeoChat.Identity.Api.MessageQueue.Events;
using GeoChat.Identity.Api.MessageQueue.Publisher;
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
    private readonly IMqPublisher _mqPublisher;

    public AuthController(
		IMapper mapper, SignInManager<AppUser> signInManager,
		ITokenGenerator tokenGenerator,
		IMqPublisher mqPublisher)
	{
		_mapper = mapper;
		_signInManager = signInManager;
		_tokenGenerator = tokenGenerator;
		_mqPublisher = mqPublisher;
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
		
		var newUserCreatedEvent = _mapper.Map<NewAccountCreatedEvent>(user);
		_mqPublisher.PublishEvent(newUserCreatedEvent);

		return Ok(mappedUser);
	}
}
