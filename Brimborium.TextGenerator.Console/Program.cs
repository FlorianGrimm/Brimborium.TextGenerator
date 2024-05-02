using Microsoft.Extensions.Options;

namespace Brimborium.TextGenerator;

public sealed class Program {

    public static async Task Main(string[] args) {

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        //builder.Services.AddHostedService<Worker>();
        builder.Services.AddOptions<ApplicationConfiguration>().BindConfiguration("");
        builder.Services.AddSingleton<Program>();
        var host = builder.Build();
        
        await host.Services.GetRequiredService<Program>().MainAsync(
            host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping
            );
        /*
        var task = host.RunAsync();
        host.Services.GetRequiredService<IHostApplicationLifetime>().StopApplication();
        await task;
        */
    }


    private readonly ApplicationConfiguration _ApplicationConfiguration;

    public Program(
        IOptions<ApplicationConfiguration> applicationConfiguration) {
        this._ApplicationConfiguration = applicationConfiguration.Value;
    }

    public async Task MainAsync(CancellationToken cancellationToken) {
        System.Console.Out.WriteLine($"Input: {_ApplicationConfiguration.Input}");
        var content = await System.IO.File.ReadAllTextAsync(_ApplicationConfiguration.Input, cancellationToken);
        if (string.IsNullOrWhiteSpace(content)) {
            return;
        }
        var parser = Parser.CreateForCSharp();
        parser.Parse(content);
        await Task.CompletedTask;
    }
}
public sealed class ApplicationConfiguration {
    public string Input { get; set; } = string.Empty;
}