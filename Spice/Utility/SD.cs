using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
	//---SD - Static Details: Used as a helper for const repository
    public static class SD
    {
        
        public const string DefaultFoodImage = "default_food.png";
        //---MXV: Adding roles for registration
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";
        //---For session
        public const string ssShoppingCartCount = "ssCartCount";
		public const string ssCouponCode = "ssCouponCode";
		//---Convert HTML to Raw HTML

		//---Order status
		public const string StatusSubmitted = "Submitted";
		public const string StatusInProcess = "Being Prepared";
		public const string StatusReady = "Ready for Pickup";
		public const string StatusCompleted = "Completed";
		public const string StatusCancelled = "Cancelled";

		//---Status for payment
		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";



		public static string ConvertToRawHtml(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		//---Get the type of coupon used (15%/20%), calculate the discounted price
		public static double DiscountedPrice(Coupon couponFromDb, double OriginalOrderTotal)
        {
			if(couponFromDb == null)
            {
				//---no coupon, keeps original price
				return OriginalOrderTotal;
            } else
            {
				//---check if min amount is met
				if(couponFromDb.MinimumAmount > OriginalOrderTotal)
                {
					return OriginalOrderTotal;
                } else
                {
					//---everything is valid
					//---checks if it is money or % discount
					if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Dollar)
                    {
						//---$10 off $100
						//---Rounded to the next number
						return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);
                    } 
                    
					if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
					{
						//---10% off $100
						//---Rounded to the next number
						return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * couponFromDb.Discount/100),2);
					}					
                }
            }

			return OriginalOrderTotal;
        }
	}
}
