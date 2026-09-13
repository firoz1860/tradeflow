using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Services;
using TradeFlow.Contracts.Requests;
using TradeFlow.Contracts.Responses;

namespace TradeFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request.Email, request.Password, cancellationToken);
        return result.Success
            ? Ok(new AuthResponse(result.Token!, result.ExpiresAt!.Value, result.User!.Email, result.User.Role.ToString()))
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return result.Success
            ? Ok(new AuthResponse(result.Token!, result.ExpiresAt!.Value, result.User!.Email, result.User.Role.ToString()))
            : Unauthorized(new { error = result.Error });
    }
}
