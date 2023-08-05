var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GreetingHandler>();

var app = builder.Build();

app.MapGet("/", async (GreetingHandler handler, HttpContext context) => await handler.HandleAsync(context));


await app.RunAsync();


class GreetingHandler
{
  private readonly string[] _greetings = new string[] { "Привет!", "Здравствуй!", "Приветики", "Хай" };
  private readonly ILogger<GreetingHandler> _logger;

    public GreetingHandler(ILogger<GreetingHandler> logger)
    {
        _logger = logger;
    }

    private const string VisitsCookieName = "visits";
  public Task<IResult> HandleAsync(HttpContext context)
  {
    Console.WriteLine(context.Request.Cookies.ContainsKey(VisitsCookieName));
    return Task.FromResult(
      context.Request.Cookies.ContainsKey(VisitsCookieName)
        ? HandleAuthorized(context)
        : HandleNonAuthorized(context)
    );
  }

  private IResult HandleNonAuthorized(HttpContext context)
  {
    _logger.LogInformation("Non-authorized request from {Ip}", context.Connection.RemoteIpAddress);
    context.Response.Cookies.Append(VisitsCookieName, "0");
    var greeting = _greetings[Random.Shared.Next(_greetings.Length)];
    return Results.Ok($"""
          {greeting}
          Кажется, ты ещё не был здесь :)
          """);
  }

  private IResult HandleAuthorized(HttpContext context)
  {
    string cookie = context.Request.Cookies[VisitsCookieName]!;
    if (!int.TryParse(cookie, out var visits)) {
      _logger.LogInformation("User from {Ip} tried to change cookie...", context.Connection.RemoteIpAddress);
      context.Response.Cookies.Delete(VisitsCookieName);
      return Results.BadRequest("Зачем вы изменяли то, что не нужно?... Мы всё сбросили, теперь всё по-новой");
    }
    _logger.LogInformation("User from {Ip} visited {Visited} times", context.Connection.RemoteIpAddress, visits);
    context.Response.Cookies.Append(VisitsCookieName, (visits + 1).ToString());
    return Results.Ok($"Хей! Вы были тут уже {visits} раз(-а)");
  }
}