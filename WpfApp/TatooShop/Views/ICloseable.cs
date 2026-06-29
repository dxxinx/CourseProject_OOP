using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatooShop.Views
{
    internal interface ICloseable
    {
        event EventHandler CloseRequest;
    }
}
