namespace TourCrm.Application.Settings;

public static class EmailTemplates
{
    public static string VerificationHtml(string appName, string code)
        => BaseHtml(appName, "Подтверждение email",
            $"Введите этот код в {appName}", code);

    public static string ResetHtml(string appName, string code, int minutes = 10)
        => BaseHtml(appName, "Сброс пароля",
            $"Для сброса пароля в {appName} используйте код в течение {minutes} минут:", code);

    public static string CredentialsHtml(
        string productName,
        string firstName,
        string lastName,
        string loginEmail,
        string password,
        string signInUrl)
    {
        string E(string s) => System.Net.WebUtility.HtmlEncode(s ?? "");

        return $@"
<!doctype html>
<html lang=""ru"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
  <title>Доступ в {E(productName)}</title>
</head>
<body style=""margin:0;padding:24px 12px;background:#f5f7fb;color:#111e11;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Arial,sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
    <tr><td align=""center"">
      <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600""
             style=""max-width:600px;background:#ffffff;border-radius:16px;box-shadow:0 8px 24px rgba(17,30,17,0.08);
                    border:1px solid #eef1ea;"">
        <tr>
          <td style=""padding:24px 28px;border-bottom:1px solid #eef1ea;"">
            <div style=""font-weight:700;font-size:18px;letter-spacing:.3px;color:#88926D"">{E(productName)}</div>
          </td>
        </tr>
        <tr>
          <td style=""padding:28px 28px 8px;"">
            <h1 style=""margin:0 0 8px;font-size:22px;line-height:1.35;color:#111e11"">Добро пожаловать, {E(firstName)} {E(lastName)}!</h1>
            <p style=""margin:0 0 16px;font-size:15px;line-height:1.7;color:#4b5563"">
              Вам создана учётная запись в {E(productName)}. Ниже ваши данные для входа:
            </p>
          </td>
        </tr>
        <tr>
          <td style=""padding:0 28px 8px"">
            <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0""
                   style=""background:#f8faf7;border:1px solid #eef1ea;border-radius:12px"">
              <tr>
                <td style=""padding:16px 18px;font-size:14px;line-height:1.6;color:#111e11"">
                  <div><strong>Логин (email):</strong> {E(loginEmail)}</div>
                  <div><strong>Пароль:</strong> {E(password)}</div>
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td style=""padding:16px 28px 28px"">
            <a href=""{E(signInUrl)}"" target=""_blank""
               style=""display:inline-block;background:linear-gradient(135deg,#cde374,#8ba26d);
                      color:#111e11;text-decoration:none;padding:12px 18px;border-radius:12px;
                      font-weight:600;border:1px solid rgba(206,219,149,.6)"">
              Перейти к входу
            </a>
            <p style=""margin:12px 0 0;font-size:12px;color:#6b7280"">
              По соображениям безопасности рекомендуем сменить пароль после первого входа.
            </p>
          </td>
        </tr>
        <tr>
          <td style=""padding:18px 28px;border-top:1px solid #eef1ea;color:#9ca3af;font-size:12px"">
            Если вы не ожидали это письмо, просто проигнорируйте его.
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";
    }

    private static string BaseHtml(string appName, string title, string subtitle, string code)
    {
        string accent = "#8B926D";
        string displayCode = FormatCode(code);

        return $@"<!doctype html>
<html lang=""ru"">
<head>
  <meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>{title} — {appName}</title>
</head>
<body style=""margin:0;padding:0;background:#f5f7fb;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Arial,sans-serif;color:#111e11;"">
  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background:#f5f7fb;padding:24px 12px;"">
    <tr><td align=""center"">

      <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600""
             style=""max-width:600px;background:#ffffff;border-radius:16px;box-shadow:0 8px 24px rgba(17,30,17,.08);border:1px solid #eef1ea;"">
        <tr>
          <td style=""padding:0;height:0;overflow:hidden;color:transparent;opacity:0;font-size:1px;line-height:1px;"">
            Ваш код {displayCode}.
          </td>
        </tr>
        <tr>
          <td style=""padding:24px 28px;border-bottom:1px solid #eef1ea;"">
            <div style=""font-weight:700;font-size:18px;letter-spacing:.3px;color:{accent}"">{appName}</div>
          </td>
        </tr>
        <tr>
          <td style=""padding:28px 28px 8px;"">
            <h1 style=""margin:0 0 8px;font-size:22px;line-height:1.35;color:#111e11;"">{title}</h1>
            <p style=""margin:0 0 18px;font-size:15px;line-height:1.7;color:#4b5563;"">{subtitle}</p>
          </td>
        </tr>
        <tr>
          <td align=""center"" style=""padding:8px 28px 24px;"">
            <div style=""display:inline-block;padding:14px 22px;border-radius:14px;border:1px solid #e6ecd9;
                        background:linear-gradient(135deg,#ffffff 0%,#f7faf3 100%);box-shadow:0 8px 20px rgba(206,219,149,.25);"">
              <div style=""font-size:28px;letter-spacing:4px;font-weight:800;color:#111e11;"">{displayCode}</div>
            </div>
          </td>
        </tr>
        <tr>
          <td style=""padding:0 28px 28px;"">
            <p style=""margin:0;font-size:13px;line-height:1.6;color:#6b7280;"">
              Если вы не запрашивали этот код, просто проигнорируйте письмо.
            </p>
          </td>
        </tr>
        <tr>
          <td style=""padding:16px 28px 28px;border-top:1px solid #eef1ea;color:#6b7280;font-size:12px;"">
            © {DateTime.UtcNow.Year} {appName}. Все права защищены.
          </td>
        </tr>
      </table>

    </td></tr>
  </table>
</body>
</html>";
    }

    private static string FormatCode(string raw)
    {
        var digits = new string((raw ?? string.Empty).Where(char.IsDigit).ToArray());
        return digits.Length == 6 ? $"{digits[..3]} {digits[3..]}" : digits;
    }
}