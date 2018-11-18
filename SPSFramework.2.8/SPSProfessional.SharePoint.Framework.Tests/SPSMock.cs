using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using TypeMock;

namespace SPSProfessional.SharePoint.Framework.Tests
{
    public class SPSMock
    {
        public static void SPContextWeb()
        {            
            using (RecordExpectations rec = new RecordExpectations())
            {                
                SPWeb web = RecorderManager.CreateMockedObject<SPWeb>(Constructor.Mocked);                
                rec.ExpectAndReturn(SPContext.Current.Web, web).RepeatAlways();                
            }
        }
    }

    public class SPSMockSPContext
    {
        public SPSMockSPContext()
        {

        }
    }
}
