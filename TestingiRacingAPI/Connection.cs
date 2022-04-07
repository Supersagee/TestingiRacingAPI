using Aydsko.iRacingData;
using Aydsko.iRacingData.Member;
using Aydsko.iRacingData.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingiRacingAPI
{
    public class Connection
    {             
        
        private readonly IDataClient dataClient;

        public Connection(IDataClient dataClient)
        {
            this.dataClient = dataClient;
        }

        public async Task<MemberInfo> GetMyInfoAsync(CancellationToken cancellationToken = default)
        {
            var infoResponse = await dataClient.GetMyInfoAsync(cancellationToken);
            return infoResponse.Data;
        }
    }
}
