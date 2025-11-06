using Microsoft.AspNetCore.Identity.UI.Services;

namespace SoloUnClick.web.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // TODO: Implementar envío de email real con SendGrid, SMTP, etc.
        // Por ahora solo loguear para desarrollo
        Console.WriteLine($"Email enviado a: {email}");
        Console.WriteLine($"Asunto: {subject}");
        Console.WriteLine($"Mensaje: {htmlMessage}");
        
        return Task.CompletedTask;
    }
}
