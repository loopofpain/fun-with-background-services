using System;

namespace TestService.AuthenticationHandler
{
    public class AuthenticationHandlerException : Exception
    {
        public AuthenticationHandlerException(string message) : base(message)
        {
        }
    }
}