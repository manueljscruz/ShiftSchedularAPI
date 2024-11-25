namespace ShiftSchedularIL
{
    public class EmailMessages
    {
        public const string POST_REGISTRATION_EMAIL = @"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    margin: 0;
                    padding: 0;
                }
                .container {
                    padding: 20px;
                    max-width: 600px;
                    margin: auto;
                    background-color: #f9f9f9;
                    border: 1px solid #ddd;
                    border-radius: 8px;
                }
                .header {
                    text-align: center;
                    padding: 10px 0;
                }
                .header h1 {
                    color: #4CAF50;
                }
                .content {
                    margin-top: 20px;
                }
                .button {
                    display: inline-block;
                    margin: 20px 0;
                    padding: 10px 20px;
                    background-color: #4CAF50;
                    color: white;
                    text-decoration: none;
                    border-radius: 5px;
                    font-size: 16px;
                }
                .footer {
                    margin-top: 20px;
                    font-size: 14px;
                    color: #888;
                    text-align: center;
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Welcome to Shift Schedular!</h1>
                </div>
                <div class='content'>
                    <p>Hi [Name],</p>
                    <p>Thank you for registering with us. Please confirm your email address to complete the registration process.</p>
                    <a href='[Url]/confirm-email/[WorkerId]' class='button'>Confirm Email</a>
                    <p>If you did not register for this account, please ignore this email.</p>
                </div>
                <div class='footer'>
                    <p>Need help? Contact us at <a href='mailto:support@shiftschedular.com'>support@shiftschedular.com</a>.</p>
                    <p>&copy; 2024 Shift Schedular. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

        public const string POST_FORGOT_PASSWORD_EMAIL = @"

        ";
        
    }
}
