using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAzureStore.Models
{
    public class IndexViewModel
    {
        public IEnumerable<string> Products { get; set; }
    }
}