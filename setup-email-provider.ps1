# Email Provider Setup Script
# Run this in PowerShell to quickly setup different email providers

Write-Host "📧 Email Provider Setup for CRUD Project" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

Write-Host "`n🔹 Available Email Providers:" -ForegroundColor Yellow
Write-Host "1. Gmail (Free - Recommended for beginners)"
Write-Host "2. Outlook (Free - Microsoft)"  
Write-Host "3. Yahoo Mail (Free)"
Write-Host "4. SendGrid (Professional - Best for production)"
Write-Host "5. Mailgun (Professional)"
Write-Host "6. Amazon SES (Professional - Cheapest)"
Write-Host "7. Zoho Mail (Free/Paid)"

$choice = Read-Host "`n🎯 Select provider (1-7)"

$configs = @{
    "1" = @{
        "Provider" = "Gmail"
        "SmtpServer" = "smtp.gmail.com"
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. Go to Google Account Settings`n2. Security → 2-Step Verification → App Passwords`n3. Generate app password for 'Mail'`n4. Use that 16-digit password"
    }
    "2" = @{
        "Provider" = "Outlook"
        "SmtpServer" = "smtp-mail.outlook.com" 
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. Use your regular Outlook email and password`n2. Make sure 2FA is enabled if required"
    }
    "3" = @{
        "Provider" = "Yahoo"
        "SmtpServer" = "smtp.mail.yahoo.com"
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. Yahoo Account → Security → Generate app password`n2. Use the generated app password"
    }
    "4" = @{
        "Provider" = "SendGrid"
        "SmtpServer" = "smtp.sendgrid.net"
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. Create SendGrid account`n2. Verify sender email`n3. Generate API key`n4. Use API key as password"
    }
    "5" = @{
        "Provider" = "Mailgun"
        "SmtpServer" = "smtp.mailgun.org"
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. Create Mailgun account`n2. Add and verify domain`n3. Get SMTP credentials from dashboard"
    }
    "6" = @{
        "Provider" = "AmazonSES"
        "SmtpServer" = "email-smtp.us-east-1.amazonaws.com"
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. AWS Account → SES`n2. Verify email/domain`n3. Create SMTP credentials`n4. Use SMTP username/password"
    }
    "7" = @{
        "Provider" = "Zoho"
        "SmtpServer" = "smtp.zoho.com"
        "SmtpPort" = 587
        "EnableSsl" = $true
        "Instructions" = "1. Create Zoho Mail account`n2. Use regular password or generate app password"
    }
}

if ($configs.ContainsKey($choice)) {
    $config = $configs[$choice]
    
    Write-Host "`n✅ Selected: $($config.Provider)" -ForegroundColor Green
    Write-Host "`n📋 Setup Instructions:" -ForegroundColor Yellow
    Write-Host $config.Instructions
    
    Write-Host "`n🔧 Configuration for appsettings.json:" -ForegroundColor Cyan
    
    $senderEmail = Read-Host "Enter your sender email"
    $senderName = Read-Host "Enter sender name (e.g., 'My App')"
    $appPassword = Read-Host "Enter password/API key" -AsSecureString
    $appPasswordText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($appPassword))
    
    $jsonConfig = @"
{
  "EmailSettings": {
    "Provider": "$($config.Provider)",
    "SmtpServer": "$($config.SmtpServer)",
    "SmtpPort": $($config.SmtpPort),
    "SenderEmail": "$senderEmail",
    "SenderName": "$senderName",
    "AppPassword": "$appPasswordText",
    "EnableSsl": $($config.EnableSsl.ToString().ToLower())
  }
}
"@

    Write-Host "`n📄 Copy this to your appsettings.json:" -ForegroundColor Green
    Write-Host $jsonConfig -ForegroundColor White
    
    # Save to file
    $jsonConfig | Out-File -FilePath "email-config-$($config.Provider.ToLower()).json" -Encoding UTF8
    Write-Host "`n💾 Configuration saved to: email-config-$($config.Provider.ToLower()).json" -ForegroundColor Green
    
} else {
    Write-Host "`n❌ Invalid choice. Please run the script again." -ForegroundColor Red
}

Write-Host "`n🎉 Setup complete! Update your appsettings.json and restart the application." -ForegroundColor Green