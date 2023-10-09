﻿using Service.Identity.Application.Common;
using Service.Identity.Domain.Roles;

namespace Service.Identity.Application.Roles.Contracts;

public class RoleUpdateResponseModel : IMapping<Role>, IContract
{
    public long Id { get; set; }
}