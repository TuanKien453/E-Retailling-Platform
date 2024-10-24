using Microsoft.EntityFrameworkCore;

namespace E_Retalling_Portal.Models.Query
{
    public static class SettingQuery
    {
        public static IQueryable<Setting> getSettingByID(this DbSet<Setting> dbSetting, int id) 
        {
            return dbSetting.Where(s => s.id == id);
        }

        public static IQueryable<Setting> getAllSettings(this DbSet<Setting> dbSetting)
        {
            return dbSetting;
        }

    }
}
