using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Application.Common
{
    public static class FluentValiationMessage
    {
        public static readonly Func<string, string, string> MUST_BE_GREATER_THAN =
            (param1, param2) => $"[{param1}] [mustBeGreaterThan] [{param2}]";

        public static readonly Func<string, string, string> MUST_BE_GREATER_THAN_OR_EQUAL =
            (param1, param2) => $"[{param1}] [mustBeGreaterThanOrEqual] [{param2}]";

        public static readonly Func<string, string, string> MUST_BE_BETWEEN =
            (param1, param2) => $"[mustBeBetween] [{param1}] [and] [{param2}]";

        public static readonly Func<string, string, string> MUST_BE_SMALLER_THAN_OR_EQUAL =
            (param1, param2) => $"[{param1}] [mustBeSmallerThanOrEqual] [{param2}]";

        public static readonly Func<string, string, string> MUST_BE_SMALLER_THAN =
            (param1, param2) => $"[{param1}] [mustBeSmallerThan] [{param2}]";

        public static readonly Func<string, string, string> MUST_BE_EQUAL_TO =
            (param1, param2) => $"[{param1}] [mustBeEqualTo] [{param2}]";

        public static readonly Func<string, string> MUST_BE_NUMERIC_DATA = (param) => $"[{param}] [mustBeNumericData]";

        public static readonly Func<string, string> DUPLICATED_DATA = (param) => $"[{param}] [isDuplicated]";

        public static readonly Func<string, string> IS_NOT_VALID = (param) => $"[{param}] [isNotValid]";

        public static readonly Func<string, string> CANNOT_BE_EMPTY = (param) => $"[{param}] [canNotBeEmpty]";

        public static readonly Func<string, string, string> LENGTH_MUSY_BE_EQUAL_TO =
            (param1, param2) => $"[{param1}] [lengthMustBeEqualTo] [{param2}]";

        public static readonly Func<string, string> CANNOT_BE_INSERT_WITH = (param) => $"[canNotBeInsertWith] [{param}]";

        public static readonly Func<string, string, string> ONE_OF_THESE_FIELD_SHOULD_FILL =
            (param1, param2) => $"[atLeastOneFieldShouldSet] ([{param1},{param2}])";
    }

    public static class ConsumerMessage
    {
        public static readonly Func<string, string> CREATE_SUCCESSFULLY = (param) => $"[{param}][createSuccessfully";

        public static readonly Func<string, string> DELETE_SUCCESSFULLY = (param) => $"[{param}][deleteSuccessfully]";

        public static readonly Func<string, string> UPDATE_SUCCESSFULLY = (param) => $"[{param}][updateSuccessfully]";

        public static readonly Func<string, string> GET_SUCCESSFULLY = (param) => $"[{param}][fetchSuccessfully]";

        public static readonly Func<string, string> GET_PAGINATED_SUCCESSFULLY =
            (param) => $"[{param}][fetchPaginatedSuccessfully]";

        public static readonly Func<string, string> NOTFOUND = (param) => $"[{param}] [notFound]";

        public static readonly Func<string, string> DUPLICATED = (param) => $"[{param}] [thereIsDuplicate]";

        public static readonly Func<string> ACCESS_DENDIED = () => $"[theUserHasNoSuitablePermissionToCallThisAction]";

        public static readonly Func<string, string>
            ACCESS_RESTRICT = (param) => $"[youHaveBeenRestricted, reason:] [{param}]";
    }

    public static class ConstraintErrorMessage
    {
        public static readonly Func<string, string> KEY_IS_NOT_VALID = param => $"[{param}][isNotValid]";

        public static readonly Func<string, string> KEY_ALREADY_USED = param => $"[{param}][alreadyUsed]";

        public static readonly Func<string, string> KEY_NOT_FOUND = param => $"[{param}][notFound]";

        public static readonly Func<string, string> KEY_CAN_NOT_BE_DUPLICATE = param => $"[{param}][canNotBeDuplicate]";
    }

    public enum ConsumerStatusCode
    {
        Success = 200,
        Update = 201,
        UnAuthorized = 401,
        AccessRestrict = 403,
        Conflict = 409,
        NotFound = 404,
        BadRequest = 400
    }
}
