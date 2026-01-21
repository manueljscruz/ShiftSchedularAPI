namespace ShiftSchedularIL
{
    public class EmailMessages
    {
        public const string CONFIRM_EMAIL_EMAIL = @"
        <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Confirm Your Email - Shift Scheduler</title>
            </head>
            <body style=""margin: 0; padding: 0; font-family: 'Roboto', 'Helvetica Neue', Arial, sans-serif; background-color: #f8f9fa;"">
                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f8f9fa; padding: 40px 20px;"">
                    <tr>
                        <td align=""center"">
                            <!-- Main Container -->
                            <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 16px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden; max-width: 100%;"">
                    
                                <!-- Header with Gradient -->
                                <tr>
                                    <td style=""background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%); padding: 40px 30px; text-align: center;"">
                                        <!-- Logo -->
                                        <div style=""margin-bottom: 20px;"">
                                            <span style=""font-size: 32px; font-weight: 700; color: #ffffff; letter-spacing: -0.5px;"">
                                                Shift<span style=""color: #C03221;"">Scheduler</span>
                                            </span>
                                            <div style=""width: 60px; height: 4px; background: linear-gradient(90deg, #C03221, #ff6b5a); border-radius: 2px; margin: 8px auto 0;""></div>
                                        </div>
                                        <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: 600;"">Confirm Your Email Address</h1>
                                    </td>
                                </tr>

                                <!-- Content -->
                                <tr>
                                    <td style=""padding: 40px 30px;"">
                                        <p style=""margin: 0 0 20px; color: #555555; font-size: 16px; line-height: 1.6;"">
                                            Hello <strong>[UserName]</strong>,
                                        </p>
                                        <p style=""margin: 0 0 20px; color: #555555; font-size: 16px; line-height: 1.6;"">
                                            Thank you for registering with Shift Scheduler! To complete your registration and start managing your shifts, please confirm your email address by clicking the button below.
                                        </p>

                                        <!-- CTA Button -->
                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin: 30px 0;"">
                                            <tr>
                                                <td align=""center"">
                                                    <a href=""[ConfirmationLink]"" style=""display: inline-block; background-color: #C03221; color: #ffffff; text-decoration: none; font-weight: 600; font-size: 16px; padding: 14px 40px; border-radius: 10px; box-shadow: 0 4px 12px rgba(192, 50, 33, 0.3); letter-spacing: 0.5px; text-transform: uppercase;"">
                                                        Confirm Email Address
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>

                                        <p style=""margin: 30px 0 20px; color: #555555; font-size: 14px; line-height: 1.6;"">
                                            If the button above doesn't work, copy and paste this link into your browser:
                                        </p>
                                        <p style=""margin: 0 0 20px; word-break: break-all;"">
                                            <a href=""[ConfirmationLink]"" style=""color: #C03221; text-decoration: none; font-size: 14px;"">[ConfirmationLink]</a>
                                        </p>

                                        <!-- Divider -->
                                        <div style=""border-top: 1px solid #e9ecef; margin: 30px 0;""></div>

                                        <p style=""margin: 0; color: #545E75; font-size: 14px; line-height: 1.6;"">
                                            This confirmation link will expire in <strong>24 hours</strong>. If you didn't create an account with Shift Scheduler, you can safely ignore this email.
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #e9ecef;"">
                                        <p style=""margin: 0 0 10px; color: #545E75; font-size: 14px;"">
                                            Need help? Contact us at <a href=""mailto:support@shiftscheduler.com"" style=""color: #C03221; text-decoration: none;"">support@shiftscheduler.com</a>
                                        </p>
                                        <p style=""margin: 0; color: #545E75; font-size: 12px;"">
                                            &copy; [Year] Shift Scheduler. All rights reserved.
                                        </p>
                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

        public const string POST_FORGOT_PASSWORD_EMAIL = @"
            <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Reset Your Password - Shift Scheduler</title>
                </head>
                <body style=""margin: 0; padding: 0; font-family: 'Roboto', 'Helvetica Neue', Arial, sans-serif; background-color: #f8f9fa;"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f8f9fa; padding: 40px 20px;"">
                        <tr>
                            <td align=""center"">
                                <!-- Main Container -->
                                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 16px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden; max-width: 100%;"">
                    
                                    <!-- Header with Gradient -->
                                    <tr>
                                        <td style=""background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%); padding: 40px 30px; text-align: center;"">
                                            <!-- Logo -->
                                            <div style=""margin-bottom: 20px;"">
                                                <span style=""font-size: 32px; font-weight: 700; color: #ffffff; letter-spacing: -0.5px;"">
                                                    Shift<span style=""color: #C03221;"">Scheduler</span>
                                                </span>
                                                <div style=""width: 60px; height: 4px; background: linear-gradient(90deg, #C03221, #ff6b5a); border-radius: 2px; margin: 8px auto 0;""></div>
                                            </div>
                                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: 600;"">Reset Your Password</h1>
                                        </td>
                                    </tr>

                                    <!-- Content -->
                                    <tr>
                                        <td style=""padding: 40px 30px;"">
                                            <p style=""margin: 0 0 20px; color: #555555; font-size: 16px; line-height: 1.6;"">
                                                Hello <strong>[UserName]</strong>,
                                            </p>
                                            <p style=""margin: 0 0 20px; color: #555555; font-size: 16px; line-height: 1.6;"">
                                                We received a request to reset the password for your Shift Scheduler account. Click the button below to create a new password.
                                            </p>

                                            <!-- CTA Button -->
                                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin: 30px 0;"">
                                                <tr>
                                                    <td align=""center"">
                                                        <a href=""[ResetPasswordLink]"" style=""display: inline-block; background-color: #C03221; color: #ffffff; text-decoration: none; font-weight: 600; font-size: 16px; padding: 14px 40px; border-radius: 10px; box-shadow: 0 4px 12px rgba(192, 50, 33, 0.3); letter-spacing: 0.5px; text-transform: uppercase;"">
                                                            Reset Password
                                                        </a>
                                                    </td>
                                                </tr>
                                            </table>

                                            <p style=""margin: 30px 0 20px; color: #555555; font-size: 14px; line-height: 1.6;"">
                                                If the button above doesn't work, copy and paste this link into your browser:
                                            </p>
                                            <p style=""margin: 0 0 20px; word-break: break-all;"">
                                                <a href=""[ResetPasswordLink]"" style=""color: #C03221; text-decoration: none; font-size: 14px;"">[ResetPasswordLink]</a>
                                            </p>

                                            <!-- Security Info Box -->
                                            <div style=""background-color: #fff3f0; border-left: 4px solid #C03221; padding: 16px; margin: 30px 0; border-radius: 4px;"">
                                                <p style=""margin: 0; color: #1a1a2e; font-size: 14px; font-weight: 600;"">
                                                    🔒 Security Note
                                                </p>
                                                <p style=""margin: 8px 0 0; color: #555555; font-size: 14px; line-height: 1.6;"">
                                                    This password reset link will expire in <strong>1 hour</strong> for security reasons.
                                                </p>
                                            </div>

                                            <!-- Divider -->
                                            <div style=""border-top: 1px solid #e9ecef; margin: 30px 0;""></div>

                                            <p style=""margin: 0; color: #545E75; font-size: 14px; line-height: 1.6;"">
                                                If you didn't request a password reset, please ignore this email or contact support if you have concerns about your account security. Your password will remain unchanged.
                                            </p>
                                        </td>
                                    </tr>

                                    <!-- Footer -->
                                    <tr>
                                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #e9ecef;"">
                                            <p style=""margin: 0 0 10px; color: #545E75; font-size: 14px;"">
                                                Need help? Contact us at <a href=""mailto:support@shiftscheduler.com"" style=""color: #C03221; text-decoration: none;"">support@shiftscheduler.com</a>
                                            </p>
                                            <p style=""margin: 0; color: #545E75; font-size: 12px;"">
                                                &copy; [Year] Shift Scheduler. All rights reserved.
                                            </p>
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
            </html>
        ";
        
    }
}
