using Newtonsoft.Json;
using TaskManagement.Models;

public static class AuthorizationHelper
{
    public static bool CheckRole(IHttpContextAccessor contextAccessor, string controller, string action)
    {
        var ssRole = contextAccessor.HttpContext.Session.GetString("roles");
        List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(ssRole);

        // Tìm role tương ứng với controller và kiểm tra quyền theo action
        Role matchingRole = roles.FirstOrDefault(x => x.ModuleName == controller);

        if (matchingRole != null)
        {
            switch (action)
            {
                case "View":
                    return (bool)matchingRole.View;
                case "Add":
                    return (bool)matchingRole.Add;
                case "Edit":
                    return (bool)matchingRole.Edit;
                case "Delete":
                    return (bool)matchingRole.Delete;
                case "Comment":
                    return (bool)matchingRole.Comment;
                case "Review":
                    return (bool)matchingRole.Review;
                case "Export":
                    return (bool)matchingRole.Export;
                default:
                    return false;
            }
        }

        return false;
    }
}
