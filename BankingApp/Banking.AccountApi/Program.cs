using System;
using System.Security.Principal;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

var init_accounts = new[]
{
    new Account() { Index = 0, Number = "CG084548", Name = "Checking", Balance = 1232},
    new Account() { Index = 1, Number = "SA193473", Name = "Saving 1", Balance = 10000 },
    new Account() { Index = 2, Number = "SA204233", Name = "Holiday Saving", Balance = 2300 }
};

var memCacheAccountsKey = $"account-list";

app.MapGet("/accounts", (IMemoryCache memoryCache) =>
{
    Account  [] accounts;

    if (!memoryCache.TryGetValue(memCacheAccountsKey, out accounts))
    {
        memoryCache.Set(memCacheAccountsKey, init_accounts);
    }

    init_accounts = memoryCache.Get<Account[]>(memCacheAccountsKey);

    return init_accounts;
});

app.MapPost("/account/transfer", (AccountTransfer accountTransfer, IMemoryCache memoryCache) =>
{
    Account[] myaccounts;

    if (accountTransfer != null)
    {
        myaccounts = memoryCache.Get<Account[]>(memCacheAccountsKey);

        lock (myaccounts[accountTransfer.FromIndex])
        {
            Thread.Sleep(2000);

            lock (myaccounts[accountTransfer.ToIndex])
            {
                myaccounts[accountTransfer.FromIndex].Balance -= accountTransfer.Amount;
                myaccounts[accountTransfer.ToIndex].Balance += accountTransfer.Amount;
            }
        }

        memoryCache.Set(memCacheAccountsKey, myaccounts);
    }

    return Results.Ok();
});

app.MapGet("/account/history/{index}", (string index) =>
{

    return Results.Ok();
});

app.Run();

internal record Account()
{
    public int Index { get; set; }
    public string Number { get; set; }
    public string Name { get; set; } 
    public decimal Balance { get; set; }
}

internal record AccountTransfer
{
    public int FromIndex { get; set; }

    public int ToIndex { get; set; }

    public decimal Amount { get; set; }
}