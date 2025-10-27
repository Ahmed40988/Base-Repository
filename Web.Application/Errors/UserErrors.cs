

namespace Web.Application.Errors
{
    public static class UserErrors
    {
       
        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Invalid email or password.", StatusCodes.Status401Unauthorized);

        public static readonly Error UserNotFound =
            new("User.NotFound", "No user found with the given email or ID.", StatusCodes.Status404NotFound);

        public static readonly Error DuplicatedEmail =
            new("User.DuplicatedEmail", "Another user with the same email already exists.", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "Email is not confirmed yet.", StatusCodes.Status401Unauthorized);

        public static readonly Error PasswordMismatch =
            new("User.PasswordMismatch", "New password and confirmation do not match.", StatusCodes.Status400BadRequest);

        public static readonly Error ResetPasswordFailed =
            new("User.ResetPasswordFailed", "Failed to reset the password.", StatusCodes.Status400BadRequest);

        public static readonly Error RegisterFailed =
            new("User.RegisterFailed", "Failed to register new user.", StatusCodes.Status400BadRequest);

        public static readonly Error DisabledUser =
    new("User.DisabledUser", "Disabled User ,Please Contact your Administrator ", StatusCodes.Status401Unauthorized);

        public static readonly Error LockedUser =
            new("User.LockedUser", "Locked User ,Please Contact your Administrator ", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);


        public static readonly Error InvalidOTP =
            new("User.InvalidOTP", "Invalid OTP code. Please enter the correct code.", StatusCodes.Status401Unauthorized);

        public static readonly Error OTPExpired =
            new("User.OTPExpired", "OTP expired or not found. Please request a new one.", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedConfirmation =
            new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);
        public static readonly Error UnexpectedServerError =
            new("Server.Error", "An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);


        public static readonly Error UpdateFailed =
            new("User.UpdateFailed", "Failed to update user information.", StatusCodes.Status400BadRequest);

        public static readonly Error DeleteFailed =
            new("User.DeleteFailed", "Failed to delete user.", StatusCodes.Status400BadRequest);

        public static readonly Error LockFailed =
            new("User.LockFailed", "Failed to lock user account.", StatusCodes.Status400BadRequest);

        public static readonly Error UnlockFailed =
            new("User.UnlockFailed", "Failed to unlock user account.", StatusCodes.Status400BadRequest);

        
        public static readonly Error None =
            Error.None;


    
}
}
