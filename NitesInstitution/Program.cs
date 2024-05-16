using NitesInstitution;
using NitesInstitution.Data;
using NitesInstitution.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IInstitutionService, InstitutionService>();
builder.Services.AddTransient<IInstitutionRepository, InstitutionRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<ExceptionToStatusCodeMiddleware>();

app.MapGet("/PatientCases", async (IInstitutionService institutionService, HttpContext context,
     string patientIdentificationType, string patientIdentificationValue, DateTime? dateFrom, DateTime? dateTo) =>
{
    var cases = await institutionService.Execute(context.Request, patientIdentificationType, patientIdentificationValue, dateFrom, dateTo);
    return TypedResults.Ok(cases);
});

app.Run();


