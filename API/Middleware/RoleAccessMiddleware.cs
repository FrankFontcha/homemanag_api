using API.Data;
using System.Security.Claims;

namespace API.Middleware
{
    public class RoleAccessMiddleware
    {
        public readonly RequestDelegate _next;
        public readonly ILogger _logger;
        public readonly IHostEnvironment _env;
        public RoleAccessMiddleware(RequestDelegate next, ILogger<RoleAccessMiddleware> logger, IHostEnvironment env)
        {
            this._env = env;
            this._logger = logger;
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {

                using (var scope = context.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    // Use the dbContext in your middleware logic
                    // ...
                    var resultCheck = this.CheckUserAccess(context, dbContext);

                    if(resultCheck) {
                        await this._next(context);
                    }else{
                        await context.Response.WriteAsync("An error occured or not have access");
                    }
                }
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync("An error occured or not have access");
            }
        }

        public bool CheckUserAccess(HttpContext context, DataContext _context)
        {
            try
            {
                ClaimsPrincipal currentUser = context.User;
                var userid = Int32.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

                var userData = _context.UserProperties.Where((x) => x.UserId == userid).FirstOrDefault();

                return true;
            }
            catch (System.Exception)
            {
                return true;
            }
        }
    }
}