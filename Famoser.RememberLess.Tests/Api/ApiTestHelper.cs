using System;
using Famoser.FrameworkEssentials.Singleton;

namespace Famoser.RememberLess.Tests.Api
{
    public class ApiTestHelper : SingletonBase<ApiTestHelper>
    {
        public static Guid TestUserGuid = Guid.Parse("302f864d-0842-473d-bab6-28dfd1902e5e");
        
    }
}
