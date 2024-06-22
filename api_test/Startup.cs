using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace api_test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //Подключение EF r MySql
            string connection = Configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<AppContext>(options =>
                options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddMvc();
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddEndpointsApiExplorer();
            services.AddDistributedMemoryCache();






        }
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders("content-disposition")
                );
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();



            app.MapControllers();
            app.Run();


            
            


            

           



           // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        }
    }
}
