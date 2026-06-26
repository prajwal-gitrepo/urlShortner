Awesome 🔥—this is the best way to learn. We’ll build a real production-style URL shortener step by step using .NET + Azure-ready design.
I’ll guide you like a mentor. You code along ✅

✅ 🚀 Step 0 — What we are building
We’ll build:

✅ ASP.NET Core Web API
✅ Shorten URL endpoint
✅ Redirect endpoint
✅ Database (initially in-memory → then SQL/Cosmos)
✅ Clean architecture (basic level)
✅ Azure-ready


✅ 🧱 Step 1 — Create the Project
Open terminal / PowerShell:
Shell1dotnet new webapi -n UrlShortener2cd UrlShortenerShow more lines
Run the app:
Shell1dotnet runShow more lines
Open:
https://localhost:xxxx/swagger

✅ Confirm it's working

✅ 🧱 Step 2 — Clean Up Default Code
Delete:

WeatherForecast.cs
WeatherForecastController.cs


✅ 🧱 Step 3 — Create Models
📁 Create folder: Models
UrlMapping.cs
C#1namespace UrlShortener.Models2{3    public class UrlMapping4    {5        public int Id { get; set; }6        public string ShortCode { get; set; }7        public string OriginalUrl { get; set; }8        public DateTime CreatedAt { get; set; }9    }10}Show more lines

✅ 🧱 Step 4 — Create DTOs (API contracts)
📁 Create folder: Dtos
CreateUrlRequest.cs
C#1namespace UrlShortener.Dtos2{3    public class CreateUrlRequest4    {5        public string Url { get; set; }6    }7}Show more lines

CreateUrlResponse.cs
C#1namespace UrlShortener.Dtos2{3    public class CreateUrlResponse4    {5        public string ShortUrl { get; set; }6    }7}Show more lines

✅ 🧱 Step 5 — Create Service Layer
📁 Folder: Services
IUrlService.cs
C#1using UrlShortener.Models;2 3namespace UrlShortener.Services4{5    public interface IUrlService6    {7        UrlMapping CreateShortUrl(string originalUrl);8        string GetOriginalUrl(string shortCode);9    }10}Show more lines

✅ Implementation
UrlService.cs
C#1using UrlShortener.Models;2 3namespace UrlShortener.Services4{5    public class UrlService : IUrlService6    {7        private static List<UrlMapping> _store = new();8        private static int _idCounter = 1;9 10        public UrlMapping CreateShortUrl(string originalUrl)11        {12            var shortCode = GenerateShortCode(_idCounter);13 14            var mapping = new UrlMapping15            {16                Id = _idCounter++,17                OriginalUrl = originalUrl,18                ShortCode = shortCode,19                CreatedAt = DateTime.UtcNow20            };21 22            _store.Add(mapping);23            return mapping;24        }25 26        public string GetOriginalUrl(string shortCode)27        {28            return _store.FirstOrDefault(x => x.ShortCode == shortCode)?.OriginalUrl;29        }30 31        private string GenerateShortCode(int id)32        {33            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";34            var result = "";35 36            while (id > 0)37            {38                result = chars[id % 62] + result;39                id /= 62;40            }41 42            return result;43        }44    }45}Show more lines
✅ This is Base62 encoding (industry approach)

✅ 🧱 Step 6 — Register Service (DI)
In Program.cs:
C#1builder.Services.AddSingleton<IUrlService, UrlService>();Show more lines

✅ 🧱 Step 7 — Create Controller
📁 Folder: Controllers
UrlController.cs
C#1using Microsoft.AspNetCore.Mvc;2using UrlShortener.Dtos;3using UrlShortener.Services;4 5namespace UrlShortener.Controllers6{7    [ApiController]8    [Route("api/url")]9    public class UrlController : ControllerBase10    {11        private readonly IUrlService _urlService;12 13        public UrlController(IUrlService urlService)14        {15            _urlService = urlService;16        }17 18        [HttpPost]19        public IActionResult CreateShortUrl([FromBody] CreateUrlRequest request)20        {21            var result = _urlService.CreateShortUrl(request.Url);22 23            var shortUrl = $"{Request.Scheme}://{Request.Host}/{result.ShortCode}";24 25            return Ok(new CreateUrlResponse26            {27                ShortUrl = shortUrl28            });29        }30    }31}Show more lines

✅ 🧱 Step 8 — Add Redirect Endpoint
Update controller:
C#1[HttpGet("/{code}")]2public IActionResult RedirectToOriginal(string code)3{4    var originalUrl = _urlService.GetOriginalUrl(code);5 6    if (originalUrl == null)7        return NotFound();8 9    return Redirect(originalUrl);10}Show more lines

✅ ✅ TEST IT (IMPORTANT)
1. POST Request (Swagger)
JSON1{2  "url": "https://google.com"3}Show more lines
✅ Output:
https://localhost:xxxx/abc123


2. Open in browser
https://localhost:xxxx/abc123

✅ It should redirect to Google

✅ 🎉 Congrats — You Built Version 1

✅ 🔥 Next Steps (We will do step-by-step next)
Say “next step” and we will upgrade this into production-grade system:
✅ Coming next:

Add Entity Framework + SQL
Add Redis Cache
Add Logging
Add Validation
Add Custom short URL
Deploy to Azure App Service


✅ ⚡ Small Challenge (Do this before next step)
Try this yourself:
✅ Add validation:

Reject empty URLs
Validate proper URL format


👉 Once done, say:
“Done, next step” 🚀
Provide your feedback on BizChatYou said: done next stepsCopilot said: CopilotAwesome 🔥—you’ve done the hardest part already. Now we’ll upgrade your basic version → production-grade step by step.

✅ 🚀 Step 9 — Add Entity Framework + SQL (Persist Data)
Right now your data is:
👉 In-memory (lost on restart ❌)
Let’s fix that ✅

🧱 9.1 Install EF Core packages
Run:
Shell1dotnet add package Microsoft.EntityFrameworkCore2dotnet add package Microsoft.EntityFrameworkCore.SqlServer3dotnet add package Microsoft.EntityFrameworkCore.ToolsShow more lines

🧱 9.2 Create DbContext
📁 Create folder: Data
AppDbContext.cs
C#1using Microsoft.EntityFrameworkCore;2using UrlShortener.Models;3 4namespace UrlShortener.Data5{6    public class AppDbContext : DbContext7    {8        public AppDbContext(DbContextOptions<AppDbContext> options)9            : base(options)10        {11        }12 13        public DbSet<UrlMapping> UrlMappings { get; set; }14    }15}Show more lines

🧱 9.3 Configure DB in Program.cs
C#1using UrlShortener.Data;2using Microsoft.EntityFrameworkCore;3 4builder.Services.AddDbContext<AppDbContext>(options =>5    options.UseSqlServer("Your_Connection_String"));Show more lines
👉 For now, you can use local SQL Server:
Server=localhost;Database=UrlShortenerDb;Trusted_Connection=True;


🧱 9.4 Run Migration
Shell1dotnet ef migrations add InitialCreate2dotnet ef database updateShow more lines
✅ Your DB is ready

✅ 🚀 Step 10 — Update Service to Use DB
Replace in-memory logic ❌
Updated UrlService
C#1using UrlShortener.Data;2using UrlShortener.Models;3 4namespace UrlShortener.Services5{6    public class UrlService : IUrlService7    {8        private readonly AppDbContext _context;9 10        public UrlService(AppDbContext context)11        {12            _context = context;13        }14 15        public UrlMapping CreateShortUrl(string originalUrl)16        {17            var entity = new UrlMapping18            {19                OriginalUrl = originalUrl,20                CreatedAt = DateTime.UtcNow21            };22 23            _context.UrlMappings.Add(entity);24            _context.SaveChanges();25 26            entity.ShortCode = GenerateShortCode(entity.Id);27            _context.SaveChanges();28 29            return entity;30        }31 32        public string GetOriginalUrl(string shortCode)33        {34            return _context.UrlMappings35                .FirstOrDefault(x => x.ShortCode == shortCode)?.OriginalUrl;36        }37 38        private string GenerateShortCode(int id)39        {40            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";41            var result = "";42 43            while (id > 0)44            {45                result = chars[id % 62] + result;46                id /= 62;47            }48 49            return result;50        }51    }52}Show more lines

✅ 🔥 Important Change
In Program.cs:
C#1builder.Services.AddScoped<IUrlService, UrlService>();Show more lines
👉 Why Scoped now?

Because DbContext is scoped

✅ You are applying DI best practice

✅ 🚀 Step 11 — Add Redis Cache (Huge Performance Boost)
This is what makes your app production-level ⚡

🧱 11.1 Install Redis package
Shell1dotnet add package Microsoft.Extensions.Caching.StackExchangeRedisShow more lines

🧱 11.2 Configure Redis
C#1builder.Services.AddStackExchangeRedisCache(options =>2{3    options.Configuration = "localhost:6379"; // Azure Redis later4});Show more lines

🧱 11.3 Update Service
Inject cache:
C#1using Microsoft.Extensions.Caching.Distributed;Show more lines
C#1private readonly IDistributedCache _cache;2 3public UrlService(AppDbContext context, IDistributedCache cache)4{5    _context = context;6    _cache = cache;7}Show more lines

✅ Modify GetOriginalUrl
C#1public string GetOriginalUrl(string shortCode)2{3    var cached = _cache.GetString(shortCode);4 5    if (cached != null)6        return cached;7 8    var url = _context.UrlMappings9        .FirstOrDefault(x => x.ShortCode == shortCode)?.OriginalUrl;10 11    if (url != null)12    {13        _cache.SetString(shortCode, url,14            new DistributedCacheEntryOptions15            {16                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)17            });18    }19 20    return url;21}Show more lines

✅ 🔥 What you just built

✅ DB persistence
✅ Caching layer
✅ Scalable design

👉 This is real production architecture

✅ 🚀 Step 12 — Add Logging (Interview MUST)
In controller:
C#1private readonly ILogger<UrlController> _logger;2 3public UrlController(IUrlService urlService, ILogger<UrlController> logger)4{5    _urlService = urlService;6    _logger = logger;7}Show more lines
Use it:
C#1_logger.LogInformation("Creating short URL for {Url}", request.Url);Show more lines

✅ 🚀 Step 13 — Add Validation (Clean Code)
Install:
Shell1dotnet add package FluentValidation.AspNetCoreShow more lines

CreateUrlRequestValidator.cs
C#1using FluentValidation;2using UrlShortener.Dtos;3 4public class CreateUrlRequestValidator : AbstractValidator<CreateUrlRequest>5{6    public CreateUrlRequestValidator()7    {8        RuleFor(x => x.Url)9            .NotEmpty()10            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))11            .WithMessage("Invalid URL");12    }13}Show more lines

✅ 🚀 Step 14 — Ready for Azure Deployment
Now your app is:
✅ API + DB
✅ Cache
✅ Logging
✅ Validation

✅ Azure Mapping

























ComponentAzure ServiceAPIApp ServiceDBAzure SQLCacheAzure Redis CacheSecretsKey Vault

✅ 🎯 What You Achieved
👉 You didn’t just code — you built:
✅ Real backend service
✅ Scalable architecture
✅ Interview-level project
✅ Resume-worthy project 🔥

✅ 🚀 Next Upgrade (Advanced Level)
If you want to go next level, we can add:

✅ Custom short URLs (/prajwal)
✅ Expiry links
✅ Click analytics (Azure Queue + Functions)
✅ Rate limiting
✅ Azure deployment (step-by-step)


✅ Your Next Task
Try:
👉 Move connection string to appsettings.json
👉 Add .env style config (best practice)

👉 Then tell me:
“Next level features” 😄
We’ll make this resume + GitHub standout project 🚀
Provide your feedback on BizChatYou said: CreateUrlRequestValidator.cs under which folderCopilot said: CopilotGood question 👍—this is about clean project structure, which interviewers care about.

✅ ✅ Where to place CreateUrlRequestValidator.cs
👉 Create a new folder:
📁 Validators
Your project structure should look like:
UrlShortener/
│
├── Controllers/
├── Models/
├── Dtos/
├── Services/
├── Data/
├── Validators/   ✅ (create this)
│     └── CreateUrlRequestValidator.cs


✅ ✅ Why Validators folder?

Keeps validation logic separate from DTOs
Follows clean architecture principles
Makes project scalable and maintainable

👉 In larger apps, you'll have many validators → this keeps things organized.

✅ ✅ Full Code (with namespace)
Make sure namespace matches:
C#1using FluentValidation;2using UrlShortener.Dtos;3 4namespace UrlShortener.Validators5{6    public class CreateUrlRequestValidator : AbstractValidator<CreateUrlRequest>7    {8        public CreateUrlRequestValidator()9        {10            RuleFor(x => x.Url)11                .NotEmpty()12                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))13                .WithMessage("Invalid URL");14        }15    }16}Show more lines

✅ ✅ Register Validator (VERY IMPORTANT)
In Program.cs, add:
C#1builder.Services.AddFluentValidationAutoValidation();2builder.Services.AddValidatorsFromAssemblyContaining<CreateUrlRequestValidator>();Show more lines

✅ 🔥 Pro Tip (Interview-level answer)
If interviewer asks:

Where do you put validation logic?

👉 Say:

“I keep validation in a separate Validators folder using FluentValidation to maintain separation of concerns and keep controllers clean.”


✅ You're doing great 🚀
Next we can:
👉 Add Custom short URLs (/prajwal)
👉 OR Expiry links + analytics (real production feature)
Just tell me: "custom url" or "analytics" 👍
Provide your feedback on BizChatYou said: using FluentValidation;
using UrlShortener.Dtos;

public class CreateUrlRequestValidator : AbstractValidator<CreateUrlRequest>
{
    public CreateUrlRequestValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid URL");
    }
}When does this work we have not called this functionCopilot said: CopilotExcellent question 👏 — this shows you’re thinking like a good backend engineer.
You’re absolutely right to ask:

“We never called this validator — so how does it run?”


✅ ✅ How FluentValidation Works (Important Concept)
👉 The validator is NOT called manually
👉 It is triggered automatically by ASP.NET Core pipeline

🔷 🔥 What actually happens behind the scenes
When you added:
C#1builder.Services.AddFluentValidationAutoValidation();2builder.Services.AddValidatorsFromAssemblyContaining<CreateUrlRequestValidator>();Show more lines
You told ASP.NET:

✅ “Whenever a request comes with a model, run validation automatically before hitting the controller”


🔷 ✅ Execution Flow
Step 1: Request comes in
JSON1POST /api/url2{3  "url": ""4}Show more lines

Step 2: Model binding happens
ASP.NET converts JSON → CreateUrlRequest

Step 3: FluentValidation kicks in ✅
It finds:
C#1CreateUrlRequestValidatorShow more lines
And runs:
C#1RuleFor(x => x.Url)Show more lines

Step 4: If validation fails ❌
ASP.NET stops execution before controller
It automatically returns:
JSON1{2  "errors": {3    "Url": ["Invalid URL"]4  }5}Show more lines
⚠️ Your controller is never reached

Step 5: If validation passes ✅
Only then:
C#1CreateShortUrl(...)Show more lines
is executed

✅ ✅ That’s why you don’t see a function call
👉 Because it’s part of the middleware pipeline

✅ ✅ How to PROVE it's working (important)
Test this in Swagger/Postman:
❌ Case 1: Invalid
JSON1{2  "url": ""3}Show more lines
✅ You should get:
400 Bad Request


✅ Case 2: Valid
JSON1{2  "url": "https://google.com"3}Show more lines
✅ Works normally

✅ 🔥 Optional (Advanced — Interview Edge)
If you want manual control, you can do:
C#1var validator = new CreateUrlRequestValidator();2var result = validator.Validate(request);3 4if (!result.IsValid)5    return BadRequest(result.Errors);Show more lines
👉 But ❌ not recommended in ASP.NET Core
👉 Because automatic pipeline is cleaner

✅ 🔥 Interview-Level Explanation
If asked:

“How does FluentValidation work?”

Say:

“FluentValidation integrates with ASP.NET Core model binding pipeline. Validators are automatically executed during model binding, and if validation fails, the request is short-circuited with a 400 response before reaching the controller.”


✅ ✅ Common Mistake (Check this)
If validation is NOT working, usually you missed:
C#1builder.Services.AddFluentValidationAutoValidation();Show more lines

✅ 🚀 You are doing very well
You’re now thinking about:

✅ Middleware pipeline
✅ Framework internals
✅ Clean architecture

👉 This is exactly what interviewers want at 5 yrs level ✅

✅ Next step (your upgrade path)
Pick one:
👉 "custom url" → allow /prajwal style links
👉 "analytics" → track clicks (real production level 🔥)
Provide your feedback on BizChatYou said: give me Rate limiting & Azure deployment (step-by-step)Copilot said: CopilotPerfect 🔥 — this is next-level production engineering. I’ll guide you step-by-step for:

✅ Rate Limiting (protect your API)
✅ Azure Deployment (real-world setup)


✅ 🚀 PART 1 — Rate Limiting (ASP.NET Core)
We’ll implement IP-based rate limiting to prevent abuse.

🧱 Step 1 — Install Package
Shell1dotnet add package AspNetCoreRateLimitShow more lines

🧱 Step 2 — Add Config to appsettings.json
JSON1"IpRateLimiting": {2  "EnableEndpointRateLimiting": true,3  "StackBlockedRequests": false,4  "RealIpHeader": "X-Real-IP",5  "ClientIdHeader": "X-ClientId",6  "HttpStatusCode": 429,7  "GeneralRules": [8    {9      "Endpoint": "*",10      "Period": "1m",11      "Limit": 2012    }13  ]14}Show more lines
👉 Meaning:

Max 20 requests per minute per IP


🧱 Step 3 — Register Services (Program.cs)
C#1using AspNetCoreRateLimit;2 3builder.Services.AddMemoryCache();4builder.Services.Configure<IpRateLimitOptions>(5    builder.Configuration.GetSection("IpRateLimiting"));6 7builder.Services.AddInMemoryRateLimiting();8builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();Show more lines

🧱 Step 4 — Add Middleware
C#1app.UseIpRateLimiting();Show more lines
⚠️ Add it before app.MapControllers()

✅ ✅ Test It
Call API repeatedly (Postman / Swagger):
👉 After limit → you get:
429 Too Many Requests


🔥 Interview Tip
Say:

“I implemented rate limiting at API level using middleware to protect from abuse and DDoS-like traffic.”


✅ 🚀 PART 2 — Azure Deployment (Step-by-Step)
We’ll deploy your app to Azure App Service.

✅ 🧱 Step 1 — Prepare Project
✅ Ensure:

appsettings.json has no hardcoded secrets
Move DB connection string to:

JSON1"ConnectionStrings": {2  "DefaultConnection": ""3}Show more lines

✅ 🧱 Step 2 — Install Azure CLI
If not installed:
Shell1az --versionShow more lines
If not → install from Microsoft site

✅ 🧱 Step 3 — Login to Azure
Shell1az loginShow more lines

✅ 🧱 Step 4 — Create Resource Group
Shell1az group create --name urlshortener-rg --location centralindiaShow more lines

✅ 🧱 Step 5 — Create App Service Plan
Shell1az appservice plan create \2  --name urlshortener-plan \3  --resource-group urlshortener-rg \4  --sku B1Show more lines

✅ 🧱 Step 6 — Create Web App
Shell1az webapp create \2  --resource-group urlshortener-rg \3  --plan urlshortener-plan \4  --name your-unique-app-name \5  --runtime "DOTNET:8"Show more lines

✅ 🧱 Step 7 — Deploy Your App
From your project root:
Shell1dotnet publish -c ReleaseShow more lines
Then:
Shell1cd bin/Release/net8.0/publish2zip -r app.zip .Show more lines
Deploy:
Shell1az webapp deployment source config-zip \2  --resource-group urlshortener-rg \3  --name your-unique-app-name \4  --src app.zipShow more lines

✅ ✅ Access Your App
https://your-unique-app-name.azurewebsites.net

🎉 Your API is LIVE

✅ 🧱 Step 8 — Configure Environment Variables
Set DB connection string:
Shell1az webapp config appsettings set \2  --resource-group urlshortener-rg \3  --name your-unique-app-name \4  --settings ConnectionStrings__DefaultConnection="your-db-connection"Show more lines

✅ 🧱 Step 9 — Add Azure SQL Database
Shell1az sql server create \2  --name your-sql-server \3  --resource-group urlshortener-rg \4  --location centralindia \5  --admin-user adminuser \6  --admin-password YourPassword123!Show more lines
Shell1az sql db create \2  --resource-group urlshortener-rg \3  --server your-sql-server \4  --name urlshortenerdb \5  --service-objective S0Show more lines

✅ 🧱 Step 10 — Add Redis Cache (Production)
Create via Portal or CLI:
Shell1az redis create \2  --name urlshortenercache \3  --resource-group urlshortener-rg \4  --location centralindia \5  --sku Basic \6  --vm-size c0Show more lines
👉 Update your Program.cs:
C#1options.Configuration = "your-redis-connection-string";Show more lines

✅ 🔥 Final Architecture (What you built)
User
 ↓
Azure Front Door (optional)
 ↓
App Service (.NET API)
 ↓
 ├── Azure SQL (storage)
 ├── Redis Cache (fast lookup)
 └── Rate Limiting (middleware)


✅ 🎯 What You Achieved
You now have:
✅ Production-grade API
✅ Rate limiting (security)
✅ Cloud deployment
✅ Scalable architecture
✅ Resume-ready project 🔥

✅ 🚀 Next Level (Highly Recommended)
If you want to go top-tier engineer level, next we can add:

✅ Custom short URLs (/prajwal)
✅ Click tracking (Azure Functions + Queue)
✅ API Gateway (Azure API Management)
✅ CI/CD pipeline (Azure DevOps)
