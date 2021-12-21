﻿namespace Domain.Common.Results
{
    public class Result : IResult
    {
        protected Result() { }

        public bool Failed => !Succeeded;

        public string Message { get; protected set; }

        public bool Succeeded { get; protected set; }

        public static IResult Fail() => new Result { Succeeded = false };

        public static IResult Fail(string message) => new Result { Succeeded = false, Message = message };

        public static IResult Success() => new Result { Succeeded = true };

        public static IResult Success(string message) => new Result { Succeeded = true, Message = message };
    }

    public class Result<T> : Result, IResult<T>
    {
        protected Result() { }

        public T Data { get; private init; }

        public new static IResult<T> Fail() => new Result<T> { Succeeded = false };

        public new static IResult<T> Fail(string message) => new Result<T> { Succeeded = false, Message = message };

        public new static IResult<T> Success() => new Result<T> { Succeeded = true };

        public new static IResult<T> Success(string message) => new Result<T> { Succeeded = true, Message = message };

        public static IResult<T> Success(T data) => new Result<T> { Succeeded = true, Data = data };

        public static IResult<T> Success(T data, string message) => new Result<T> { Succeeded = true, Data = data, Message = message };
    }
}
