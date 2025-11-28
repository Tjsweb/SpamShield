using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace InstaHyreSDETest.Constants
{
    public class InstaHyreTokens
    {
        public const string Issuer = "InstaHyre";

        public const string Audience = "Appuser";

        public const string Key = "sde1 test of InstaHyre - JWT20110125";  //expecting an error, just to point something out

        public const string AuthScheme = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}
