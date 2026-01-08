using Microsoft.Extensions.DependencyInjection;
using Supabase;

namespace Shared.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddSupabaseClient(this IServiceCollection services, string url, string key)
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        services.AddSingleton(new Client(url, key, options));
        return services;
    }
}
