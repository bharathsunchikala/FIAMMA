using System;

namespace Fiamma.ApplicationCore.Exceptions;
public class RoleStillAssignedException : Exception
{
    public RoleStillAssignedException(string message) : base(message)
    {

    }
}

