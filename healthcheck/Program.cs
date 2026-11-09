const int DOCKER_HEALTHY = 0;
const int DOCKER_UNHEALTHY = 1;
const int TIMEOUT_SECONDS = 5;

string port = args.Length > 0 ? args[0] : "8080";
string path = args.Length > 1 ? args[1] : "health";

if (!int.TryParse(port, out int portNum) || portNum < 1 || portNum > 65535)
{
    Console.WriteLine($"Configuration Error: Invalid port number '{port}'. Must be an integer between 1 and 65535.");
    return DOCKER_UNHEALTHY;
}

if (Uri.EscapeDataString(path) != path && !path.Contains('/'))
{
    Console.WriteLine($"Configuration Error: Invalid characters detected in path route '{path}'.");
    return DOCKER_UNHEALTHY;
}

path = path.Trim('/');

Uri endpoint = new($"http://localhost:{port}/{path}");

using HttpClient client = new();
client.DefaultRequestHeaders.UserAgent.ParseAdd("Docker-Compose-HealthCheck/1.0");

using CancellationTokenSource cts = new(TimeSpan.FromSeconds(TIMEOUT_SECONDS));

try
{
    HttpResponseMessage response = await client.GetAsync(endpoint, cts.Token);
    int statusCode = (int)response.StatusCode;

    if (response.IsSuccessStatusCode)
    {
        return DOCKER_HEALTHY;
    }

    Console.WriteLine($"App Unhealthy: {endpoint} -> HTTP {statusCode}");
    return DOCKER_UNHEALTHY;
}
catch (OperationCanceledException)
{
    Console.WriteLine($"App Unhealthy: Connection to {endpoint} timed out after {TIMEOUT_SECONDS} seconds.");
    return DOCKER_UNHEALTHY;
}
catch (Exception ex)
{
    Console.WriteLine($"App Unhealthy: Failed to communicate with {endpoint}. Error: {ex.Message}");
    return DOCKER_UNHEALTHY;
}