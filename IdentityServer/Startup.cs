using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityServerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<IdentityServerDbContext>()
                .AddDefaultTokenProviders();

            services.AddRazorPages();

            services.AddIdentityServer(options =>
                {
                    // http://docs.identityserver.io/en/release/reference/options.html#refoptions
                    options.Endpoints = new EndpointsOptions
                    {
                        // � Implicit Flow ������������ ��� ��������� �������
                        EnableAuthorizeEndpoint = true,
                        // ��� ��������� ������� ������
                        EnableCheckSessionEndpoint = true,
                        // ��� ������� �� ���������� ������������
                        EnableEndSessionEndpoint = true,
                        // ��� ��������� claims �������������������� ������������
                        // http://openid.net/specs/openid-connect-core-1_0.html#UserInfo
                        EnableUserInfoEndpoint = true,
                        // ������������ OpenId Connect ��� ��������� ����������
                        EnableDiscoveryEndpoint = true,

                        // ��� ��������� ���������� � �������, �� �� ����������
                        EnableIntrospectionEndpoint = false,
                        // ��� �� ����� �.�. � Implicit Flow access_token �������� ����� authorization_endpoint
                        EnableTokenEndpoint = false,
                        // �� �� ���������� refresh � reference tokens
                        // http://docs.identityserver.io/en/release/topics/reference_tokens.html
                        EnableTokenRevocationEndpoint = false
                    };

                    // IdentitySever ���������� cookie ��� �������� ����� ������
                    options.Authentication = new IdentityServer4.Configuration.AuthenticationOptions
                    {
                        CookieLifetime = TimeSpan.FromDays(1)
                    };
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<IdentityUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}