using System;

namespace TheWindowsService.Test2.AuthenticationHandler
{
    public class AuthenticationHandlerException : Exception
    {
        public AuthenticationHandlerException(string message) : base(message)
        {
        }
    }
}