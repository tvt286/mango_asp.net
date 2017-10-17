using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Data.Enums
{
  
    public enum Permission
    {
        [Description("View store")]
        Store_View = 1001,
        [Description("View/Edit store")]
        Store_Create = 1002,
        [Description("Delete store")]
        Store_Deleted = 1003,

        // nhân viên
        [Description("View user")]
        User_View = 2001,
        [Description("Create/Edit user")]
        User_Create = 2002,
        [Description("Delete user")]
        User_Delete = 2003,
        [Description("View role")]
        GroupPermission_View = 2004,
        [Description("Create/Edit role")]
        GroupPermission_Create = 2005,
        [Description("Delete role")]
        GroupPermission_Delete = 2006,

        [Description("View category")]
        Category_View = 3001,
        [Description("Create/Edit category")]
        Category_Create = 3002,
        [Description("Delete category")]
        Category_Delete = 3003,

       
        [Description("View product")]
        Product_View = 4001,
        [Description("Create/Edit product")]
        Product_Create = 4002,
        [Description("Delete product")]
        Product_Delete = 4003,

        [Description("View order import")]
        StoreOrder_ViewImport = 5001,
        [Description("Create order import")]
        StoreOrder_CreateImport = 5002,
        [Description("Confirm order import")]
        StoreOrder_ConfirmImport = 5003,
        [Description("View order export")]
        StoreOrder_ViewExport = 5004,
        [Description("Create order export")]
        StoreOrder_CreateExport = 5005,

        [Description("View order")]
        Order_View = 6001,
        [Description("Create order")]
        Order_Create = 6002,
        [Description("Confirm order")]
        Order_Confirm = 6003,

        [Description("View warehouse")]
        Warehouse_View = 7001,
        [Description("View/Edit warehouse")]
        Warehouse_Create = 7002

    }
}
