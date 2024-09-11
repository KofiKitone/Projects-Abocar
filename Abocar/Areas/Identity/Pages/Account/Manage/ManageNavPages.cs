using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string Email => "Email";

        public static string Address => "Address";

        public static string Orders => "Orders";

        public static string Cart => "Cart";

        public static string Wishlist => "Wishlist";

        public static string ChangePassword => "ChangePassword";

        public static string DownloadPersonalData => "DownloadPersonalData";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";
        public static string Inbox => "Inbox";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);
        public static string AddressNavClass(ViewContext viewContext) => PageNavClass(viewContext, Address);
        public static string OrdersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Orders);
        public static string CartNavClass(ViewContext viewContext) => PageNavClass(viewContext, Cart);
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        public static string WishlistNavClass(ViewContext viewContext) => PageNavClass(viewContext, Wishlist);
        public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);
        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);
        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);
        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);
        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);
        public static string InboxNavClass(ViewContext viewContext) => PageNavClass(viewContext, Inbox);


        //for nav dash 
        public static string Products => "Products";
        public static string Transactions => "Transactions";
        public static string Users => "Users";
        public static string AllOrders => "Orders";
        public static string Options => "Options";
        public static string ProductsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Products);
        public static string TransactionsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Transactions);
        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);
        public static string AllOrdersNavClass(ViewContext viewContext) => PageNavClass(viewContext, AllOrders);
        public static string OptionsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Options);
        
        
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
