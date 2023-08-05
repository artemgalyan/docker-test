class GreetingHandler
{
  private readonly string[] _greetings = new string[] { "Привет!", "Здравствуй!", "Приветики", "Хай" };
  private readonly ILogger<GreetingHandler> _logger;

  public GreetingHandler(ILogger<GreetingHandler> logger)
  {
    _logger = logger;
  }

  private const string VisitsCookieName = "visits";
  public IResult Handle(HttpContext context)
  {
    return context.Request.Cookies.ContainsKey(VisitsCookieName)
        ? HandleAuthorized(context)
        : HandleNonAuthorized(context);
  }

  private IResult HandleNonAuthorized(HttpContext context)
  {
    _logger.LogInformation("Non-authorized request from {Ip}", context.Connection.RemoteIpAddress);
    context.Response.Cookies.Append(VisitsCookieName, "1");
    var greeting = _greetings[Random.Shared.Next(_greetings.Length)];
    return Results.Ok($"""
          {greeting}
          Кажется, ты ещё не был здесь :)
          """);
  }

  private IResult HandleAuthorized(HttpContext context)
  {
    string cookie = context.Request.Cookies[VisitsCookieName]!;
    if (!int.TryParse(cookie, out var visits))
    {
      _logger.LogInformation("User from {Ip} tried to change cookie...", context.Connection.RemoteIpAddress);
      context.Response.Cookies.Delete(VisitsCookieName);
      return Results.BadRequest("Зачем вы изменяли то, что не нужно?... Мы всё сбросили, теперь всё по-новой");
    }
    _logger.LogInformation("User from {Ip} visited {Visited} times", context.Connection.RemoteIpAddress, visits);
    context.Response.Cookies.Append(VisitsCookieName, (visits + 1).ToString());
    return Results.Ok($"Хей! Вы были тут уже {visits} раз(-а)");
  }
}