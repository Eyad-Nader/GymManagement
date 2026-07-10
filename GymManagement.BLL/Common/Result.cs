using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Common
{
    public sealed record Result(bool Success,string? Message = null,ResultKind  Kind = ResultKind.Ok)
    {
        public static Result Ok() => new (true);
        public static Result Fail(string? message = null, ResultKind Kind = ResultKind.Conflict) => new (false, message, Kind);
        public static Result NotFound(string error = "Not Found") => new(false, error, ResultKind.NotFound);
        public static Result Validation(string? message = null) => new(false, message, ResultKind.ValidationField);
    }

    public sealed record Result<T>(bool Success,T? Value, string? Message = null, ResultKind Kind = ResultKind.Ok)
    {
        public static Result<T> Ok(T value) => new(true , value);
        public static Result<T> Fail(string? message = null, ResultKind Kind = ResultKind.Conflict) => new(false,default, message, Kind);
        public static Result<T> NotFound(string error = "Not Found") => new(false, default, error, ResultKind.NotFound);

        //public static Result<T> Validation(string? message = null) => new(false, default, message, ResultKind.ValidationField);
    }
}
