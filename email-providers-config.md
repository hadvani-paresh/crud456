# 📧 Email Provider Configurations

## 1. 🟢 Gmail (Recommended - Free)
```json
{
  "EmailSettings": {
    "Provider": "Gmail",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Your App Name",
    "AppPassword": "your-16-digit-app-password",
    "EnableSsl": true
  }
}
```
**Setup:**
- Google Account → Security → 2-Step Verification → App Passwords
- Generate app password for "Mail"
- Free: 500 emails/day

---

## 2. 🔵 Outlook/Hotmail (Free)
```json
{
  "EmailSettings": {
    "Provider": "Outlook",
    "SmtpServer": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@outlook.com",
    "SenderName": "Your App Name", 
    "AppPassword": "your-outlook-password",
    "EnableSsl": true
  }
}
```
**Setup:**
- Use regular Outlook password
- Free: 300 emails/day

---

## 3. 🟡 Yahoo Mail (Free)
```json
{
  "EmailSettings": {
    "Provider": "Yahoo",
    "SmtpServer": "smtp.mail.yahoo.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@yahoo.com",
    "SenderName": "Your App Name",
    "AppPassword": "your-app-password",
    "EnableSsl": true
  }
}
```
**Setup:**
- Yahoo Account → Security → Generate app password
- Free: 500 emails/day

---

## 4. 🚀 SendGrid (Professional - Paid)
```json
{
  "EmailSettings": {
    "Provider": "SendGrid",
    "SmtpServer": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SenderEmail": "your-verified-email@yourdomain.com",
    "SenderName": "Your App Name",
    "AppPassword": "your-sendgrid-api-key",
    "EnableSsl": true
  }
}
```
**Setup:**
- Create SendGrid account
- Verify sender email
- Generate API key
- Free: 100 emails/day, Paid: Unlimited

---

## 5. 📮 Mailgun (Professional - Paid)
```json
{
  "EmailSettings": {
    "Provider": "Mailgun",
    "SmtpServer": "smtp.mailgun.org",
    "SmtpPort": 587,
    "SenderEmail": "noreply@your-domain.com",
    "SenderName": "Your App Name",
    "AppPassword": "your-mailgun-smtp-password",
    "EnableSsl": true
  }
}
```
**Setup:**
- Create Mailgun account
- Add and verify domain
- Get SMTP credentials
- Free: 5,000 emails/month

---

## 6. 🔶 Amazon SES (Professional - Paid)
```json
{
  "EmailSettings": {
    "Provider": "AmazonSES",
    "SmtpServer": "email-smtp.us-east-1.amazonaws.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@your-domain.com",
    "SenderName": "Your App Name",
    "AppPassword": "your-ses-smtp-password",
    "EnableSsl": true
  }
}
```
**Setup:**
- AWS Account → SES
- Verify email/domain
- Create SMTP credentials
- Very cheap: $0.10 per 1,000 emails

---

## 7. 🟣 Zoho Mail (Free/Paid)
```json
{
  "EmailSettings": {
    "Provider": "Zoho",
    "SmtpServer": "smtp.zoho.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@zoho.com",
    "SenderName": "Your App Name",
    "AppPassword": "your-zoho-password",
    "EnableSsl": true
  }
}
```
**Setup:**
- Create Zoho Mail account
- Use regular password or app password
- Free: 5 users, 5GB storage

---

## 🎯 **Recommendations:**

### For Development/Testing:
- **Gmail** - Easy setup, reliable
- **Outlook** - Good alternative to Gmail

### For Production:
- **SendGrid** - Best deliverability, analytics
- **Amazon SES** - Cheapest for high volume
- **Mailgun** - Good for developers

### For Small Business:
- **Zoho Mail** - Professional email + SMTP
- **Gmail Workspace** - Google's business solution

---

## 🔧 **How to Switch Providers:**

1. Copy desired configuration to `appsettings.json`
2. Update your email credentials
3. Restart application
4. Test with your email!

**No code changes needed - just configuration! 🎉**