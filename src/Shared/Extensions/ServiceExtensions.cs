using Microsoft.Extensions.DependencyInjection;
using Supabase;

namespace Shared.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddSupabaseClient(this IServiceCollection services, string? url, string? key)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentNullException(nameof(url), "Supabase URL is required");

        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key), "Supabase Key is required");

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        services.AddSingleton(new Client(url, key, options));
        return services;
    }
}
