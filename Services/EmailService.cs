using System.Net;
using System.Net.Mail;

namespace CrudProject.Services
{
    public interface IEmailService
    {
        Task<bool> SendOTPAsync(string toEmail, string otp);
        Task<bool> SendPasswordResetAsync(string toEmail, string resetToken);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendOTPAsync(string toEmail, string otp)
        {
            try
            {
                // Get email settings from configuration
                var provider = _configuration["EmailSettings:Provider"] ?? "Gmail";
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var appPassword = _configuration["EmailSettings:AppPassword"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                // Check if this is test mode (fake email or missing config)
                bool isTestMode = string.IsNullOrEmpty(senderEmail) || 
                                 string.IsNullOrEmpty(appPassword) || 
                                 senderEmail.Contains("test") || 
                                 senderEmail.Contains("example") ||
                                 appPassword == "your-app-password-here" ||
                                 appPassword == "test-password";

                if (isTestMode)
                {
                    // Test mode - just log the OTP
                    Console.WriteLine("===========================================");
                    Console.WriteLine($"📧 [TEST MODE] Email Simulation");
                    Console.WriteLine($"📧 To: {toEmail}");
                    Console.WriteLine($"📧 OTP: {otp}");
                    Console.WriteLine($"📧 Provider: {provider}");
                    Console.WriteLine($"📧 Use this OTP to verify your email!");
                    Console.WriteLine("===========================================");
                    
                    // Simulate async operation
                    await Task.Delay(500);
                    return true;
                }

                // Real email mode
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, appPassword),
                    EnableSsl = enableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Your Email Verification OTP",
                    Body = GenerateEmailTemplate(otp, senderName ?? "Your App", provider),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
                
                Console.WriteLine($"[EMAIL SENT] OTP '{otp}' successfully sent to {toEmail} via {provider}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Failed to send email: {ex.Message}");
                
                // Fallback to test mode if email fails
                Console.WriteLine("===========================================");
                Console.WriteLine($"📧 [FALLBACK MODE] Email Failed - Showing OTP");
                Console.WriteLine($"📧 To: {toEmail}");
                Console.WriteLine($"📧 OTP: {otp}");
                Console.WriteLine($"📧 Error: {ex.Message}");
                Console.WriteLine($"📧 Use this OTP to verify your email!");
                Console.WriteLine("===========================================");
                
                return true; // Return true so registration continues
            }
        }

        public async Task<bool> SendPasswordResetAsync(string toEmail, string resetToken)
        {
            try
            {
                // Get email settings from configuration
                var provider = _configuration["EmailSettings:Provider"] ?? "Gmail";
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var appPassword = _configuration["EmailSettings:AppPassword"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                // Check if this is test mode
                bool isTestMode = string.IsNullOrEmpty(senderEmail) || 
                                 string.IsNullOrEmpty(appPassword) || 
                                 senderEmail.Contains("test") || 
                                 senderEmail.Contains("example") ||
                                 appPassword == "your-app-password-here" ||
                                 appPassword == "test-password";

                if (isTestMode)
                {
                    // Test mode - just log the reset token
                    Console.WriteLine("===========================================");
                    Console.WriteLine($"🔐 [TEST MODE] Password Reset Email Simulation");
                    Console.WriteLine($"📧 To: {toEmail}");
                    Console.WriteLine($"🔑 Reset Token: {resetToken}");
                    Console.WriteLine($"📧 Provider: {provider}");
                    Console.WriteLine($"🔐 Use this token to reset your password!");
                    Console.WriteLine("===========================================");
                    
                    await Task.Delay(500);
                    return true;
                }

                // Real email mode
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, appPassword),
                    EnableSsl = enableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Password Reset Request",
                    Body = GeneratePasswordResetTemplate(resetToken, senderName ?? "Your App", provider),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);
                
                Console.WriteLine($"[EMAIL SENT] Password reset token sent to {toEmail} via {provider}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Failed to send password reset email: {ex.Message}");
                
                // Fallback to test mode
                Console.WriteLine("===========================================");
                Console.WriteLine($"🔐 [FALLBACK MODE] Password Reset Email Failed");
                Console.WriteLine($"📧 To: {toEmail}");
                Console.WriteLine($"🔑 Reset Token: {resetToken}");
                Console.WriteLine($"📧 Error: {ex.Message}");
                Console.WriteLine($"🔐 Use this token to reset your password!");
                Console.WriteLine("===========================================");
                
                return true;
            }
        }

        private string GeneratePasswordResetTemplate(string resetToken, string senderName, string provider)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #f5f5f5; padding: 20px;'>
                    <div style='background-color: white; padding: 30px; border-radius: 15px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                        
                        <!-- Header -->
                        <div style='text-align: center; margin-bottom: 30px;'>
                            <div style='background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%); color: white; padding: 20px; border-radius: 10px; margin-bottom: 20px;'>
                                <h1 style='margin: 0; font-size: 28px;'>🔐 Password Reset</h1>
                                <p style='margin: 10px 0 0 0; opacity: 0.9;'>Reset your account password</p>
                            </div>
                        </div>

                        <!-- Main Content -->
                        <div style='text-align: center;'>
                            <p style='font-size: 16px; color: #333; margin-bottom: 25px;'>
                                We received a request to reset your password. Use the following reset code:
                            </p>
                            
                            <!-- Reset Token Box -->
                            <div style='background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%); padding: 25px; border-radius: 12px; margin: 25px 0; display: inline-block;'>
                                <div style='background-color: white; padding: 20px; border-radius: 8px;'>
                                    <h1 style='color: #ff6b6b; margin: 0; font-size: 36px; letter-spacing: 8px; font-weight: bold; font-family: monospace;'>{resetToken}</h1>
                                </div>
                            </div>
                        </div>

                        <!-- Instructions -->
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 25px 0;'>
                            <h3 style='color: #495057; margin-top: 0;'>📋 Instructions:</h3>
                            <ul style='color: #6c757d; line-height: 1.6; margin: 0; padding-left: 20px;'>
                                <li>Enter this reset code on the password reset page</li>
                                <li>This code will expire in <strong>15 minutes</strong></li>
                                <li>Create a new secure password</li>
                                <li>Do not share this code with anyone</li>
                                <li>If you didn't request this, please ignore this email</li>
                            </ul>
                        </div>

                        <!-- Security Notice -->
                        <div style='background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <p style='margin: 0; color: #721c24; font-size: 14px;'>
                                🛡️ <strong>Security Notice:</strong> If you didn't request a password reset, 
                                someone may be trying to access your account. Please secure your account immediately.
                            </p>
                        </div>

                        <!-- Footer -->
                        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                        <div style='text-align: center;'>
                            <p style='color: #999; font-size: 12px; margin: 0;'>
                                This is an automated email. Please do not reply.<br>
                                Sent from <strong>{senderName}</strong> via {provider}
                            </p>
                            <p style='color: #ccc; font-size: 11px; margin: 10px 0 0 0;'>
                                © 2024 {senderName}. All rights reserved.
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateEmailTemplate(string otp, string senderName, string provider)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #f5f5f5; padding: 20px;'>
                    <div style='background-color: white; padding: 30px; border-radius: 15px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                        
                        <!-- Header -->
                        <div style='text-align: center; margin-bottom: 30px;'>
                            <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; margin-bottom: 20px;'>
                                <h1 style='margin: 0; font-size: 28px;'>🔐 Email Verification</h1>
                                <p style='margin: 10px 0 0 0; opacity: 0.9;'>Secure your account with OTP</p>
                            </div>
                        </div>

                        <!-- Main Content -->
                        <div style='text-align: center;'>
                            <p style='font-size: 16px; color: #333; margin-bottom: 25px;'>
                                Thank you for registering! Please use the following OTP to verify your email address:
                            </p>
                            
                            <!-- OTP Box -->
                            <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 25px; border-radius: 12px; margin: 25px 0; display: inline-block;'>
                                <div style='background-color: white; padding: 20px; border-radius: 8px;'>
                                    <h1 style='color: #667eea; margin: 0; font-size: 42px; letter-spacing: 12px; font-weight: bold; font-family: monospace;'>{otp}</h1>
                                </div>
                            </div>
                        </div>

                        <!-- Instructions -->
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 25px 0;'>
                            <h3 style='color: #495057; margin-top: 0;'>📋 Instructions:</h3>
                            <ul style='color: #6c757d; line-height: 1.6; margin: 0; padding-left: 20px;'>
                                <li>Enter this OTP on the verification page</li>
                                <li>This OTP will expire in <strong>10 minutes</strong></li>
                                <li>Do not share this OTP with anyone</li>
                                <li>If you didn't request this, please ignore this email</li>
                            </ul>
                        </div>

                        <!-- Security Notice -->
                        <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <p style='margin: 0; color: #856404; font-size: 14px;'>
                                🛡️ <strong>Security Notice:</strong> We will never ask for your OTP via phone or email. 
                                Only enter your OTP on our official website.
                            </p>
                        </div>

                        <!-- Footer -->
                        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                        <div style='text-align: center;'>
                            <p style='color: #999; font-size: 12px; margin: 0;'>
                                This is an automated email. Please do not reply.<br>
                                Sent from <strong>{senderName}</strong> via {provider}
                            </p>
                            <p style='color: #ccc; font-size: 11px; margin: 10px 0 0 0;'>
                                © 2024 {senderName}. All rights reserved.
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}