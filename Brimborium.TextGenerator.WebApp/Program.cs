

namespace Brimborium.TextGenerator.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // builder.Services.AddRazorPages();

            builder.Services.AddSingleton<TextRunController>();
            builder.Services.AddSingleton<IMinimalController, TextRunController>(sp=>sp.GetRequiredService<TextRunController>());

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            //app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Request.Path = "/index.html";
                }
                await next.Invoke();
            });

#if DEBUG
            var distRelative = @"Brimborium.TextGenerator.WebAppClient\dist\brimborium.text-generator.web-app-client";
            var location = GetDistPath(distRelative, typeof(Program).Assembly.Location);
            if (string.IsNullOrEmpty(location))
            {
                throw new Exception("DEBUG location is not found.");
            }
            var contentFileProvider = new PhysicalFileProvider(location);
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(location),
                RequestPath = "",
                ServeUnknownFileTypes = true
            });

#else
            app.UseStaticFiles();
#endif

            app.UseRouting();

            foreach (var minimalController in app.Services.GetServices<IMinimalController>()) {
                minimalController.Map(app);
            }

            app.MapFallbackToFile("index.html").WithMetadata(new HttpMethodMetadata(new[] { "GET" }));
            app.Run();
        }

        public static string? GetDistPath(string distRelative, string? location)
        {
            while (!string.IsNullOrEmpty(location))
            {
                location = Path.GetDirectoryName(location);
                if (location is null) { return null; }
                var distAbsolute = Path.Combine(location, distRelative);
                if (Directory.Exists(distAbsolute))
                {
                    return distAbsolute;
                }
            }
            return default;
        }
    }
}
