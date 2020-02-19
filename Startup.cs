using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace jpitkonsult
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            if (CurrentEnvironment.IsProduction())
            {
                //the following line adds the automatic renewal service.
                services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions()
                {
                    Email = Configuration.GetSection("LetsEncrypt").GetSection("Email").Value, //LetsEncrypt will send you an e-mail here when the certificate is about to expire
                    UseStaging = false, //switch to true for testing
                    Domains = Configuration.GetSection("LetsEncrypt").GetSection("Domains").Get<List<string>>(),
                    TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30), //renew automatically 30 days before expiry
                    TimeAfterIssueDateBeforeRenewal = TimeSpan.FromDays(7), //renew automatically 7 days after the last certificate was issued
                    CertificateSigningRequest = new CsrInfo() //these are your certificate details
                    {
                        CountryName = Configuration.GetSection("LetsEncrypt").GetSection("CountryName").Value,
                        Locality = Configuration.GetSection("LetsEncrypt").GetSection("Locality").Value,
                        Organization = Configuration.GetSection("LetsEncrypt").GetSection("Organization").Value,
                        OrganizationUnit = Configuration.GetSection("LetsEncrypt").GetSection("OrganizationUnit").Value,
                        State = Configuration.GetSection("LetsEncrypt").GetSection("State").Value
                    }
                });

                //the following line tells the library to persist the certificate to a file, so that if the server restarts, the certificate can be re-used without generating a new one.
                services.AddFluffySpoonLetsEncryptFileCertificatePersistence();

                //the following line tells the library to persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
                services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseFluffySpoonLetsEncrypt();

            }
            else if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
