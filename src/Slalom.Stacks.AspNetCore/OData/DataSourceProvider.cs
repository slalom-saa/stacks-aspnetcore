using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Slalom.Stacks.AspNetCore.OData
{
    public class DataSourceProvider
    {
        public static IEdmModel GetEdmModel(string dataSourceName)
        {
            return GetDataSource(dataSourceName).Model;
        }

        public static IDataSource GetDataSource(string dataSourceName)
        {
            return new StacksDataSource();
        }
    }
}
