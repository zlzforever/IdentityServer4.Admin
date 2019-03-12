using System;

namespace IdentityServer4.Admin
{
    public class IdentityServer4AdminException : Exception
    {
        public IdentityServer4AdminException(string msg) : base(msg)
        {
        }
    }
}