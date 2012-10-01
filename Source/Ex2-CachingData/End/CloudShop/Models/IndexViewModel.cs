using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudShop.Models
{
    public class IndexViewModel
    {
        public IEnumerable<string> Products { get; set; }
        public bool IsCacheEnabled { get; set; }
        public bool IsLocalCacheEnabled { get; set; }
        public long ElapsedTime { get; set; }
        public string ObjectId { get; set; }
        public string InstanceId
        {
            get
            {
                return Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.CurrentRoleInstance.Id;
            }
        }
    }
}