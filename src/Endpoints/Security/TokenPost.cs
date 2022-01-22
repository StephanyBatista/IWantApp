namespace IWantApp.Endpoints.Security;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action; 

    [AllowAnonymous]
    public static async Task<IResult> Action(
        LoginRequest loginRequest, 
        IConfiguration configuration, 
        UserManager<IdentityUser> userManager, 
        ILogger<TokenPost> log,
        IWebHostEnvironment environment)
    {
        log.LogInformation("Getting token");

        var user = await userManager.FindByEmailAsync(loginRequest.Email);
        if(user == null)
            Results.BadRequest();
        if (!await userManager.CheckPasswordAsync(user, loginRequest.Password))
            Results.BadRequest();

        var claims = await userManager.GetClaimsAsync(user);
        var subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, loginRequest.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        });
        subject.AddClaims(claims);

        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = configuration["JwtBearerTokenSettings:Audience"],
            Issuer = configuration["JwtBearerTokenSettings:Issuer"],
            Expires = environment.IsDevelopment() || environment.IsStaging() ? 
                DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(2)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new
        {
            token = tokenHandler.WriteToken(token)
        });
    }
}
