using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hrnetgroup.Wms.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hrnetgroup.Wms.Controllers.TokenAuth;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class SimpleTokenAuthController : ControllerBase
{
    private readonly TokenAuthConfiguration _tokenAuthConfiguration;
    protected IConfiguration Configuration;
    
    public SimpleTokenAuthController(IOptions<TokenAuthConfiguration> tokenAuthConfiguration, IConfiguration configuration)
    {
        _tokenAuthConfiguration = tokenAuthConfiguration.Value;
        Configuration = configuration;
    }

    [HttpGet]
    [Route("GetConfiguration")]
    public string GetConfiguration()
    {
        throw new UserFriendlyException("hello world");
        return Configuration.GetValue<string>("Test");
    }

    [HttpGet]
    [Route("GetToken")]
    public string GetToken()
    {
        var expiration = TimeSpan.FromDays(365);
        return CreateAccessToken(CreateJwtClaims(), expiration);
    }
    
    private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
    {
        var now = DateTime.Now.ToUniversalTime();
        var maxClockSkewAllowed = 10;
        var jwtSecurityToken = new JwtSecurityToken( 
            issuer: _tokenAuthConfiguration.Issuer,
            audience: _tokenAuthConfiguration.Audience,
            claims: claims,
            notBefore: now.Add(TimeSpan.FromMinutes( - maxClockSkewAllowed )),
            expires: now.Add(expiration.Value),
            signingCredentials: _tokenAuthConfiguration.SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
    
    private List<Claim> CreateJwtClaims()
    {
        var defaultClaims = new List<Claim>() {
            new Claim(ClaimTypes.Name, "hrnetgroup"),
            new Claim(JwtRegisteredClaimNames.Sub, "hrnetgroup"),
        };
        
        return defaultClaims;
    }
    
}