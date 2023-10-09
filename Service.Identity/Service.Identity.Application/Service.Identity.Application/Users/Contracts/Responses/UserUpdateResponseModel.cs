﻿using Service.Identity.Application.Common;
using Service.Identity.Domain.Users;

namespace Service.Identity.Application.Users.Contracts;

public class UserUpdateResponseModel : IMapping<User>, IContract
{
    public long Id { get; set; }
}