// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	class SelfPresentityMonitor
		: Presentity
	{
		//private IUccCategoryInstance stateCategoryInstance;
		//private EventHandler stateInstanceUpdated;

		//public SelfPresentityMonitor(EventHandler stateInstanceUpdated)
		//{
		//    this.stateInstanceUpdated = stateInstanceUpdated;
		//}

		//public override AvailabilityValues Availability
		//{
		//    get
		//    {
		//        if (this.stateCategoryInstance == null)
		//            return AvailabilityValues.Unknown;
		//        return base.Availability;
		//    }
		//    protected set
		//    {
		//        base.Availability = value;
		//    }
		//}
		

		//public override UccPresentity DestroyUccPresentity()
		//{
		//    this.stateCategoryInstance = null;
		//    return base.DestroyUccPresentity();
		//}

		//public IUccCategoryInstance CreatePublishableStateCategoryInstance()
		//{
		//    if (stateCategoryInstance != null)
		//        return stateCategoryInstance.CreatePublishableCategoryInstance();
		//    return null;
		//}

		//protected override void ProcessCategoryInstance(IUccCategoryInstance categoryInstance)
		//{
		//    base.ProcessCategoryInstance(categoryInstance);

		//    switch (categoryInstance.CategoryContext.Name.Trim())
		//    {
		//        case "state":
		//            stateCategoryInstance = categoryInstance;
		//            if (this.stateInstanceUpdated != null)
		//                stateInstanceUpdated(this, null);
		//            break;
		//        case "contactCard":
		//            break;
		//    }
		//}
	}
}
