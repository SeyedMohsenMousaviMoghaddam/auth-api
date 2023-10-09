using AutoMapper;
using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Application.Common
{
    public interface IMap
    {
        void Mapping(Profile profile, IUserInfo userInfo)
        {
        }
    }

    public abstract class IMapping<TMap>
    {
        public virtual void Mapping(Profile profile, IUserInfo userInfo)
        {
        }
    }
}
