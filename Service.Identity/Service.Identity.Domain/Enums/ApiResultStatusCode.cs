using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Domain.Enums;
public enum ApiResultStatusCode
{
    [Display(Name = "Successful")] Success = 0,

    [Display(Name = "Internal Server Error")]
    ServerError = 1,

    [Display(Name = "Bad Request")] BadRequest = 2,

    [Display(Name = "Not Found Error")] NotFound = 3,

    [Display(Name = "List is empty")] ListEmpty = 4,

    [Display(Name = "Login Error")] LogicError = 5,

    [Display(Name = "Unauthorized Error")] Unauthorized = 6,

    [Display(Name = "Unprocessable Entity Error")]
    UnprocessableEntity = 7,

    [Display(Name = "Access Forbidden")] Forbidden = 8
}