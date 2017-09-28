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
        [Description("Xem cửa hàng")]
        Store_View = 1001,
        [Description("Tạo tạo cửa hàng")]
        Store_Create = 1002,
        [Description("Tạo tạo cửa hàng")]
        Store_Deleted = 1003,

        [Description("Xem User")]
        User_View = 2001,
        [Description("Tạo user")]
        User_Create = 2002,
        [Description("Xóa user")]
        User_Delete = 2003,
        [Description("Xem danh sách vai trò")]
        GroupPermission_View = 2004,
        [Description("Tạo vai trò")]
        GroupPermission_Create = 2005,
        [Description("Xóa vai trò")]
        GroupPermission_Delete = 2006,

        [Description("View category")]
        Category_View = 3001,
        [Description("Create category")]
        Category_Create = 3002,
        [Description("Delete category")]
        Category_Delete = 3003,

        //[Description("Xem đơn vị tính")]
        //Unit_View = 3003,
        //[Description("Tạo đơn vị tính")]
        //Unit_Create = 3004,
        [Description("Xem sản phẩm")]
        Product_View = 3005,
        [Description("Tạo sản phẩm")]
        Product_Create = 3006,
        [Description("Xóa sản phẩm")]
        Product_Delete = 3007,

        [Description("Xem lệnh nhập kho")]
        WarehouseOrder_ViewImport = 7001,
        [Description("Tạo lệnh nhập kho")]
        WarehouseOrder_CreateImport = 7002,
        [Description("Xác nhận lệnh nhập kho khác")]
        WarehouseOrder_ConfirmImport = 7003,
        [Description("Xem lệnh xuất kho")]
        WarehouseOrder_ViewExport = 7004,
        [Description("Tạo lệnh xuất kho")]
        WarehouseOrder_CreateExport = 7005,

        [Description("Xem đơn hàng")]
        Order_ViewWholeSale = 12001,
  
    }
}
