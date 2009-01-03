using System;
using Cfixctl;

namespace Cfix.Control
{
    public interface ITestItem
    {
        String Name { get; }
        uint Ordinal { get; }
    }

    public interface ITestItemContainer : ITestItem
    {
        ITestItem GetItem( uint ordinal );
        uint ItemCount { get; }
    }
    
}
