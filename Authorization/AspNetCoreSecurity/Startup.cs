using AspNetCoreAuthentication.Policies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddAuthorization(options =>
            {
                // some examples
                options.AddPolicy("SalesOnly", policy =>
                {
                    policy.RequireClaim("department", "sales");
                });

                options.AddPolicy("SalesSenior", policy =>
                {
                    policy.RequireClaim("department", "sales");
                    policy.RequireClaim("status", "senior");
                });

                options.AddPolicy("DevInterns", policy =>
                {
                    policy.RequireClaim("department", "development");
                    policy.RequireClaim("status", "intern");
                });

                // custom policy
                options.AddPolicy("CxO", policy =>
                {
                    policy.RequireJobLevel(JobLevel.CxO);
                });
            });

            // register resource authorization handlers
            services.AddTransient<IAuthorizationHandler, CustomerAuthorizationHandler>();

            // register data access services
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }
}