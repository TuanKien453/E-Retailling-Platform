using E_Retalling_Portal.Models;

namespace E_Retalling_Portal.Services
{
    //class 
    public class RolePermissionService
    {
        private Dictionary<string, List<Permission>> rolePermissionMappings = new Dictionary<string, List<Permission>>
    {
        { "Admin", new List<Permission>
               {
                   Permission.ViewCategoryList,
               }
        },

    };
        /// <summary>
        /// Checks if a role has a specific permission.
        /// </summary>
        /// <param name="role">The role to check permission for.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the role has the specified permission, false otherwise.</returns>
        public bool HasPermission(string role, Permission permission)
        {
            if (role == null)
            {
                role = "Guest";
            }
            if (rolePermissionMappings.ContainsKey(role))
            {
                return rolePermissionMappings[role].Contains(permission);
            }
            return false;
        }
    }
}
