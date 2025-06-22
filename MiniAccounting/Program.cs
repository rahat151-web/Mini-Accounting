using MiniAccounting.Data;
using MiniAccounting.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AccountsRepository>();
builder.Services.AddScoped<ManageUsersRepository>();
builder.Services.AddScoped<VoucherRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<AuthorizationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var _repository = scope.ServiceProvider.GetRequiredService<ManageUsersRepository>();

    if (!await _repository.UserExistsAsync("admin")) 
    {
        await _repository.CreateUserAsync(
            username: "admin",
            email: "admin@accountingbd.com",
            password: "Admin@1234",
            phoneNumber: "0123456789",
            roleName: "Admin"
        );
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
